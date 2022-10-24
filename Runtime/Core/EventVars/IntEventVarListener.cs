//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    public class IntEventVarListener : GenericBaseEventVarListener<int>
    {
        protected override void HandleTypedEventRaised(int data) => base.HandleTypedEventRaised(data);
    }
}