//alex@bardicbytes.com
namespace BB.BardicFramework.EventVars
{
    public class FloatEventVarListener : GenericBaseEventVarListener<float>
    {
        protected override void HandleTypedEventRaised(float data) => base.HandleTypedEventRaised(data);
    }
}