using System;
using System.Runtime.CompilerServices;

namespace ElevatorSample.MVVM
{
    public interface IOnNotifyPropertyChanged
    {
        void RaisePropertyChanged([CallerMemberName] String propertyName = "");
    }
}
