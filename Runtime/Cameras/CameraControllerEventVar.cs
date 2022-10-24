using BardicBytes.BardicFramework.Core;
using BardicBytes.BardicFramework.Core.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Core.Prefixes.Cameras + "Event Var: Camera Controller")]
    public class CameraControllerEventVar : GenericEventVar<CameraController> 
    {
        public override CameraController To(EventVarInstanceField bc) => (CameraController)bc.SystemObjectValue;
    }
}