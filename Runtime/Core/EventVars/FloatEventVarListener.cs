//alex@bardicbytes.com
namespace BardicBytes.BardicFramework.Core.EventVars
{
    public class FloatEventVarListener : GenericBaseEventVarListener<float>
    {
        protected override void HandleTypedEventRaised(float data) => base.HandleTypedEventRaised(data);
    }
}