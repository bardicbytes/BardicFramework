//alex@bardicbytes.com

using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    public class GameObjectEventVarListener : GenericEventVarListener<GameObject>
    {
        protected override void HandleTypedEventRaised(GameObject data) => base.HandleTypedEventRaised(data);
    }
}