using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TIM.Accepts;
using TIM.Tracing;

namespace TIM._Internal
{
    /// <summary>
    /// Internal class used by the proxy interface and state implementations.\
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public abstract class InternalControl<TKey, TContext> : IMachineControl<TKey, TContext>
    {
        internal InternalMachineOptions<TKey, TContext> _Options = null;
        /// <summary>
        /// Gives the current overall state of the state machine.
        /// </summary>
        public MachineState MachineState { get { return _MachineState; } }
        /// <summary>
        /// Gives the name of the state machine
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Gives the context of the state machine
        /// </summary>
        public TContext Context { get; private set; }
        /// <summary>
        /// Gives the list of all states defined within this state machine.
        /// </summary>
        public IStates<TKey> States { get { return _States; } }
        /// <summary>
        /// Creates a new internal control class.
        /// </summary>
        /// <param name="states">The list of states in this state machine.</param>
        public InternalControl(IStates<TKey> states)
        {
            Name = "TIM";
            _Options = new InternalMachineOptions<TKey, TContext>(this);
            _States = (InternalStates<TKey, TContext>)states;
            _States.InitStates(this);
        }
        internal void Run(TKey startState, TContext context)
        {
            lock (_Options.LockObject)
            {
                if (MachineState == MachineState.Running) throw new InvalidOperationException("The machine has already been started.");
                Start(startState, context);
                _States.Start = startState;
            }
        }
        internal void Reset()
        {
            Reset(_States.Start);
        }
        internal void Reset(TKey startState)
        {
            lock (_Options.LockObject)
            {
                if (MachineState == MachineState.Idle) throw new InvalidOperationException("The machine has not been started yet.");
                if (_RunInfo.TriggerInfo != null) throw new Exception(string.Format("You are not allowed to reset the machine from inside a trigger."));

                _RunInfo.PreviousState = _States._Current;
                try
                {
                    Stop();
                    Start(startState, Context);
                }
                catch (Exception)
                {
                    throw;
                }

                _States.Start = startState;
            }
        }
        internal void Halt()
        {
            lock (_Options.LockObject)
            {
                Stop();
            }
        }

        #region Tracing
        internal TraceEvents EventsToTrace { get { return _Options.TraceHandler == null ? 0 : _Options.EventsToTrace; } }
        internal void FireTrace(TraceData trace)
        {
            try
            {
                if (_RunInfo.AcceptMoves)
                {
                    _RunInfo.AcceptMoves = false;
                    _Options.TraceHandler(trace);
                    _RunInfo.AcceptMoves = true;
                }
                else
                {
                    _Options.TraceHandler(trace);
                }
            }
            catch (Exception)
            { }
        }
        internal void SendTrace(string message)
        {
            if ((EventsToTrace & TraceEvents.User) != 0) FireTrace(new TraceData(TraceEvents.User, Name, States.Current.ToString(), _RunInfo.TriggerInfo, message));
        }
        #endregion

        #region IMachineControl<TKey, TContext>
        private int _CurrentCallId = 0;
        private readonly Dictionary<int, CancellationTokenSource> _CurrentCalls = new();

        void IMachineControl<TKey, TContext>.GoTo(TKey state)
        {
            if (!Monitor.IsEntered(_Options.LockObject) || !_RunInfo.AcceptMoves) throw new Exception(string.Format("Attempt to call GoTo() outside of a trigger, entry or exit."));
            IState<TKey, TContext> State = _States.FindState(state) ?? throw new Exception(string.Format("State '{0}' does not exist in the defined states.", state));
            _RunInfo.NextState = State;
        }
        void IMachineControl<TKey, TContext>.Call(string description, Task task, Action onComplete, Action<Exception> onException, CancellationTokenSource tokenSource)
        {
            int Id = ++_CurrentCallId;
            _CurrentCalls[Id] = tokenSource;
            if ((EventsToTrace & TraceEvents.CallStart) != 0) FireTrace(new TraceData(TraceEvents.CallStart, Name, States.Current.ToString(), null, description));

            task.ContinueWith(task =>
                {
                    try
                    {
                        TriggerStart(null, description);
                        if (!_CurrentCalls.ContainsKey(Id) || task.IsCanceled)
                        {
                            if ((EventsToTrace & TraceEvents.CallCancelled) != 0) FireTrace(new TraceData(TraceEvents.CallCancelled, Name, "", null, description));
                            return;
                        }
                        if (task.Exception != null)
                        {
                            onException?.Invoke(task.Exception);
                        }
                        else
                        {
                            onComplete?.Invoke();
                        }
                        Move();
                    }
                    catch (Exception e)
                    {
                        HandleException(e);
                    }
                    finally
                    {
                        TriggerEnd();
                    }

                }, TaskContinuationOptions.RunContinuationsAsynchronously);
        }
        void IMachineControl<TKey, TContext>.Call<T>(string description, Task<T> task, Action<T> onComplete, Action<Exception> onException, CancellationTokenSource tokenSource)
        {
            int Id = ++_CurrentCallId;
            _CurrentCalls[Id] = tokenSource;
            if ((EventsToTrace & TraceEvents.CallStart) != 0) FireTrace(new TraceData(TraceEvents.CallStart, Name, States.Current.ToString(), null, description));

            task.ContinueWith(task =>
            {
                if (!_CurrentCalls.ContainsKey(Id) || task.IsCanceled) return;
                TriggerStart(null, description);
                try
                {
                    if (task.Exception != null)
                    {
                        onException?.Invoke(task.Exception);
                    }
                    else
                    {
                        onComplete?.Invoke(task.Result);
                    }
                    Move();
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
                finally
                {
                    TriggerEnd();
                }

            }, TaskContinuationOptions.RunContinuationsAsynchronously);
        }
        #endregion

        #region derived proxy calls
        /// <summary>
        /// Tells the state machine to start a new trigger.
        /// </summary>
        /// <param name="interfaceType">The type of the interface that was called.</param>
        /// <param name="trigger">The name of the trigger.</param>
        /// <returns>The current state.</returns>
        /// <exception cref="Exception">Throws when called from inside a trigger or when the state machine is not running</exception>
        protected object TriggerStart(Type interfaceType, string trigger)
        {
            Monitor.Enter(_Options.LockObject);

            if (_RunInfo.TriggerInfo != null) throw new Exception(string.Format("You are not allowed to trigger from inside a trigger."));
            _RunInfo.Start(_States._Current, new InternalTriggerInfo(interfaceType, trigger));
            if (MachineState == MachineState.Idle) throw new Exception(string.Format("The machine is not running."));
            if (MachineState == MachineState.Error) throw new Exception(string.Format("The machine is in error state."));

            if ((EventsToTrace & TraceEvents.TriggerStart) != 0) FireTrace(new TraceData(TraceEvents.TriggerStart, Name, States.Current.ToString(), _RunInfo.TriggerInfo, ""));
            string name = interfaceType == null ? trigger : interfaceType.Name + "." + trigger;
            _Options.TriggerBegin?.Invoke(_States.Current, name);
            _RunInfo.AcceptMoves = true;

            return _States._Current;
        }
        /// <summary>
        /// Tells the state machine to move to another state.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void Move()
        {
            while (_RunInfo.NextState != null)
            {
                JumpToNext();
                if (_RunInfo.TotalJumps > _Options.MaxNumberOfStateChanges)
                {
                    throw new Exception("Exceeded max. number of state changes in a single trigger. Recursive loop suspected.");
                }
            }

            _RunInfo.AcceptMoves = false;
            _Options.TriggerEnd?.Invoke(_States.Current, _RunInfo.TotalJumps > 0, _RunInfo.Exception != null);
        }
        /// <summary>
        /// Tells the state machine that the trigger has ended.
        /// </summary>
        protected void TriggerEnd()
        {
            _RunInfo.AcceptMoves = false;

            //After the Monitor.Exit() __RunInfo can get modified so copy the exception locally
            MachineException<TKey> Exception = _RunInfo.Exception;

            if (MachineState == MachineState.Running && _RunInfo.TotalJumps > 0 && _States._Current.IsAcceptState())
            {
                if ((EventsToTrace & TraceEvents.Accepted) != 0) FireTrace(new TraceData(TraceEvents.Accepted, Name, States.Current.ToString(), _RunInfo.TriggerInfo, ""));
                _RunInfo.Stop();

                FireAccepted(States.Current);
            }
            else
            {
                _RunInfo.Stop();
            }

            if ((EventsToTrace & TraceEvents.TriggerEnd) != 0) FireTrace(new TraceData(TraceEvents.TriggerEnd, Name, States.Current.ToString(), _RunInfo.TriggerInfo, ""));
            Monitor.Exit(_Options.LockObject);

            //Exception must be thrown after we have left the lock
            if (Exception != null)
            {
                throw Exception;
            }
        }
        /// <summary>
        /// Asks the state machine to handle an exception that occurred while executing the trigger.
        /// </summary>
        /// <param name="e"></param>
        protected void HandleException(Exception e)
        {
            if ((EventsToTrace & TraceEvents.Exception) != 0) FireTrace(new TraceData(TraceEvents.Exception, Name, States.Current.ToString(), _RunInfo.TriggerInfo, e.Message));
            _RunInfo.SetAborted();

            _RunInfo.Exception = new MachineException<TKey>(Name, States.Current, _RunInfo.TriggerInfo.Trigger, _RunInfo.Location, e);
        }
        /// <summary>
        /// asks the state machine to finalize the trigger.
        /// </summary>
        protected void TriggerFinalize()
        {
            if (_RunInfo.Exception != null)
            {
                throw _RunInfo.Exception;
            }
        }
        #endregion

        private readonly InternalRunInfo<TKey, TContext> _RunInfo = new();
        private readonly InternalStates<TKey, TContext> _States;
        volatile private MachineState _MachineState = MachineState.Idle;
        private void JumpToNext()
        {
            IState<TKey, TContext> To = _RunInfo.NextState;
            IState<TKey, TContext> From = _States._Current;

            if ((EventsToTrace & TraceEvents.StateExit) != 0) FireTrace(new TraceData(TraceEvents.StateExit, Name, States.Current.ToString(), _RunInfo.TriggerInfo, ""));
            foreach (var source in _CurrentCalls.Values)
            {
                source?.Cancel();
            }
            _CurrentCalls.Clear();

            //OnExit is allowed te redirect to another state, so set Next to null
            _RunInfo.NextState = null;

            if (_States._Current is IStateEntryExit<TKey>)
            {
                _RunInfo.Location = "Exit";
                (_States._Current as IStateEntryExit<TKey>).Exit(To.Key);
            }

            //pick up the new next state, if not set take the old next state
            DoStateChange(_RunInfo.NextState ?? To);
            _RunInfo.Jump();

            if ((EventsToTrace & TraceEvents.StateEntry) != 0) FireTrace(new TraceData(TraceEvents.StateEntry, Name, States.Current.ToString(), _RunInfo.TriggerInfo, ""));
            if (_States._Current is IStateEntryExit<TKey>)
            {
                _RunInfo.Location = "Entry";
                (_States._Current as IStateEntryExit<TKey>).Entry(From.Key);
            }
        }
        private void Start(TKey startState, TContext context)
        {
            IState<TKey, TContext> state = _States.FindState(startState);
            if (state == null) throw new ArgumentException(string.Format("The given start state '{0}' is not a valid state.", startState), nameof(startState));

            _States.StartInState(state);
            Context = context;
            _Options.MachineStart?.Invoke(startState, context);

            if (state is IStateStartStop)
            {
                if ((EventsToTrace & TraceEvents.StateStart) != 0) FireTrace(new TraceData(TraceEvents.StateStart, Name, state.ToString(), _RunInfo.TriggerInfo, ""));
                ((state as IStateStartStop)).Start();
            }

            _MachineState = MachineState.Running;
            DoStateChange(state);
        }
        private void Stop()
        {
            if (MachineState != MachineState.Running) return;

            if (_States._Current is IStateStartStop)
            {
                if ((EventsToTrace & TraceEvents.StateStop) != 0) FireTrace(new TraceData(TraceEvents.StateStop, Name, States.Current.ToString(), _RunInfo.TriggerInfo, ""));
                try
                {
                    (_States._Current as IStateStartStop).Stop();
                }
                catch (Exception e)
                {
                    if ((EventsToTrace & TraceEvents.Exception) != 0) FireTrace(new TraceData(TraceEvents.Exception, Name, States.Current.ToString(), _RunInfo.TriggerInfo, e.Message));
                }
            }

            try
            {
                _Options.MachineStart?.Invoke(_States.Current, Context);
            }
            catch (Exception e)
            {
                if ((EventsToTrace & TraceEvents.Exception) != 0) FireTrace(new TraceData(TraceEvents.Exception, Name, States.Current.ToString(), _RunInfo.TriggerInfo, e.Message));
            }

            _MachineState = MachineState.Error;
        }
        private void DoStateChange(IState<TKey, TContext> newState)
        {
            try
            {
                if (_RunInfo.AcceptMoves)
                {
                    _RunInfo.AcceptMoves = false;
                    _States.ChangeState(newState);
                    _RunInfo.AcceptMoves = true;
                }
                else
                {
                    _States.ChangeState(newState);
                }
            }
            catch (Exception e)
            {
                if ((EventsToTrace & TraceEvents.Exception) != 0) FireTrace(new TraceData(TraceEvents.Exception, Name, States.Current.ToString(), _RunInfo.TriggerInfo, e.Message));
            }
        }
        private void FireAccepted(TKey state)
        {
            try
            {
                _Options.OnAccepted?.Invoke(state);
            }
            catch (Exception e)
            {
                if ((EventsToTrace & TraceEvents.Exception) != 0) FireTrace(new TraceData(TraceEvents.Exception, Name, States.Current.ToString(), _RunInfo.TriggerInfo, e.Message));
            }
        }
    }
}

