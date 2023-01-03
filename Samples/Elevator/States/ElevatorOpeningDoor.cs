namespace ElevatorSample
{
    public class ElevatorOpeningDoor : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.DoorOpening; 

        protected override void OnEntry(ElevatorStates from)
        {
            CallTimer("Elevator opening door", 1000, OnDoorTimer);
            base.OnEntry(from);
        }

        private void OnDoorTimer()
        {
            GoTo(ElevatorStates.DoorOpen);
        }

        public override void ElevatorDoorClosePressed()
        {
            GoTo(ElevatorStates.DoorClosing);
        }
    }

    public class ElevatorDoorOpen : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.DoorOpen;

        protected override void OnEntry(ElevatorStates from)
        {
            Context.SetElevatorButton(Context.CurrentFloor, ButtonState.Off);
            CallTimer("door open", 2000, OnDoorTimer);
            base.OnEntry(from);
        }

        private void OnDoorTimer()
        {
            GoTo(ElevatorStates.DoorClosing);
        }

        public override void ElevatorDoorClosePressed()
        {
            GoTo(ElevatorStates.DoorClosing);
        }

        public override void ElevatorDoorOpenPressed()
        {
            GoTo(ElevatorStates.DoorOpen);
        }
    }
}

