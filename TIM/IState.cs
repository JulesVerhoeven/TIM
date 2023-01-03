namespace TIM
{
    /// <summary>
    /// Represents basic state functionality used by the state machine.
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public interface IState<TKey, TContext>
    {
        /// <summary>
        /// Gets the key that identifies the state.
        /// </summary>
        TKey Key { get; }
        /// <summary>
        /// Called by the state machine when the machine is initialized.
        /// The machine parameter should be stored to provide acces to the state machine from within the state.
        /// </summary>
        /// <param name="machine">A reference state machine</param>
        void Initialize(IMachineControl<TKey, TContext> machine);
    }
}
