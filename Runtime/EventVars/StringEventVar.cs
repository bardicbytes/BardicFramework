//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    [CreateAssetMenu(menuName = Prefixes.EV + "String")]
    public class StringEventVar : SimpleGenericEventVar<string>
    {
        public override string To(EventVars.EVInstData bc) => bc.StringValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(string val, EventVars.EVInstData config) => config.StringValue = val;
#endif
    }

}