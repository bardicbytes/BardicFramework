//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEngine;

namespace BB.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Prefixes.Cameras + "Event Var: Camera Controller")]
    public class CameraControllerEventVar : GenericEventVar<CameraController> 
    {
        public override CameraController To(EventVarInstanceField bc) => (CameraController)bc.SystemObjectValue;
    }
}