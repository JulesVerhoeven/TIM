using ElevatorSample.MVVM;
using System.Collections.Generic;

namespace ElevatorSample.ViewModel
{

    public class ElevatorViewModel : ObservableBase
    {
        public List<FloorViewModel> Floors { get; }
        public ElevatorStateViewModel ElevatorState { get; }
        public List<ElevatorButtonViewModel> ElevatorButtons { get; }

        public TriggerCommand OpenCmd { get; private set; }
        public TriggerCommand CloseCmd { get; private set; }
        public TriggerCommand JamElevatorCmd { get; private set; }


        public ElevatorViewModel()
        {
            Floors = new List<FloorViewModel>();
            ElevatorButtons = new List<ElevatorButtonViewModel>();
            for (Floor floor = Floor.TopFloor; floor >= Floor.Basement; floor--)
            {
                Floors.Add(new FloorViewModel(floor));
                ElevatorButtons.Add(new ElevatorButtonViewModel(floor));
            }
        }


        public void Init(IElevator elevator)
        {
            OpenCmd = new TriggerCommand(() => elevator.ElevatorDoorOpenPressed(), () => elevator.CurrentState != ElevatorStates.MovingDown && elevator.CurrentState != ElevatorStates.MovingUp);
            CloseCmd = new TriggerCommand(() => elevator.ElevatorDoorOpenPressed(), () => elevator.CurrentState != ElevatorStates.MovingDown && elevator.CurrentState != ElevatorStates.MovingUp);
            JamElevatorCmd = new TriggerCommand(() => elevator.JamElevator(), () => elevator.CurrentState != ElevatorStates.Jammed);
            foreach (FloorViewModel floor in Floors)
            {
                floor.Init(elevator);
                floor.StateChanged(elevator.CurrentFloor, elevator.SubFloor, elevator.CurrentState);
            }

            foreach (ElevatorButtonViewModel button in ElevatorButtons)
            {
                button.Init(elevator);
                button.ButtonStateChanged(elevator.ElevatorButtonState(button.Floor), button.Floor);
            }

            elevator.FloorDownButtonChanged += Context_FloorDownButtonChanged;
            elevator.FloorUpButtonChanged += Context_FloorUpButtonChanged;
            elevator.ElevatorStateChanged += Context_ElevatorStateChanged;
            elevator.ElevatorButtonChanged += Context_ElevatorButtonChanged;
        }

        private void Context_ElevatorButtonChanged(Floor floor, ButtonState state)
        {
            foreach (var button in ElevatorButtons)
            {
                button.ButtonStateChanged(state, floor);
            }
        }

        private void Context_ElevatorStateChanged(ElevatorStates state, Floor floor, SubFloor sub, Direction direction)
        {
            foreach(var floorModel in Floors)
            {
                floorModel.StateChanged(floor, sub, state);
            }

            OpenCmd.RaiseCanExecuteChanged();
            CloseCmd.RaiseCanExecuteChanged();
        }

        private void Context_FloorUpButtonChanged(Floor floor, ButtonState state)
        {
            Floors[Floor.TopFloor - floor].UpEnabled = (state == ButtonState.Off);
        }

        private void Context_FloorDownButtonChanged(Floor floor, ButtonState state)
        {
            Floors[Floor.TopFloor - floor].DownEnabled = (state == ButtonState.Off);
        }
    }
}
