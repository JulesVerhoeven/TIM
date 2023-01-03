using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSample
{
    public class ElevatorJammed : ElevatorBaseState
    {
        public override ElevatorStates Key => ElevatorStates.Jammed;

        protected override void OnEntry(ElevatorStates from)
        {
            Context.JammedReturnState = from;
            CallTimer("Waiting", 5000, OnWaitingTimer);
            base.OnEntry(from);
        }

        private void OnWaitingTimer()
        {
            GoTo(Context.JammedReturnState);
        }
    }
}
