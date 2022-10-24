//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    [CreateAssetMenu(menuName = Core.Prefixes.EV + "Int")]
    public class IntEventVar : GenericMinMaxEventVar<int>, IMinMax<int>
    {
        public override int To(EventVarInstanceField bc) => bc.IntValue;
        public override int MinMaxClamp(int val)
        {
            if (hasMax && hasMin)
                return Mathf.Clamp(val, MinValue, MaxValue);
            else if (hasMax)
                return Mathf.Min(val, maxValue);
            else if (hasMin)
                return Mathf.Max(val, minValue);
            else return val;
        }

        public void Increment()
        {
            Raise(Value+1);
        }
        
    }
}