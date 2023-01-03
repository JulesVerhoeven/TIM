using System;

namespace TIM
{
    /// <summary>
    /// Represents errors that occur during trigger execution
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    public class MachineException<TKey> : Exception
    {
        /// <summary>
        /// Gets the name of the state machine where the error occurred.
        /// </summary>
        public string Machine { get; private set; }
        /// <summary>
        /// Gets the key of the state where the error occurred.
        /// </summary>
        public TKey State { get; private set; }
        /// <summary>
        /// Gets a string representation of the trigger during which the error occurred.
        /// </summary>
        public string Trigger { get; private set; }
        /// <summary>
        /// Gets the location in the state where the error occurred.
        /// </summary>
        public string Location { get; private set; }
        /// <summary>
        /// Initializes a new instance of the MachineException class with the machine, state, trigger, location and 
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="machine">The name of the state machine where the error occurred.</param>
        /// <param name="state">The key of the state where the error occurred.</param>
        /// <param name="trigger">A string representation of the trigger during which the error occurred.</param>
        /// <param name="location">The location in the state where the error occurred.</param>
        /// <param name="innerException">A reference to the inner exception that is the cause of this exception</param>
        public MachineException(string machine, TKey state, string trigger, string location, Exception innerException) 
            : base(string.Format("An exception has been thrown in state {0}.{1} while handling trigger {2}.", machine, state.ToString(), trigger), innerException)
        {
            Machine = machine;
            State = state;
            Trigger = trigger;
            Location = location;
        }
    }
}
