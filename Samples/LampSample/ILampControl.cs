namespace LampSample
{
    public interface ILampControl
    {
        int BlinkDelayInMS { get; set; }
        void TurnOff();
        void TurnOn();
        void Blink();
    }
}
