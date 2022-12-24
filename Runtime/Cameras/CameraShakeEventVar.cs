//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;


namespace BardicBytes.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Prefixes.Cameras+ "Event Var: Camera Shake")]

    public class CameraShakeEventVar : SimpleGenericEventVar<CameraShakeConfig>
    {
        public override CameraShakeConfig To(EventVars.EVInstData bc) => (CameraShakeConfig)bc.SystemObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(CameraShakeConfig val, EventVars.EVInstData config) => config.SystemObjectValue = val;
#endif
    }
}