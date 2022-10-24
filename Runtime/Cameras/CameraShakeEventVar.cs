using BardicBytes.BardicFramework.Core;
using BardicBytes.BardicFramework.Core.EventVars;
using UnityEngine;


namespace BardicBytes.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Core.Prefixes.Cameras+ "Event Var: Camera Shake")]

    public class CameraShakeEventVar : GenericEventVar<CameraShakeConfig>
    {
        public override CameraShakeConfig To(EventVarInstanceField bc) => (CameraShakeConfig)bc.SystemObjectValue;
    }
}