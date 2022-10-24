//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    public class GameObjectEventVarListener : GenericBaseEventVarListener<GameObjectEventVar>
    {
        protected override void HandleTypedEventRaised(GameObjectEventVar data) => base.HandleTypedEventRaised(data);
    }
}