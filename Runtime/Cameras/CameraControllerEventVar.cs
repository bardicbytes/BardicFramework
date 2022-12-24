//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Prefixes.Cameras + "Event Var: Camera Controller")]
    public class CameraControllerEventVar : SimpleGenericEventVar<CameraController> 
    {
        public override CameraController To(EventVars.EVInstData bc) => (CameraController)bc.SystemObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(CameraController val, EventVars.EVInstData config) => config.UnityObjectValue = val;
#endif

    }
}