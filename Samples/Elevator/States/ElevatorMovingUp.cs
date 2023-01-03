namespace ElevatorSample
{
    public class ElevatorMovingUp : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.MovingUp;// "Elevator moving up";

        protected override void OnEntry(ElevatorStates from)
        {
            Context.SetElevatorDirection(Direction.Up);
            CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
            base.OnEntry(from);
        }

        private void OnMovingTimer()
        {
            switch (Context.SubFloor)
            {
                case SubFloor.Above:
                    Context.SetElevatorFloor(Context.CurrentFloor + 1, SubFloor.Below);
                    CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                    break;
                case SubFloor.Center:
                    if (Context.CurrentFloor == Floor.TopFloor)
                    {
                        if (LowerButtonPressed(Context.CurrentFloor))
                        {
                            GoTo(ElevatorStates.MovingDown);
                            return;
                        }
                        GoTo(ElevatorStates.Idle);
                        return;
                    }
                    Context.SetElevatorFloor(Context.CurrentFloor, SubFloor.Above);
                    CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                    break;
                case SubFloor.Below:
                    Context.SetElevatorFloor(Context.CurrentFloor, SubFloor.Center);
                    if (Context.FloorUpState(Context.CurrentFloor) == ButtonState.On ||
                        Context.ElevatorButtonState(Context.CurrentFloor) == ButtonState.On)
                    {
                        GoTo(ElevatorStates.DoorOpening);
                        return;
                    }

                    if (HigherButtonPressed(Context.CurrentFloor))
                    {
                        CallTimer("Elevator Moving", ElevatorSpeed, OnMovingTimer);
                        return;
                    }

                    if (Context.FloorDownState(Context.CurrentFloor) == ButtonState.On)
                    {
                        GoTo(ElevatorStates.DoorOpening);
                        return;
                    }

                    GoTo(ElevatorStates.MovingDown);
                    break;
            }
        }
    }
}

