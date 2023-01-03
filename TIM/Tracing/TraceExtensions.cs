using System;
using TIM._Internal;

namespace TIM.Tracing
{
    /// <summary>
    /// Defines the trace extensions on the state machines.
    /// </summary>
    public static class TraceExtensions
    {
        /// <summary>
        /// Allows the user to set a handler for the trace events.
        /// </summary>
        /// <typeparam name="TKey">The type of the key for the states.</typeparam>
        /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
        /// <param name="options">The state machine options where the tracehandler can be set.</param>
        /// <param name="trace">The handler to set.</param>
        /// <param name="traceEvents">Optional, specifies the events to trace. Defaults to all events.</param>
        public static void SetTraceHandler<TKey, TContext>(this IMachineOptions<TKey, TContext> options, Action<TraceData> trace, TraceEvents traceEvents = TraceEvents.All)
        {
            (options as InternalMachineOptions<TKey, TContext>).TraceHandler = trace;
        }
        /// <summary>
        /// Allows the user to specify the evnts he wants to see in the trace.
        /// </summary>
        /// <typeparam name="TKey">The type of the key for the states.</typeparam>
        /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
        /// <param name="options">The state machine options where the events to trace can be set.</param>
        /// <param name="traceEvents">The events to trace.</param>
        public static void SetEventsToTrace<TKey, TContext>(this IMachineOptions<TKey, TContext> options, TraceEvents traceEvents)
        {
            (options as InternalMachineOptions<TKey, TContext>).EventsToTrace = traceEvents;
        }
        /// <summary>
        /// Allows the user to send a trace event from within a state machine.
        /// </summary>
        /// <typeparam name="TKey">The type of the key for the states.</typeparam>
        /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
        /// <param name="machine">The machine on which this trace event occurs.</param>
        /// <param name="message">The message to send with the trace event.</param>
        public static void Trace<TKey, TContext>(this IMachine<TKey, TContext> machine, string message)
        {
            (machine as InternalControl<TKey, TContext>).SendTrace(message);
        }
        /// <summary>
        /// Allows the user to send a trace event from within a state.
        /// </summary>
        /// <typeparam name="TKey">The type of the key for the states.</typeparam>
        /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
        /// <param name="state">The state in which the trace event occurs</param>
        /// <param name="message">The message to send with the trace event.</param>
        /// <exception cref="Exception">throws if the state has not been initialized yet.</exception>
        public static void Trace<TKey, TContext>(this State<TKey, TContext> state, string message)
        {
            if (state.MachineControl == null) throw new Exception("The state must have been initialized before you can trace.");
            state.MachineControl.Trace(message);
        }          
    }
}
