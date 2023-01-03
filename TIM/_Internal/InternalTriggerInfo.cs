using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIM._Internal
{
    internal class InternalTriggerInfo
    {
        public Type Interface = null;
        public string Trigger = null;
        public string[] Params = null;

        public InternalTriggerInfo(Type interfce, string trigger, string[] parameters = null)
        {
            Interface = interfce;
            Trigger = trigger;
            Params = parameters;
        }
        public override string ToString()
        {
            string prefix = Interface == null ? "" : Interface.ToString() + ".";
            if (Params == null || Params.Length == 0)
            {
                return prefix + Trigger.ToString() + "()";
            }
            return prefix + Trigger.ToString() + "(" + string.Join(", ", Params) + ")";
        }
    }
}
