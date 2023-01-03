using System.Collections.Generic;
using System.ComponentModel;

namespace ElevatorSample
{
    public interface IElevator
    {
        event ElevatorButtonChangedDelegate ElevatorButtonChanged;
        event ElevatorStateChangedDelegate ElevatorStateChanged;
        event FloorDownButtonChangedDelegate FloorDownButtonChanged;
        event FloorUpButtonChangedDelegate FloorUpButtonChanged;

        Direction CurrentDirection { get; }
        Floor CurrentFloor { get; }
        ElevatorStates CurrentState { get; }
        SubFloor SubFloor { get; }

        ButtonState ElevatorButtonState(Floor floor);
        ButtonState FloorDownState(Floor floor);
        ButtonState FloorUpState(Floor floor);

        void FloorButtonUpPressed(Floor floor);
        void FloorButtonDownPressed(Floor floor);
        void ElevatorButtonPressed(Floor floor);
        void ElevatorDoorClosePressed();
        void ElevatorDoorOpenPressed();
        void JamElevator();
    }
}
