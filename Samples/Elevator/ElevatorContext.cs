using ElevatorSample.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElevatorSample
{
    public enum SubFloor
    {
        Above,
        Center,
        Below
    }

    public delegate void FloorUpButtonChangedDelegate(Floor floor, ButtonState state);
    public delegate void FloorDownButtonChangedDelegate(Floor floor, ButtonState state);
    public delegate void ElevatorButtonChangedDelegate(Floor floor, ButtonState state);
    public delegate void ElevatorStateChangedDelegate(ElevatorStates state, Floor floor, SubFloor sub, Direction direction);

    public class ElevatorContext 
    {
        public event FloorUpButtonChangedDelegate FloorUpButtonChanged;
        public event FloorDownButtonChangedDelegate FloorDownButtonChanged;
        public event ElevatorStateChangedDelegate ElevatorStateChanged;
        public event ElevatorButtonChangedDelegate ElevatorButtonChanged;

        public ElevatorStates CurrentState { get; private set; }
        public Direction CurrentDirection { get; private set; }
        public Floor CurrentFloor { get; private set; }
        public SubFloor SubFloor { get; private set; }
        public ElevatorStates JammedReturnState { get; internal set; }


        public ButtonState Alarm = ButtonState.Off;

        public ButtonState FloorUpState(Floor floor)
        {
            return FloorUpRequests[(int)floor];
        }

        public ButtonState FloorDownState(Floor floor)
        {
            return FloorDownRequests[(int)floor];
        }

        public ButtonState ElevatorButtonState(Floor floor)
        {
            return ElevatorRequests[(int)floor];
        }

        public void SetFloorUp(Floor floor, ButtonState state)
        {
            if (FloorUpRequests[(int)floor] == state) return;
            FloorUpRequests[(int)floor] = state;
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FloorUpButtonChanged?.Invoke(floor, state);
            });
        }

        public void SetFloorDown(Floor floor, ButtonState state)
        {
            if (FloorDownRequests[(int)floor] == state) return;
            FloorDownRequests[(int)floor] = state;
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FloorDownButtonChanged?.Invoke(floor, state);
            });
        }

        public void SetElevatorButton(Floor floor, ButtonState state)
        {
            if (ElevatorRequests[(int)floor] == state) return;
            ElevatorRequests[(int)floor] = state;
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ElevatorButtonChanged?.Invoke(floor, state);
            });
        }

        public void SetElevatorState(ElevatorStates state)
        {
            if (CurrentState == state) return;
            CurrentState = state;
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ElevatorStateChanged?.Invoke(CurrentState, CurrentFloor, SubFloor, CurrentDirection);
            });
        }

        public void SetElevatorFloor(Floor floor, SubFloor sub)
        {
            if (CurrentFloor == floor && SubFloor == sub) return;
            if (floor < Floor.Basement || floor > Floor.TopFloor)
                throw new Exception("Should not get here");
            CurrentFloor = floor;
            SubFloor = sub;
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ElevatorStateChanged?.Invoke(CurrentState, CurrentFloor, SubFloor, CurrentDirection);
            });
        }

        public void SetElevatorDirection(Direction direction)
        {
            if (CurrentDirection == direction) return;
            CurrentDirection = direction;
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ElevatorStateChanged?.Invoke(CurrentState, CurrentFloor, SubFloor, CurrentDirection);
            });
        }



        private ButtonState[] FloorUpRequests { get; } = new ButtonState[(int)Floor.TopFloor + 1];
        private ButtonState[] FloorDownRequests { get; } = new ButtonState[(int)Floor.TopFloor + 1];
        private ButtonState[] ElevatorRequests { get; } = new ButtonState[(int)Floor.TopFloor + 1];
    }
}
