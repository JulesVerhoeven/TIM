using System;

namespace ElevatorSample
{
    public class ElevatorClosingDoor : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.DoorClosing;

        protected override void OnEntry(ElevatorStates from)
        {
            CallTimer("Elevator closing door", 1000, OnDoorTimer);
            base.OnEntry(from);
        }

        private void OnDoorTimer()
        {
            if (Context.CurrentDirection == Direction.Up)
            {
                Context.SetFloorUp(Context.CurrentFloor, ButtonState.Off);
                if (HigherButtonPressed(Context.CurrentFloor))
                {                  
                    GoTo(ElevatorStates.MovingUp);
                    return;
                }

                if (LowerButtonPressed(Context.CurrentFloor))
                {
                    Context.SetFloorDown(Context.CurrentFloor, ButtonState.Off);
                    GoTo(ElevatorStates.MovingDown);
                    return;
                }
            }

            if (Context.CurrentDirection == Direction.Down)
            {
                Context.SetFloorDown(Context.CurrentFloor, ButtonState.Off);
                if (LowerButtonPressed(Context.CurrentFloor))
                {                   
                    GoTo(ElevatorStates.MovingDown);
                    return;
                }

                if (HigherButtonPressed(Context.CurrentFloor))
                {
                    Context.SetFloorUp(Context.CurrentFloor, ButtonState.Off); 
                    GoTo(ElevatorStates.MovingUp);
                    return;
                }            
            }
            Context.SetFloorUp(Context.CurrentFloor, ButtonState.Off);
            Context.SetFloorDown(Context.CurrentFloor, ButtonState.Off);
            GoTo(ElevatorStates.Idle);
        }

        public override void ElevatorDoorOpenPressed()
        {
            GoTo(ElevatorStates.DoorOpening);
        }
        protected override void OnFloorDownButtonPressed(Floor floor)
        {
            if (floor == Context.CurrentFloor && Context.CurrentDirection == Direction.Down)
            {
                GoTo(ElevatorStates.DoorOpening);
            }
        }

        protected override void OnFloorUpButtonPressed(Floor floor)
        {          
            if (floor == Context.CurrentFloor && Context.CurrentDirection == Direction.Up)
            {
                GoTo(ElevatorStates.DoorOpening);
            }
        }


        protected override void OnExit(ElevatorStates to)
        {
            Context.SetElevatorButton(Context.CurrentFloor, ButtonState.Off);
        }
    }
}

