using System;

namespace TIM
{
    /// <summary>
    /// Represents the options you can set on a state machine. 
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public interface IMachineOptions<TKey, TContext>
    {
        /// <summary>
        /// Gets or sets the maximum number of state changes the state machine allows on a single trigger.
        /// You cannot change this when the machine is running.
        /// Default is 64 state changes.
        /// </summary>
        int MaxNumberOfStateChanges { get; set; }
        /// <summary>
        /// Gets the object that is used for locking, that is synchronizing the access to the state machine.
        /// This defaults to the given context if this is a non-null reference object, otherwise it is the control class itself.
        /// </summary>
        object LockObject { get; }
        /// <summary>
        /// When set, the state machine invokes the handler every time a trigger is called.
        /// </summary>
        /// <param name="handler">The handler to call:
        ///     TKey: Gives the state on which the trigger is called.
        ///     string: The string representation of the trigger method.</param>
        void SetTriggerBeginHandler(Action<TKey, string> handler);
        /// <summary>
        /// When set, the state machine invokes the handler every time a trigger is finished.
        /// </summary>
        /// <param name="handler">The handler to invoke:
        ///     TKey: Gives the state in which the trigger call ends.
        ///     bool: True if a Goto() has been called during the trigger, false otherwise.
        ///     bool: True if an exception has been thrown during the execution of the trigger, false otherwise</param>
        void SetTriggerEndHandler(Action<TKey, bool, bool> handler);
        /// <summary>
        /// When set, the state machine invokes the handler every time it is started. (also in Reset())
        /// </summary>
        /// <param name="handler">The handler to invoke:
        ///     TKey: Gives the state in which the machine starts.
        ///     TContext: Gives the context.</param>
        void SetMachineStartHandler(Action<TKey, TContext> handler);
        /// <summary>
        /// When set, the state machine invokes the handler every time it is stopped. (also in Reset())
        /// </summary>
        /// <param name="handler">The handler to invoke:
        ///     TKey: Gives the state in which the machine stops.
        ///     TContext: Gives the context.</param>
        void SetMachineStopHandler(Action<TKey, TContext> handler);
    }
}
