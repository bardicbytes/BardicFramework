//alex@bardicbytes.com
using UnityEngine;

namespace BB.BardicFramework.EventVars
{
    [CreateAssetMenu(menuName = Prefixes.EV + "String")]
    public class StringEventVar : GenericEventVar<string>
    {
        public override string To(EventVarInstanceField bc) => bc.StringValue;
    }
}