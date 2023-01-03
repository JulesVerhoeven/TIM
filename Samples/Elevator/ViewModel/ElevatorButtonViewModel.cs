using ElevatorSample.MVVM;

namespace ElevatorSample.ViewModel
{
    public class ElevatorButtonViewModel : ObservableBase
    {
        public Floor Floor { get; }
        public string Name { get; }
        public TriggerCommand ButtonCmd { get; private set; }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; ButtonCmd?.RaiseCanExecuteChanged(); }
        }

        public ElevatorButtonViewModel(Floor floor)
        {
            Floor = floor;
            Name = floor.ToString();
        }

        public void ButtonStateChanged(ButtonState state, Floor floor)
        {
            if (floor != Floor)
            {
                return;
            }

            IsEnabled = (state == ButtonState.Off);
        }

        public void Init(IElevator elevator)
        {
            ButtonCmd = new TriggerCommand(() => elevator.ElevatorButtonPressed(Floor), () => IsEnabled);
        }
    }
}
