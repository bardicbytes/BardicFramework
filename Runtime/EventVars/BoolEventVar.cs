//alex@bardicbytes.com
using UnityEngine;

namespace BB.BardicFramework.EventVars
{
    [CreateAssetMenu(menuName = Prefixes.EV+"Bool")]

    public class BoolEventVar : GenericEventVar<bool>
    {
        public void Toggle() => Raise(!Value);
        public override bool To(EventVarInstanceField bc) => bc.BoolValue;

        #if UNITY_EDITOR
        public void PropField()
        {

        }
        #endif
    }
}