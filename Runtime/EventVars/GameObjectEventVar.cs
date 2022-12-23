//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    [CreateAssetMenu(menuName = Prefixes.EV + "GameObject")]
    public class GameObjectEventVar : SimpleGenericEventVar<GameObject>
    {
        public override GameObject To(EventVars.EVInstData bc) => bc.UnityObjectValue as GameObject;

        protected override void SetInitialvalueOfInstanceConfig(GameObject val, EventVars.EVInstData config) => config.UnityObjectValue = val;
    }
}