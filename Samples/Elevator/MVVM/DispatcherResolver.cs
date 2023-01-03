using System;
using System.Windows;
using System.Windows.Threading;

namespace ElevatorSample.MVVM
{
    public static class DispatcherResolver
    {
        public static Dispatcher Dispatcher
        {
            get
            {
                return Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher;
            }
        }

        public static void RunInDispatcher(Action run, bool invokeAlways = false)
        {
            Dispatcher disp = Dispatcher;
            if (disp.CheckAccess() && !invokeAlways)
            {
                run();
                return;
            }
            disp.InvokeAsync(run);
        }
    }
}

