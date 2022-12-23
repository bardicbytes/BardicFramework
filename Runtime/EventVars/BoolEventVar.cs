//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    [CreateAssetMenu(menuName = Prefixes.EV+"Bool")]

    public class BoolEventVar : SimpleGenericEventVar<bool>
    {
        public void Toggle() => Raise(!Value);
        public override bool To(EventVars.EVInstData bc) => bc.BoolValue;

        #if UNITY_EDITOR
        public void PropField()
        {

        }

        protected override void SetInitialvalueOfInstanceConfig(bool val, EventVars.EVInstData config) => config.BoolValue = val;
#endif
    }
}