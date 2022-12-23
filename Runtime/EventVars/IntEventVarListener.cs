//alex@bardicbytes.com

namespace BardicBytes.BardicFramework.EventVars
{
    public class IntEventVarListener : GenericBaseEventVarListener<int>
    {
        protected override void HandleTypedEventRaised(int data) => base.HandleTypedEventRaised(data);
    }
}