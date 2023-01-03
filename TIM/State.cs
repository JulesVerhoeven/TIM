using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIM
{
    /// <summary>
    /// Represents the base state class from which you can derive your own states.
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public abstract class State<TKey, TContext> : IState<TKey, TContext>, IStateEntryExit<TKey>, IStateStartStop
    {
        /// <summary>
        /// Gets a reference to the state machine control.
        /// </summary>
        protected internal IMachineControl<TKey, TContext> MachineControl { get; private set; }
        /// <summary>
        /// Gets the name of the state machine.
        /// </summary>
        public string Machine { get { return MachineControl.Name; } }
        /// <summary>
        /// Gets the context for this state machine.
        /// </summary>
        public TContext Context { get { return MachineControl.Context; } }
        /// <summary>
        /// Gets a reference to the states of this state machine.
        /// </summary>
        public IStates<TKey> States { get { return MachineControl.States; } }
        /// <summary>
        /// Gets the unique key of this state.
        /// </summary>
        public abstract TKey Key { get; }
        /// <summary>
        /// Initializes a new State instance.
        /// </summary>
        public State()
        {
            MachineControl = null;
        }
        /// <summary>
        /// Instructs the state machine to move to the given state. (The moves are done after the trigger has finished.)
        /// If more than one GoTo is called only the last one is accepted.
        /// </summary>
        /// <param name="state">The key of the state to move to. Can only be called in entry/exit/start and inside the trigger.</param>
        protected void GoTo(TKey state)
        {
            MachineControl.GoTo(state);
        }
        /// <summary>
        /// Instructs the state machine to execute 'onComplete' after the timeout has expired.
        /// The instruction is cancelled if the state machine moves to a different state.
        /// </summary>
        /// <param name="description">The description of this timer. (Is used in tracing.)</param>
        /// <param name="timeout">The time in milliseconds after which 'onComplete' is executed.</param>
        /// <param name="onComplete">The action to execute when the timer has finished.</param>
        protected void CallTimer(string description, int timeout, Action onComplete)
        {
            CancellationTokenSource tokenSource = new ();
            Call(description, Task.Delay(timeout, tokenSource.Token), onComplete, null, tokenSource);
        }
        /// <summary>
        ///  Instructs the state machine to execute 'onComplete' after the given task has finished.
        ///  The instruction is cancelled if the state machine moves to a different state.
        /// </summary>
        /// <param name="description">The description of this call. (Is used in tracing.)</param>
        /// <param name="task">The task to await.</param>
        /// <param name="onComplete">The action to execute when the task has successfully finished. (Usually a GoTo())</param>
        /// <param name="onException">The action to execute when the task threw an exception.</param>
        /// <param name="tokenSource">An optianal TokenSource that will be cancelled when the state machine moves to a different state.</param>
        protected void Call(string description, Task task, Action onComplete, Action<Exception> onException = null, CancellationTokenSource tokenSource = null)
        {
            MachineControl.Call(description, task, onComplete, onException, tokenSource);
        }
        /// <summary>
        ///  Instructs the state machine to execute 'onComplete' after the given task has finished.
        ///  The instruction is cancelled if the state machine moves to a different state.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="description">The description of this call. (Is used in tracing.)</param>
        /// <param name="task">The task to await.</param>
        /// <param name="onComplete">The action to execute when the task has successfully finished.</param>
        /// <param name="onException">The action to execute when the task threw an exception.</param>
        /// <param name="tokenSource">An optional TokenSource that will be cancelled when the state machine moves to a different state.</param>
        protected void Call<T>(string description, Task<T> task, Action<T> onComplete, Action<Exception> onException = null, CancellationTokenSource tokenSource = null)
        {
            MachineControl.Call(description, task, onComplete, onException, tokenSource);
        }
        /// <summary>
        /// Called by the state machine when the machine is created. (The context is not valid yet.)
        /// Override this if you need to perform additional inititalization of the state.
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// Called by the state machine when it is started in this state.
        /// Override this if you need to do initialization on the context.
        /// </summary>
        protected virtual void OnStart() { }
        /// <summary>
        /// Called by the state machine when it is stopped in this state.
        /// Override this if you need to do cleaning up on the context.
        /// </summary>
        protected virtual void OnStop() { }
        /// <summary>
        /// Called by the state machine when it is moved to this state.
        /// </summary>
        /// <param name="from">The key of previous state.</param>
        protected virtual void OnEntry(TKey from) { }
        /// <summary>
        /// Called by the state machine when it leaves this state.
        /// </summary>
        /// <param name="to">The key of the next state.</param>
        protected virtual void OnExit(TKey to) { }

        #region IStateStartStop
        void IStateStartStop.Start()
        {
            OnStart();
        }
        void IStateStartStop.Stop()
        {
            OnStop();
        }
        #endregion

        #region IStateEntryExit
        void IStateEntryExit<TKey>.Entry(TKey from)
        {
            OnEntry(from);
        }
        void IStateEntryExit<TKey>.Exit(TKey to)
        {
            OnExit(to);
        }
        #endregion

        #region IState<TKey, TContext>
        void IState<TKey, TContext>.Initialize(IMachineControl<TKey, TContext> machine)
        {
            if (MachineControl != null) throw new Exception("State is already assigned to a different machine.");
            MachineControl = machine;
            OnInit();
        }
        #endregion
    }
}
