//alex@bardicbytes.com
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BardicBytes.BardicFramework.Core.EventVars
{
    [CreateAssetMenu(menuName = Core.Prefixes.EV+"Bool")]

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