
<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links

[![Contributors][contributors-shield]][contributors-url]-->
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  

<h3 align="center">TIM</h3>

  <p align="center">
    The Interface Machine
    <br />
    <a href="https://github.com/JulesVerhoeven/TIM"><strong>Explore the docs »</strong></a>
    <br />
    <br />    
    <a href="https://github.com/JulesVerhoeven/TIM/issues">Report Bug</a>
    ·
    <a href="https://github.com/JulesVerhoeven/TIM/issues">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <!-- <li><a href="#roadmap">Roadmap</a></li> -->
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

TIM is a C# library for state machine implementations that are triggered through a (proxy-)interface. 
This hides all state machine mechanics for the outside world, you just talk with an ordinary interface. 
This approach also opens the way to use ordinary classes (with interfaces) as states within the state machine.
The library is thread-safe and it further supports all the normal state machine stuff like entry/exit etc.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

[![Next][Next.js]][Next-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started


### Prerequisites

The project only depends on .net core 6.0.


### Installation

* NuGet 
  ```sh
  NuGet\Install-Package TIM -Version 2.0.0
  ```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

A statemachine consists of four parts; a trigger interface, a context, the machine and the states. 
The trigger interface is a normal interface that must be implemented on every state. 
The context is used to store the data needed in the states.
The machine represents the overall functionality of the state machine. The states do the actual implementation of the state machine.
Lets have a look at a simple example: a lamp.
#### Example
A lamp has two states On and Off. So we will create an enum that represents these states:
```cs
public enum LampStates
{
   Off,
   On
}
```
Next, we define the trigger interface. A lamp can be turned on and off and, to make this example a bit more interesting, it can also blink.
Here is the interface:
```cs
public interface ILampControl
{
    int BlinkDelayInMS { get; set; }
    void Blink();
    void TurnOff();
    void TurnOn();
}
```
Now we can also define the context. The states need to remember if they are in blinking mode and what is the blink delay. So here is the context:
```cs
public class LampContext
{
    public int BlinkDelayInMS { get; set; } = 500;
    public bool IsBlinking { get; set; }
}
```
In general, you will create a base state class that handles the properties of the interface. 
From the base state you will then derive the individual states. 
Every state must be derived from 'State<TKey, TContext>' and it must implement the trigger interface.
In our case TKey is 'LampStates' and TContext is 'LampContext'. So here is the base state:
```cs
public abstract class LampBaseState : State<LampStates, LampContext>, ILampControl
{
    public int BlinkDelayInMS 
    {   
        get => Context.BlinkDelayInMS;
        set
        {
            Context.BlinkDelayInMS = value;
        }
    }
    public abstract void Blink();
    public abstract void TurnOff();
    public abstract void TurnOn();
}
```
Notice that the blink delay is not stored within the state but in the context.
In general you should never store any data within a state. 
With the base state we can now define the states themselves.
```cs
public class LampStateOff : LampBaseState
{
    public override LampStates Key => LampStates.Off;

    protected override void OnEntry(LampStates from)
    {
        if (Context.IsBlinking)
        {
            CallTimer("Timer Blink Off", Context.BlinkDelayInMS, () => GoTo(LampStates.On));
        }
    }
    public override void Blink()
    {
        Context.IsBlinking = true;
        GoTo(LampStates.On);
    }
    public override void TurnOff()
    {
        Context.IsBlinking = false;
        GoTo(LampStates.Off);
    }
    public override void TurnOn()
    {
        Context.IsBlinking = false;
        GoTo(LampStates.On);
    }
}
```
and
```cs
public class LampStateOn : LampBaseState
{
    public override LampStates Key => LampStates.On;

    protected override void OnEntry(LampStates from)
    {
        if (Context.IsBlinking) 
        {
            CallTimer("Timer Blink On", Context.BlinkDelayInMS, () => GoTo(LampStates.Off));
        }
    }
    public override void Blink()
    {
        Context.IsBlinking= true;
        GoTo(LampStates.Off);
    }
    public override void TurnOff()
    {
        Context.IsBlinking = false;
        GoTo(LampStates.Off);
    }
    public override void TurnOn()
    {
        Context.IsBlinking = false;
        GoTo(LampStates.On);
    }   
}
```
There are several things to notice here:
* The Key property: every state has a unique key that is used in the GoTo(...) method.
* The OnEntry() override: The state machine always calls OnEntry before entering a new state. 
  This is the place where you can perform initialization of the state.
  In this case we use it to start a timer for the blinking.
* The GoTo(..) method is used to jump to a state. If GoTo has been called, 
  the state machine will exit the current state and enter the new state **after** the trigger has finished. 
  Upon the exit of a state, any running calls are cancelled.
  This happens also if the destination is the same as the current state. 
  In this sample this is used to cancel the blink timer started in the entry.
* The CallTimer(...) method: You should not await any task inside a trigger, instead use the Call() methods to do any asyn operation.
  Because the timer is a very common operation, it has a special call which is used here.

Now we have defined all elements of the statemachine, we can create and start the sate machine itself:
```cs
using TIM.Tracing;
...
Machine<ILampControl, LampStates, LampContext> machine = new ("Lamp",  new LampStateOn(), new LampStateOff());
machine.Options.SetTraceHandler((x) => Debug.WriteLine(x.ToString()));
ILampControl LampControl = machine.Run(LampStates.Off, new LampContext());
```
By added the using 'TIM.Tracing'you can activate tracing on the state machine by setting a trace handler.
In the Machine constructor you give the machine a name, which is used in the traces, and you specify the states of this state machine.
By calling Run() you start the statemachine in the specified state with the given context.

_For the code of this example and more, please refer to the [Samples](https://github.com/JulesVerhoeven/TIM/tree/main/Samples)_

#### Implementation guidelines
* The state machine should _always_ be response. 
  This means that you should not perform any lengthy calculations, waits or sleeps within the trigger implementations. Instead, create a task and use Call(...) to handle it.
* Do not access the context outside of the states. TIM makes sure that all calls to the states are synchronized. 
  If you acces the context from outside the statemachine you are likely to get concurrency issues.
* Take care that all callbacks, actions, functions, events, invokes etc. are executed asynchronously from the state machine. 
  If the trigger interface gets called synchronously from inside any of these you will either get an exception or a deadlock! 
  For an example of an event implementation, see the Elevator sample.
* Do not use asyn/await in the trigger implementation, this will lead to some unexpected results. Instead, use the Call() method.
* Normally, you would define an enum as key for the states. But, if you expect that the statemachine will get extended elsewhere, use strings.
  You can define the strings used in the statemachine as constants.
* TIM does not explicitely support sub-statemachines, but you can easily create them by storing the keys of the state(s) you want to return to in the context.
  You then return to this state from inside a trigger with a GoTo() to key in the context. 
  There is an example of this in the Elevator sample. (When Jamming the elevator.)


<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP 
## Roadmap

- [ ] Feature 1
- [ ] Feature 2
- [ ] Feature 3
    - [ ] Nested Feature

See the [open issues](https://github.com/JulesVerhoeven/TIM/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>
-->


<!-- CONTRIBUTING -->
## Contributing

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Jules Verhoeven - j-mail@kpnmail.nl.com

Project Link: [https://github.com/JulesVerhoeven/TIM](https://github.com/JulesVerhoeven/TIM)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS
## Acknowledgments

* []()
* []()
* []()

<p align="right">(<a href="#readme-top">back to top</a>)</p>
 -->


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/JulesVerhoeven/TIM.svg?style=for-the-badge
[contributors-url]: https://github.com/JulesVerhoeven/TIM/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/JulesVerhoeven/TIM.svg?style=for-the-badge
[forks-url]: https://github.com/JulesVerhoeven/TIM/network/members
[stars-shield]: https://img.shields.io/github/stars/JulesVerhoeven/TIM.svg?style=for-the-badge
[stars-url]: https://github.com/JulesVerhoeven/TIM/stargazers
[issues-shield]: https://img.shields.io/github/issues/JulesVerhoeven/TIM.svg?style=for-the-badge
[issues-url]: https://github.com/JulesVerhoeven/TIM/issues
[license-shield]: https://img.shields.io/github/license/JulesVerhoeven/TIM.svg?style=for-the-badge
[license-url]: https://github.com/JulesVerhoeven/TIM/blob/master/LICENSE.txt

[Next.js]: https://img.shields.io/badge/VisualStudio-000000?style=for-the-badge&logo=nextdotjs&logoColor=white
[Next-url]: https://visualstudio.microsoft.com/