using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ElevatorSample.MVVM
{
    public abstract class ObservableBase : INotifyPropertyChanged, IOnNotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            DispatcherResolver.RunInDispatcher(() =>
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            });
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;
            field = value;

            RaisePropertyChanged(name);
        }
    }
}
