using ElevatorSample;
using ElevatorSample.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TIM;
using TIM.Tracing;

namespace Elevator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IElevator Elevator { get; private set; }
        public MainWindow()
        {
            ElevatorViewModel viewmodel = new ElevatorViewModel(); 

            Machine<IElevator, ElevatorStates, ElevatorContext> machine = new Machine<IElevator, ElevatorStates, ElevatorContext>("Elevator",
                new ElevatorIdle(), new ElevatorMovingDown(), new ElevatorMovingUp(), new ElevatorClosingDoor(), new ElevatorDoorOpen(), new ElevatorOpeningDoor(), new ElevatorJammed());
            machine.Options.SetTraceHandler(Events_Trace);
            Elevator = machine.Run(ElevatorStates.Idle, new ElevatorContext());
            viewmodel.Init(Elevator);
            DataContext = viewmodel;

            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {

            base.OnInitialized(e);
            
        }

        public static void Events_Trace(TraceData report)
        {
            if (report.Event == TraceEvents.TriggerStart || report.Event == TraceEvents.TriggerEnd) 
            return;
            Debug.WriteLine(report.ToString());
        }
    }
}
