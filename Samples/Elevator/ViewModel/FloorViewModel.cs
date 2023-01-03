using ElevatorSample.MVVM;
using System.Windows;

namespace ElevatorSample.ViewModel
{
    public class FloorViewModel : ObservableBase
    {
        public Floor Floor { get; }

        private string _State = "";
        public string State
        {
            get { return _State; }
            private set { SetProperty(ref _State, value); }
        }

        private VerticalAlignment _Alignment = VerticalAlignment.Center;
        public VerticalAlignment Alignment
        {
            get { return _Alignment; }
            private set { SetProperty(ref _Alignment, value); }
        }

        public TriggerCommand FloorUpCmd { get; private set; }
        public TriggerCommand FloorDownCmd { get; private set; }

        private bool _UpEnabled = true;
        public bool UpEnabled
        {
            get { return _UpEnabled; }
            set { _UpEnabled = value; FloorUpCmd?.RaiseCanExecuteChanged(); }
        }

        private bool _DownEnabled= true;
        public bool DownEnabled
        {
            get { return _DownEnabled; }
            set { _DownEnabled = value; FloorDownCmd?.RaiseCanExecuteChanged(); }
        }

        public Visibility IsUpVissible => Floor == Floor.TopFloor ? Visibility.Collapsed : Visibility.Visible;
        public Visibility IsDownVissible => Floor == Floor.Basement ? Visibility.Collapsed : Visibility.Visible;


        public FloorViewModel(Floor floor)
        {
            Floor = floor;
        }

        public void StateChanged(Floor floor, SubFloor sub, ElevatorStates state)
        {
            if (floor != Floor)
            {
                State = "";
                Alignment = VerticalAlignment.Center;
                return;
            }

            switch (sub)
            {
                case SubFloor.Above:
                    Alignment = VerticalAlignment.Top;
                    break;
                case SubFloor.Center:
                    Alignment = VerticalAlignment.Center;
                    break;
                case SubFloor.Below:
                    Alignment = VerticalAlignment.Bottom;
                    break;
            }

            switch (state)
            {
                case ElevatorStates.Idle:
                    State = "Waiting";
                    break;
                case ElevatorStates.MovingUp:
                    State = "Moving";
                    break;
                case ElevatorStates.MovingDown:
                    State = "Moving";
                    break;
                case ElevatorStates.DoorOpening:
                    State = "Opening";
                    break;
                case ElevatorStates.DoorClosing:
                    State = "Closing";
                    break;
                case ElevatorStates.DoorOpen:
                    State = "Open";
                    break;
                case ElevatorStates.Jammed:
                    State = "Jammed";
                    break;
            }
        }

        public void Init(IElevator elevator)
        {
            FloorUpCmd = new TriggerCommand(() => elevator.FloorButtonUpPressed(Floor), () => UpEnabled);
            FloorDownCmd = new TriggerCommand(() => elevator.FloorButtonDownPressed(Floor), () => DownEnabled);
        }
    }
}
