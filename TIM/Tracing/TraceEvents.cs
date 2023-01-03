using System;

namespace TIM.Tracing
{
    /// <summary>
    /// Enumerates the possible trace events.
    /// </summary>
    [Flags]
    public enum TraceEvents
    {
        /// <summary>
        /// A trigger is started.
        /// </summary>
        TriggerStart = 0x0001,
        /// <summary>
        /// A trigger has ended.
        /// </summary>
        TriggerEnd = 0x0002,
        /// <summary>
        /// All trigger events.
        /// </summary>
        Trigger = TriggerStart | TriggerEnd,
        /// <summary>
        /// A state has been entered.
        /// </summary>
        StateEntry = 0x0004,
        /// <summary>
        /// A state has been exited.
        /// </summary>
        StateExit = 0x0008,
        /// <summary>
        /// The state machine starts in state.
        /// </summary>
        StateStart = 0x0010,
        /// <summary>
        /// The state machine stops in state.
        /// </summary>
        StateStop = 0x0020,
        /// <summary>
        /// All state events.
        /// </summary>
        State = StateEntry| StateExit | StateStart | StateStop,
        /// <summary>
        /// The state machine is aborted.
        /// </summary>
        Aborted = 0x0040,
        /// <summary>
        /// The state machine is reset.
        /// </summary>
        Reset = 0x0080,
        /// <summary>
        /// The state machine reached an accept state.
        /// </summary>
        Accepted = 0x0100,
        /// <summary>
        /// The state machine had an exception.
        /// </summary>
        Exception = 0x0200,
        /// <summary>
        /// All state machine events.
        /// </summary>
        StateMachine = Aborted | Reset| Accepted| Exception,    
        /// <summary>
        /// The start of a Call.
        /// </summary>
        CallStart = 0x0400,
        /// <summary>
        /// The cancellation of a call.
        /// </summary>
        CallCancelled = 0x0800,
        /// <summary>
        /// All call events.
        /// </summary>
        Call = CallStart | CallCancelled,
        /// <summary>
        /// A user defined trace.
        /// </summary>
        User = 0x1000,
        /// <summary>
        /// All trace events.
        /// </summary>
        All = Trigger | State | StateMachine | Call | User 
    }
}
