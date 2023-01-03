using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TIM;

namespace ElevatorSample
{
    public abstract class ElevatorBaseState : State<ElevatorStates, ElevatorContext>, IElevator
    {
        public int ElevatorSpeed { get; } = 1000;

        public Direction CurrentDirection => Context.CurrentDirection;

        public Floor CurrentFloor => Context.CurrentFloor;

        public ElevatorStates CurrentState => Context.CurrentState;

        public SubFloor SubFloor => Context.SubFloor;

        public event ElevatorButtonChangedDelegate ElevatorButtonChanged
        {
            add { Context.ElevatorButtonChanged += value; }
            remove { Context.ElevatorButtonChanged -= value; }
        }

        public event ElevatorStateChangedDelegate ElevatorStateChanged
        {
            add { Context.ElevatorStateChanged += value; }
            remove { Context.ElevatorStateChanged -= value; }
        }
        public event FloorDownButtonChangedDelegate FloorDownButtonChanged
        {
            add { Context.FloorDownButtonChanged += value; }
            remove { Context.FloorDownButtonChanged -= value; }
        }
        public event FloorUpButtonChangedDelegate FloorUpButtonChanged
        {
            add { Context.FloorUpButtonChanged += value; }
            remove { Context.FloorUpButtonChanged -= value; }
        }

        protected override void OnEntry(ElevatorStates from)
        {
            Context.SetElevatorState(Key);
            base.OnEntry(from);
        }

        public void ElevatorAlarmPressed() => Context.Alarm = ButtonState.On;
        public void JamElevator() => GoTo(ElevatorStates.Jammed);
        public void ElevatorButtonPressed(Floor floor)
        {
            Context.SetElevatorButton(floor, ButtonState.On);
            OnElevatorButtonPressed(floor);
        }
        public virtual void ElevatorDoorClosePressed() { }
        public virtual void ElevatorDoorOpenPressed() { }
        public void FloorButtonUpPressed(Floor floor)
        {           
            if (floor == Floor.TopFloor) return;
            Context.SetFloorUp(floor,ButtonState.On);
            
            OnFloorUpButtonPressed(floor);
        }

        public void FloorButtonDownPressed(Floor floor)
        {           
            if (floor == Floor.Basement) return;
            Context.SetFloorDown(floor, ButtonState.On);

            OnFloorDownButtonPressed(floor);
        }
        protected virtual void OnElevatorButtonPressed(Floor floor) { }
        protected virtual void OnFloorUpButtonPressed(Floor floor) { }
        protected virtual void OnFloorDownButtonPressed(Floor floor) { }

        protected bool HigherButtonPressed(Floor currentFloor)
        {
            for (int i = ((int)currentFloor) + 1; i <= (int)Floor.TopFloor; i++)
            {
                if (ButtonPressed((Floor)i)) return true;
            }
            return false;
        }

        protected bool LowerButtonPressed(Floor currentFloor)
        {
            for (int i = ((int)Floor.Basement); i < (int)currentFloor; i++)
            {
                if (ButtonPressed((Floor)i)) return true;
            }
            return false;
        }

        protected bool ButtonPressed(Floor floor)
        {
            if (Context.ElevatorButtonState(floor) == ButtonState.On) return true;
            if (Context.FloorUpState(floor) == ButtonState.On) return true;
            if (Context.FloorDownState(floor) == ButtonState.On) return true;
            return false;
        }

        public ButtonState ElevatorButtonState(Floor floor)
        {
            return Context.ElevatorButtonState(floor);
        }

        public ButtonState FloorDownState(Floor floor)
        {
            return Context.FloorDownState(floor);
        }

        public ButtonState FloorUpState(Floor floor)
        {
            return Context.FloorUpState(floor);
        }
    }
}
