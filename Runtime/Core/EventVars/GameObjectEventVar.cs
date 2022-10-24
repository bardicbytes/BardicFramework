//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    [CreateAssetMenu(menuName = Core.Prefixes.EV + "GameObject")]
    public class GameObjectEventVar : GenericEventVar<GameObject>
    {
        public override GameObject To(EventVarInstanceField bc) => bc.UnityObjectValue as GameObject;
    }
}