using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    public abstract class GenericSystemObjectEventVar<T> : SimpleGenericEventVar<T>
    {
        public override T To(EVInstData bc) => (T)bc.SystemObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(T val, EVInstData config) => config.SystemObjectValue = val;
#endif
    }
}