//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    [CreateAssetMenu(menuName = Core.Prefixes.EV + "Float")]
    public class FloatEventVar : GenericMinMaxEventVar<float>, IMinMax<float>
    {
        public override float MinMaxClamp(float val)
        {
            if (hasMax && hasMin)
                return Mathf.Clamp(val, MinValue, MaxValue);
            else if (hasMax)
                return Mathf.Min(val, maxValue);
            else if (hasMin)
                return Mathf.Max(val, minValue);
            else return val;
        }
        public override float To(EventVarInstanceField bc) => bc.FloatValue;
    }
}