//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    [CreateAssetMenu(menuName = Core.Prefixes.EV + "String")]
    public class StringEventVar : GenericEventVar<string>
    {
        public override string To(EventVarInstanceField bc) => bc.StringValue;
    }
}