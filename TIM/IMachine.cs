namespace TIM
{
    /// <summary>
    /// Represents the state machine.
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public interface IMachine<TKey, TContext>
    {
        /// <summary>
        /// Gets the name of the state machine.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the context for this state machine.
        /// </summary>
        TContext Context { get; }
        /// <summary>
        /// Gets a reference to the states of this state machine.
        /// </summary>
        IStates<TKey> States { get; }
    }
}
