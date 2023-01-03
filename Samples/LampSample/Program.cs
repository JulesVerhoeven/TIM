// See https://aka.ms/new-console-template for more information
using LampSample;
using System.Diagnostics;
using TIM;
using TIM.Tracing;

Console.WriteLine("Lamp sample");
Console.WriteLine("Possible commands:");
Console.WriteLine("\ton: Turns the lamp on.");
Console.WriteLine("\toff: Turns the lamp off.");
Console.WriteLine("\tblink: Starts blinking the lamp.");
Console.WriteLine("\tany number: Sets the blink delay in ms.");
Console.WriteLine("\texit: Halts the state machine and exites the program.");

Machine<ILampControl, LampStates, LampContext> machine = new ("Lamp",  new LampStateOn(), new LampStateOff());
machine.Options.SetTraceHandler((x) => Debug.WriteLine(x.ToString()));
ILampControl LampControl = machine.Run(LampStates.Off, new LampContext());
while (true)
{
    Console.Write("Command: ");
    string? command = Console.ReadLine();
    if (command != null)
    {
        command.Trim();
        switch(command)
        {
            case "on" :
                LampControl.TurnOn();
                break;
            case "off":
                LampControl.TurnOff();
                break;
            case "blink":
                LampControl.Blink();
                break;
            case "exit":
                machine.Halt();
                return;
            default: 
                if (int.TryParse(command, out var time))
                {
                    LampControl.BlinkDelayInMS = time;
                    break;
                }
                Console.WriteLine("Unknown command.");
                break;
        }
    }
}

