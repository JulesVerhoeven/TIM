using TIM._Internal;

namespace TIM.Tracing
{
    /// <summary>
    /// Represents the data generated on a trace
    /// </summary>
    public class TraceData
    {
        internal InternalTriggerInfo TriggerData;

        internal TraceData(TraceEvents traceEvent, string machine, string state, InternalTriggerInfo triggerData, string message = "")
        {
            Event = traceEvent;
            Machine = machine;
            State = state;
            TriggerData = triggerData;
            Message = message;
        }
        /// <summary>
        /// The trace event that occureed.
        /// </summary>
        public TraceEvents Event { get; private set; }
        /// <summary>
        /// The name of the state machine where the trace event occurred.
        /// </summary>
        public string Machine { get; private set; }
        /// <summary>
        /// The state in which the trace event occurred.
        /// </summary>
        public string State { get; private set; }
        /// <summary>
        /// The name of the interface that was called.
        /// </summary>
        public string InterfaceName { get { return (TriggerData == null || TriggerData.Interface == null) ? "" : TriggerData.Interface.Name; } }
        /// <summary>
        /// The name of the methid called.
        /// </summary>
        public string MethodName { get { return (TriggerData == null) ? "" : TriggerData.Trigger; } }
        /// <summary>
        /// The message of the trace.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// A description of the trigger that was invoked.
        /// </summary>
        public string Trigger { get { return (TriggerData == null) ? "" : TriggerData.ToString(); } }
        /// <summary>
        /// Gives a full description of the trace event.
        /// </summary>
        /// <returns>The description.</returns>
        public override string ToString()
        {
            switch (Event)
            {
                case TraceEvents.TriggerStart:
                    return string.Format("{0} fired on state {1}.{2}.", Trigger, Machine, State);
                case TraceEvents.TriggerEnd:
                    return string.Format("{0} finished in state {1}.{2}.", Trigger, Machine, State);
                case TraceEvents.StateEntry:
                    return string.Format("Enter state {0}.{1}.", Machine, State);
                case TraceEvents.StateExit:
                    return string.Format("Exit state {0}.{1}.", Machine, State);
                case TraceEvents.StateStart:
                    return string.Format("Start state {0}.{1}.", Machine, State);
                case TraceEvents.StateStop:
                    return string.Format("Stop state {0}.{1}.", Machine, State);
                case TraceEvents.Aborted:
                    return string.Format("Trigger aborted in state {0}.{1}.", Machine, State);
                case TraceEvents.Reset:
                    return string.Format("Reset {1}, new state {0}.", Machine, State);
                case TraceEvents.Accepted:
                    return string.Format("Reached accept state {0}.{1}.", Machine, State);
                case TraceEvents.User:
                    return string.Format("On {0} in {1}.{2}: {3}.", Trigger, Machine, State, Message);
                case TraceEvents.Exception:
                    return string.Format("Exception on {0} in {1}.{2}: {3}.", Trigger, Machine, State, Message);
                case TraceEvents.CallStart:
                    return string.Format("Call {0}() in {1}.{2}.", Message, Machine, State);
                case TraceEvents.CallCancelled:
                    return string.Format("Call {0}() was cancelled.", Message, Machine, State);
            }

            return "";
        }
    }
}
