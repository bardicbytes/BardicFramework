﻿//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    public class BoolEventVarListener : GenericEventVarListener<bool>
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