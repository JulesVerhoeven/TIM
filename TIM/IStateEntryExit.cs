namespace TIM
{
    /// <summary>
    /// Represents entry/exit state functionality in a state.
    /// Implement this if you want to perform actions each time the state is entered or exited.
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    public interface IStateEntryExit<TKey>
    {
        /// <summary>
        /// Called by the state machine when this state is entered. (After a Goto())
        /// </summary>
        /// <param name="from">The key of the previous state.</param>
        void Entry(TKey from);
        /// <summary>
        /// Called by the state machine before moving to the next state. (After a Goto())
        /// </summary>
        /// <param name="to">The key of the next state.</param>
        void Exit(TKey to);
    }
}
