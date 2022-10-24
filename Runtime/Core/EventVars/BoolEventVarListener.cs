//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    public class BoolEventVarListener : GenericBaseEventVarListener<bool>
    {
        [SerializeField]
        private bool invertValueForResponse = false;

        protected override void HandleTypedEventRaised(bool data)
        {
            if (invertValueForResponse) base.HandleTypedEventRaised(!data);
            else base.HandleTypedEventRaised(data);
        }
    }
}