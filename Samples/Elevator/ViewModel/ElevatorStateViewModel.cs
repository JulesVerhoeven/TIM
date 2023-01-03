using ElevatorSample.MVVM;
using System.Collections.Generic;

namespace ElevatorSample.ViewModel
{
    public class ElevatorStateViewModel : ObservableBase
    {
        public ElevatorStateViewModel(ElevatorStates state, Floor floor, bool[] floorButtonActive)
        {
            State = state;
            CurrentFloor = floor;
            FloorButtonActive = new List<bool>(floorButtonActive);
        }

        public ElevatorStates State { get; }
        public Floor CurrentFloor { get; }
        public Direction Direction { get; }
        public List<bool> FloorButtonActive { get; }
    }
}
