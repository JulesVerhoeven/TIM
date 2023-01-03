using System;
using System.Threading;
using System.Threading.Tasks;

namespace TIM
{
    /// <summary>
    /// Represents the state machine control. (Used within states)
    /// </summary>
    /// <typeparam name="TKey">The type of the key for the states.</typeparam>
    /// <typeparam name="TContext">The type of the context for this state machine.</typeparam>
    public interface IMachineControl<TKey, TContext> : IMachine<TKey, TContext>
    {
        /// <summary>
        /// Instructs the state machine to move to the given state 
        /// after the trigger, entry or exit method has finished.
        /// </summary>
        /// <param name="state">The state to move to next.</param>
        void GoTo(TKey state);
        /// <summary>
        ///  Instructs the state machine to execute 'onComplete' after the given task has finished.
        ///  The instruction is cancelled if the state machine moves to a different state.
        /// </summary>
        /// <param name="description">The description of this call. (Is used in tracing.)</param>
        /// <param name="task">The task to await.</param>
        /// <param name="onComplete">The action to execute when the task has successfully finished.</param>
        /// <param name="onException">The action to execute when the task threw an exception.</param>
        /// <param name="tokenSource">An optianal TokenSource that will be cancelled when the state machine moves to a different state.</param>
        void Call(string description, Task task, Action onComplete, Action<Exception> onException = null, CancellationTokenSource tokenSource = null);
        /// <summary>
        ///  Instructs the state machine to execute 'onComplete' after the given task has finished.
        ///  The instruction is cancelled if the state machine moves to a different state.
        /// </summary>
        /// <typeparam name="T">The type of the task result.</typeparam>
        /// <param name="description">The description of this call. (Is used in tracing.)</param>
        /// <param name="task">The task to await.</param>
        /// <param name="onComplete">The action to execute when the task has successfully finished.</param>
        /// <param name="onException">The action to execute when the task threw an exception.</param>
        /// <param name="tokenSource">An optianal TokenSource that will be cancelled when the state machine moves to a different state.</param>
        void Call<T>(string description, Task<T> task, Action<T> onComplete, Action<Exception> onException = null, CancellationTokenSource tokenSource = null);
    }
}
