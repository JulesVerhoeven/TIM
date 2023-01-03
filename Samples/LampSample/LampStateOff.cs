namespace LampSample
{
    public class LampStateOff : LampBaseState
    {
        public override LampStates Key => LampStates.Off;

        protected override void OnEntry(LampStates from)
        {
            if (Context.IsBlinking)
            {
                CallTimer("Timer Blink Off", Context.BlinkDelayInMS, () => GoTo(LampStates.On));
            }
        }
        public override void Blink()
        {
            Context.IsBlinking = true;
            GoTo(LampStates.On);
        }
        public override void TurnOff()
        {
            Context.IsBlinking = false;
            GoTo(LampStates.Off);
        }
        public override void TurnOn()
        {
            Context.IsBlinking = false;
            GoTo(LampStates.On);
        }
    }
}
