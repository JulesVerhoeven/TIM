namespace TIM
{
    /// <summary>
    /// Represents the overall state of the state machine.
    /// </summary>
    public enum MachineState
    {
        /// <summary>
        /// The state machine has been created but is not run yet.
        /// </summary>
        Idle,
        /// <summary>
        /// The state machine is running.
        /// </summary>
        Running,
        /// <summary>
        /// The state machine has encountered an exception and is now in error state.
        /// </summary>
        Error
    }
}
