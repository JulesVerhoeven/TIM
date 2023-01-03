using System;
using System.Collections.Generic;
using System.Linq;
using TIM._Internal;

namespace TIM
{
    /// <summary>
    /// Represents an interface state machine.
    /// </summary>
    /// <typeparam name="TInterface">The type of the trigger interface.</typeparam>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public class Machine<TInterface, TKey, TContext> : IMachine<TKey, TContext>
    {
        private object _Proxy = null;
        internal InternalControl<TKey, TContext> _Control { get { return (InternalControl<TKey, TContext>)_Proxy; } }

        #region IMachine<TKey, TContext>
        /// <summary>
        /// Gets the name of the state machine.
        /// </summary>
        public string Name { get { return _Control.Name; } }
        /// <summary>
        /// Gets the context of the state machine.
        /// </summary>
        public TContext Context { get { return _Control.Context; } }
        /// <summary>
        /// Gets a reference to the states of this state machine.
        /// </summary>
        public IStates<TKey> States { get; private set; }
        #endregion
        /// <summary>
        /// Gets the trigger interface for the state machine.
        /// </summary>
        public TInterface Interface { get { return (TInterface)_Proxy; } }
        /// <summary>
        /// Gets the state the state machine is currently in.
        /// </summary>
        public MachineState MachineState { get { return _Control.MachineState; } }
        /// <summary>
        /// Gets a reference to the options of this state machine.
        /// </summary>
        public IMachineOptions<TKey, TContext> Options { get { return _Control._Options; } }
        /// <summary>
        /// Creates a new state machine with the given states.
        /// </summary>
        /// <param name="states">The states for this state machine.</param>
        public Machine(params IState<TKey, TContext>[] states) :
            this("TIM", states)
        {}
        /// <summary>
        /// Creates a new state machine with the given states.
        /// </summary>
        /// <param name="states">The states for this state machine.</param>
        /// <exception cref="System.ArgumentNullException">: states is null.</exception>
        /// <exception cref="System.ArgumentException">: states has 0 elements.</exception>
        /// <exception cref="System.ArgumentException">: a given states is null.</exception>
        /// <exception cref="System.ArgumentException">: a given state has a null key.</exception>
        /// <exception cref="System.ArgumentException">: one or more states have the same key.</exception>
        /// <exception cref="System.ArgumentException">: a given state does not implement TInterface.</exception>
        public Machine(IEnumerable<IState<TKey, TContext>> states) :
            this("TIM", states.ToArray())
        {}
        /// <summary>
        /// Creates a new state machine with the given name and states.
        /// </summary>
        /// <param name="name">The name of this state machine.</param>
        /// <param name="states">The states of this state machine.</param>
        /// <exception cref="System.ArgumentNullException">: states is null.</exception>
        /// <exception cref="System.ArgumentException">: states has 0 elements.</exception>
        /// <exception cref="System.ArgumentException">: a given states is null.</exception>
        /// <exception cref="System.ArgumentException">: a given state has a null key.</exception>
        /// <exception cref="System.ArgumentException">: one or more states have the same key.</exception>
        /// <exception cref="System.ArgumentException">: a given state does not implement TInterface.</exception>
        public Machine(string name, params IState<TKey, TContext>[] states)
        {
            States = new InternalStates<TKey, TContext>(states, typeof(TInterface));
            _Proxy = Activator.CreateInstance(ProxyTypeBuilder<TInterface, TKey, TContext>.GeneratedType, States);
            _Control.Name = name;
        }
        /// <summary>
        /// Creates a new state machine with the given name and states.
        /// </summary>
        /// <param name="name">The name of this state machine.</param>
        /// <param name="states">The states of this state machine.</param>
        /// <exception cref="System.ArgumentNullException">: states is null.</exception>
        /// <exception cref="System.ArgumentException">: states has 0 elements.</exception>
        /// <exception cref="System.ArgumentException">: a given states is null.</exception>
        /// <exception cref="System.ArgumentException">: a given state has a null key.</exception>
        /// <exception cref="System.ArgumentException">: one or more states have the same key.</exception>
        /// <exception cref="System.ArgumentException">: a given state does not implement TInterface.</exception>
        public Machine(string name, IEnumerable<IState<TKey, TContext>> states) :
            this(name, states.ToArray())
        {}
        /// <summary>
        /// Run the state machine.
        /// </summary>
        /// <param name="startState">The state to start in.</param>
        /// <param name="context">The context for the states.</param>
        /// <returns>The interface upon which you can trigger the state machine.</returns>
        public TInterface Run(TKey startState, TContext context)
        {
            _Control.Run(startState, context);
            return Interface;
        }
        /// <summary>
        /// Resets the state machine to the original state.
        /// </summary>
        public void Reset()
        {
            _Control.Reset();
        }
        /// <summary>
        /// Resets the state machine to the given state.
        /// </summary>
        /// <param name="state">The state to start in after the reset.</param>
        public void Reset(TKey state)
        {
            _Control.Reset(state);
        }
        /// <summary>
        /// Stops the state machine and puts it in error state
        /// </summary>
        public void Halt()
        {
            _Control.Halt();
        }
    }
}
