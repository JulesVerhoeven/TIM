using System;
using TIM.Tracing;

namespace TIM._Internal
{
    internal class InternalMachineOptions<TKey, TContext> : IMachineOptions<TKey, TContext>
    {
        public InternalMachineOptions(object lockObject = null)
        {
            LockObject = lockObject;
            if (LockObject == null)
            {
                LockObject = this;
            }
        }

        private int __MaxNumberOfStateChanges = 64;
        public int MaxNumberOfStateChanges
        {
            get { return __MaxNumberOfStateChanges; }
            set { lock (LockObject) { __MaxNumberOfStateChanges = value; } }
        }

        public object LockObject { get; private set; }

        private Action<TraceData> __TraceHandler = null;
        internal Action<TraceData> TraceHandler
        {
            get { return __TraceHandler; }
            set { lock (LockObject) { __TraceHandler = value; } }
        }
        private TraceEvents _EventsToTrace = TraceEvents.All;
        internal TraceEvents EventsToTrace
        {
            get { return _EventsToTrace; }
            set { lock (LockObject) { _EventsToTrace = value; } }
        }

        private Action<TKey> __OnAccepted = null;
        internal Action<TKey> OnAccepted
        {
            get { return __OnAccepted; }
            set
            {
                lock (LockObject)
                {                  
                    __OnAccepted = value;
                }
            }
        }

        private Action<TKey, string> __TriggerBegin = null;
        internal Action<TKey, string> TriggerBegin
        {
            get { return __TriggerBegin; }
            set
            {
                lock (LockObject)
                {
                    __TriggerBegin = value;
                }
            }
        }

        public void SetTriggerBeginHandler(Action<TKey, string> handler)
        {
            TriggerBegin = handler;
        }

        private Action<TKey, bool, bool> __TriggerEnd = null;
        internal Action<TKey, bool, bool> TriggerEnd
        {
            get { return __TriggerEnd; }
            set
            {
                lock (LockObject)
                {
                    __TriggerEnd = value;
                }
            }
        }

        public void SetTriggerEndHandler(Action<TKey, bool, bool> handler)
        {
            TriggerEnd = handler;
        }

        private Action<TKey, TContext> __MachineStart = null;
        internal Action<TKey, TContext> MachineStart
        {
            get { return __MachineStart; }
            set
            {
                lock (LockObject)
                {
                    __MachineStart = value;
                }
            }
        }

        public void SetMachineStartHandler(Action<TKey, TContext> handler)
        {
            MachineStart = handler;
        }

        private Action<TKey, TContext> __MachineStop = null;
        internal Action<TKey, TContext> MachineStop
        {
            get { return __MachineStop; }
            set
            {
                lock (LockObject)
                {
                    __MachineStop = value;
                }
            }
        }

        public void SetMachineStopHandler(Action<TKey, TContext> handler)
        {
            MachineStop = handler;
        }
    }
}
