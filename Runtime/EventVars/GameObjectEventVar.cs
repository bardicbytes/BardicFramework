//alex@bardicbytes.com
using UnityEngine;

namespace BB.BardicFramework.EventVars
{
    [CreateAssetMenu(menuName = Prefixes.EV + "GameObject")]
    public class GameObjectEventVar : GenericEventVar<GameObject>
    {
        public override GameObject To(EventVarInstanceField bc) => bc.UnityObjectValue as GameObject;
    }
}