//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    public class StringEventVarListener : GenericBaseEventVarListener<string>
    {
        protected override void HandleTypedEventRaised(string data)
        {
            base.HandleTypedEventRaised(data);
        }
    }
}