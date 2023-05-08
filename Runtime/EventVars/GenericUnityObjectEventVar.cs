using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    public abstract class GenericUnityObjectEventVar<T> : SimpleGenericEventVar<T> where T : UnityEngine.Object
    {
        public override T To(EVInstData bc) => (T)bc.UnityObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(T val, EVInstData config) => config.UnityObjectValue = val;
#endif
    }
}