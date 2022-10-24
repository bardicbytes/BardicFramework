//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEngine;


namespace BB.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Prefixes.Cameras+ "Event Var: Camera Shake")]

    public class CameraShakeEventVar : GenericEventVar<CameraShakeConfig>
    {
        public override CameraShakeConfig To(EventVarInstanceField bc) => (CameraShakeConfig)bc.SystemObjectValue;
    }
}