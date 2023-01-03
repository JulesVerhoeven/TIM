using System;

namespace TIM
{
    /// <summary>
    /// Represents the collection of states in a state machine.
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    public interface IStates<TKey>
    {
        /// <summary>
        /// Gets the keys of all states in the state machine.
        /// </summary>
        TKey[] AllStates { get; }
        /// <summary>
        /// Gets the key of the state in which the machine was started.
        /// </summary>
        TKey Start { get; }
        /// <summary>
        /// Gets the key of the state the machine is currently in.
        /// </summary>
        TKey Current { get; }
        /// <summary>
        /// Test if the given key exists in the list of states.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>True if the key exists, otherwise false.</returns>
        bool IsValidKey(TKey key);
        /// <summary>
        /// Event that is triggered each thine the machine changes state.
        /// </summary>
        event Action<TKey> OnStateChanged;
    }
}
