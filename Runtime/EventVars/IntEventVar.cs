//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{

    [CreateAssetMenu(menuName = Prefixes.EV + "Int")]
    public class IntEventVar : GenericMinMaxEventVar<int>, IMinMax<int>
    {
        public override int To(EventVars.EVInstData bc) => bc.IntValue;
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
        protected override void SetInitialvalueOfInstanceConfig(int val, EventVars.EVInstData config) => config.IntValue = val;

        public void Increment() => Raise(Value + 1);


    }
}