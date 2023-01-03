namespace TIM
{
    /// <summary>
    /// Represents start/stop state functionality in a state.
    /// Called by the state machine when its state is changed. (Idle, Running or Error.)
    /// Implement this interface when you need to do initialization/closing of the context based on the state.
    /// </summary>
    public interface IStateStartStop
    {
        /// <summary>
        /// Called by the state machine when it (re-)starts in this state.
        /// </summary>
        void Start();
        /// <summary>
        /// Called by the state machine when it stops in this state.
        /// </summary>
        void Stop();
    }
}
