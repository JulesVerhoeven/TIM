using TIM;

namespace LampSample
{
    public abstract class LampBaseState : State<LampStates, LampContext>, ILampControl
    {
        public int BlinkDelayInMS
        {
            get => Context.BlinkDelayInMS;
            set
            {
                Context.BlinkDelayInMS = value;
            }
        }

        public abstract void Blink();
        public abstract void TurnOff();
        public abstract void TurnOn();
    }
}
