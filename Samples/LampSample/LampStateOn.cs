namespace LampSample
{
    public class LampStateOn : LampBaseState
    {
        public override LampStates Key => LampStates.On;

        protected override void OnEntry(LampStates from)
        {
            if (Context.IsBlinking) 
            {
                CallTimer("Timer Blink On", Context.BlinkDelayInMS, () => GoTo(LampStates.Off));
            }
        }
        public override void Blink()
        {
            Context.IsBlinking= true;
            GoTo(LampStates.Off);
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
