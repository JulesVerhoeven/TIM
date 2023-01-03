using System;
using TIM._Internal;

namespace TIM.Accepts
{
    /// <summary>
    /// Gives the possibility to work with accept states
    /// </summary>
    public static class AcceptExtensions
    {
        /// <summary>
        /// Extension method that allows to test a state for acceptance
        /// </summary>
        /// <typeparam name="TKey">The type of the key for the states.</typeparam>
        /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
        /// <param name="state">A reference to the state.</param>
        /// <returns>True if it is an accept state, false otherwise.</returns>
        public static bool IsAcceptState<TKey, TContext>(this IState<TKey, TContext> state)
        {
            return (state is IStateAccept);
        }
        /// <summary>
        /// Extension method that gives the possibility to set a handler that is called each time the state machine reaches an accept state.
        /// </summary>
        /// <typeparam name="TKey">The type of the key for the states.</typeparam>
        /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
        /// <param name="options"></param>
        /// <param name="acceptHandler">The handler that is called when the state machine reaches an accept state. TKey gives the key of the accept state.</param>
        public static void SetAcceptHandler<TKey, TContext>(this IMachineOptions<TKey, TContext> options, Action<TKey> acceptHandler)
        {
            (options as InternalMachineOptions<TKey, TContext>).OnAccepted = acceptHandler;
        }
    }
}
