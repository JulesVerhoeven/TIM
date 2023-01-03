namespace ElevatorSample
{
    public class ElevatorMovingDown : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.MovingDown;

        protected override void OnEntry(ElevatorStates from)
        {
            Context.SetElevatorDirection(Direction.Down);
            CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
            base.OnEntry(from);
        }

        protected override void OnFloorUpButtonPressed(Floor floor)
        {
            if (LowerButtonPressed(Context.CurrentFloor) || ButtonPressed(Context.CurrentFloor)) return;
            GoTo(ElevatorStates.MovingUp);
        }

        private void OnMovingTimer()
        {
            switch (Context.SubFloor)
            {
                case SubFloor.Above:
                    Context.SetElevatorFloor(Context.CurrentFloor, SubFloor.Center);
                    if (Context.FloorDownState(Context.CurrentFloor) == ButtonState.On ||
                        Context.ElevatorButtonState(Context.CurrentFloor) == ButtonState.On)
                    {
                        GoTo(ElevatorStates.DoorOpening);
                        return;
                    }

                    if (LowerButtonPressed(Context.CurrentFloor))
                    {
                        CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                        return;
                    }

                    if (Context.FloorUpState(Context.CurrentFloor) == ButtonState.On)
                    {
                        GoTo(ElevatorStates.DoorOpening);
                        return;
                    }

                    if (HigherButtonPressed(Context.CurrentFloor))
                    {
                        GoTo(ElevatorStates.MovingUp);
                        return;
                    }

                    if (Context.CurrentFloor == Floor.Basement)
                    {
                        GoTo(ElevatorStates.Idle);
                        return;
                    }
                    CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                    break;
                case SubFloor.Center:
                    if (Context.CurrentFloor == Floor.Basement)
                    {
                        if (HigherButtonPressed(Context.CurrentFloor))
                        {
                            GoTo(ElevatorStates.MovingUp);
                            return;
                        }
                        GoTo(ElevatorStates.Idle);
                        return;
                    }
                    Context.SetElevatorFloor(Context.CurrentFloor, SubFloor.Below);
                    CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                    break;
                case SubFloor.Below:
                    
                    Context.SetElevatorFloor(Context.CurrentFloor - 1, SubFloor.Above);
                    CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                    break;
            }
           
        }
    }
}

