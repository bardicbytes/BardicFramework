//alex@bardicbytes.com
namespace BardicBytes.BardicFramework.EventVars
{
    public class FloatEventVarListener : GenericEventVarListener<float>
    {
        protected override void HandleTypedEventRaised(float data) => base.HandleTypedEventRaised(data);
    }
}