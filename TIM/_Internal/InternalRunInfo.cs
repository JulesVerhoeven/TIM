namespace TIM._Internal
{
    internal class InternalRunInfo<TKey, TContext>
    {
        public bool AcceptMoves = false;
        public InternalTriggerInfo TriggerInfo = null;
        public bool IsAborted = false;
        public int TotalJumps = 0;
        public string Location = "Trigger";

        public IState<TKey, TContext> PreviousState = null;
        public IState<TKey, TContext> NextState = null;

        public MachineException<TKey> Exception = null;

        public InternalRunInfo()
        { }

        public void Start(IState<TKey, TContext> state, InternalTriggerInfo trigger)
        {
            PreviousState = state;
            TriggerInfo = trigger;         
            Location = "Trigger";
            Exception = null;
        }

        public void Stop()
        {
            NextState = null;
            TriggerInfo = null;
            AcceptMoves = false;
            IsAborted = false;
            TotalJumps = 0;
        }

        public void Jump()
        {
            TotalJumps++;
            NextState = null;
        }

        public void SetAborted()
        {
            if (IsAborted) return;
            IsAborted = true;
        }
    }
}
