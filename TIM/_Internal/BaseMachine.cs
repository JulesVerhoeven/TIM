//using System;

//namespace TIM._Internal
//{
//    /// <summary>
//    /// The base class for all interface state machines
//    /// </summary>
//    /// <typeparam name="TInterface">The type of the trigger interface.</typeparam>
//    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
//    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
//    public abstract class BaseMachine<TInterface, TKey, TContext> : IMachine<TKey, TContext>
//    {
//        private object _Proxy = null;         
//        internal InternalControl<TKey, TContext> _Control { get { return (InternalControl<TKey, TContext>)_Proxy; } }

//        #region IMachine<TKey, TContext>
//        /// <summary>
//        /// Gets the name of the state machine.
//        /// </summary>
//        public string Name { get { return _Control.Name; } }

//        /// <summary>
//        /// Gets the context of the state machine.
//        /// </summary>
//        public TContext Context { get { return _Control.Context; } }

//        /// <summary>
//        /// Gets a reference to the states of this state machine.
//        /// </summary>
//        public IStates<TKey> States { get; private set; }

//        #endregion
//        /// <summary>
//        /// Gets the trigger interface for the state machine.
//        /// </summary>
//        public TInterface Interface { get { return (TInterface)_Proxy; } }

//        /// <summary>
//        /// Gets the state the state machine is currently in.
//        /// </summary>
//        public MachineState MachineState { get { return _Control.MachineState; } }

//        /// <summary>
//        /// Gets a reference to the options of this state machine.
//        /// </summary>
//        public IMachineOptions<TKey, TContext> Options { get { return _Control._Options; } }
    

//        internal BaseMachine(params IState<TKey, TContext>[] states) :
//            this("TIM", states)
//        {
//        }

//        internal BaseMachine(string name, params IState<TKey, TContext>[] states)
//        {
//            //TODO: Test on interface and context, they must be public
//            States = new InternalStates<TKey, TContext>(states, typeof(TInterface));
//            _Proxy = Activator.CreateInstance(ProxyTypeBuilder<TInterface, TKey, TContext>.GeneratedType, States);
//            _Control.Name = name;
//        }

//        /// <summary>
//        /// Resets the state machine to the original state.
//        /// </summary>
//        public void Reset()
//        {
//            _Control.Reset();
//        }

//        /// <summary>
//        /// Resets the state machine to the given state.
//        /// </summary>
//        /// <param name="state">The state to start in after the reset.</param>
//        public void Reset(TKey state)
//        {
//            _Control.Reset(state);
//        }

//        /// <summary>
//        /// Stops the state machine and puts it in error state
//        /// </summary>
//        public void Halt()
//        {
//            _Control.Halt();
//        }
//    }
//}
