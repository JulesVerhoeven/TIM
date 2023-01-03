namespace ElevatorSample
{
   
    public class ElevatorIdle : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.Idle;

        protected override void OnStart()
        {
            Context.SetElevatorFloor(Floor.Basement, SubFloor.Center);
        }

        protected override void OnEntry(ElevatorStates from)
        {
            if (Context.CurrentFloor != Floor.Basement)
            {
                CallTimer("Waiting", 5000, OnWaitingTimer);
            }
            base.OnEntry(from);
        }

        private void OnWaitingTimer()
        {
            if (Context.CurrentFloor != Floor.Basement)
            {
                GoTo(ElevatorStates.MovingDown);
            }
        }

        public override void ElevatorDoorOpenPressed()
        {
            GoTo(ElevatorStates.DoorOpening);
        }

        protected override void OnFloorUpButtonPressed(Floor floor)
        {
            if (floor == Context.CurrentFloor)
            {
                GoTo(ElevatorStates.DoorOpening);
                return;
            }

            if (HigherButtonPressed(Context.CurrentFloor))
            {
                Context.SetFloorUp(Context.CurrentFloor, ButtonState.Off); 
                GoTo(ElevatorStates.MovingUp);
                return;
            }
        }

        protected override void OnFloorDownButtonPressed(Floor floor)
        {
            if (floor == Context.CurrentFloor)
            {
                return;
            }

            if (HigherButtonPressed(Context.CurrentFloor))
            {
                Context.SetFloorUp(Context.CurrentFloor, ButtonState.Off);
                GoTo(ElevatorStates.MovingUp);
                return;
            }
        }

        protected override void OnElevatorButtonPressed(Floor floor)
        {
            if (floor == Context.CurrentFloor)
            {
                return;
            }
            GoTo(ElevatorStates.MovingUp);
        }


        protected override void OnExit(ElevatorStates to)
        {
            Context.SetElevatorButton(Context.CurrentFloor, ButtonState.Off);
        }
    }
}

