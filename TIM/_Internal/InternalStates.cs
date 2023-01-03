using System;
using System.Collections.Generic;
using System.Linq;
using TIM._Internal;

namespace TIM
{
    internal class InternalStates<TKey, TContext> : IStates<TKey>
    {
        /// <summary>
        /// Gives the keys of all states in the state machine.
        /// </summary>
        public TKey[] AllStates { get { return __All.Select(x => x.Key).ToArray(); } }
        /// <summary>
        /// Gets the key of the state in which the machine was started.
        /// </summary>
        public TKey Start { get; set; } = default(TKey);
        /// <summary>
        /// Gives the key of the state the machine is currently in.
        /// </summary>
        public TKey Current { get { return _Current.Key; } }       
        /// <summary>
        /// Test if the given key exists in the list of states.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>Tue if the key exists, otherwise false.</returns>
        public bool IsValidKey(TKey key)
        {
            return __All.ContainsKey(key);
        }
        /// <summary>
        /// Event that is triggered each thine the machine change state.
        /// </summary>
        public event Action<TKey> OnStateChanged;

        #region internals
        private Dictionary<TKey, IState<TKey, TContext>> __All = new Dictionary<TKey, IState<TKey, TContext>>();
        private IState<TKey, TContext> __Current = null;
        internal IState<TKey, TContext> _Current
        {
            get { return __Current; }
        }
        internal InternalStates(IState<TKey, TContext>[] states, Type interfaceType)
        {
            if (states == null) throw new ArgumentNullException("states");
            if (states.Length == 0) throw new ArgumentException("Must define at least one state.", "states");

            foreach (IState<TKey, TContext> state in states)
            {
                if (state == null) throw new ArgumentException("The list of given states contains a 'null' value", "states");
                if (state.Key == null) throw new ArgumentException(string.Format("State '{0}' has a 'null' key.", state.Key), "states");

                if (__All.ContainsKey(state.Key))
                {
                    throw new ArgumentException(string.Format("The list of given states contain duplicate state key '{0}'.", state.Key), "states");
                }

                if (!TestState(state, interfaceType))
                {
                    throw new ArgumentException(string.Format("State '{0}' does not (fully) implement trigger interface '{1}'.", state.Key, interfaceType.Name), "states");
                }

                __All[state.Key] = state;
            }
        }
        internal void InitStates(InternalControl<TKey, TContext> control)
        {
            foreach (IState<TKey, TContext> state in __All.Values)
            {
                state.Initialize(control);               
            }
        }
        internal IState<TKey, TContext> FindState(TKey state)
        {
            if (AllStates.Contains(state)) return __All[state];
            return null;
        }
        internal void ChangeState(IState<TKey, TContext> newState)
        {
            if (__Current == newState) return;
            __Current = newState;

            OnStateChanged?.Invoke(__Current.Key);
        }
        internal void StartInState(IState<TKey, TContext> newState)
        {
            __Current = newState;
        }

        private bool TestState(IState<TKey, TContext> state, Type interfaceType)
        {
                //ToDo: find more efficient way to check on interface
            if (state.GetType().GetInterfaces().Contains(interfaceType))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
