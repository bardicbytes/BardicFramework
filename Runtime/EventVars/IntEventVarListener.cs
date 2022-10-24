//alex@bardicbytes.com

namespace BB.BardicFramework.EventVars
{
    public class IntEventVarListener : GenericBaseEventVarListener<int>
    {
        protected override void HandleTypedEventRaised(int data) => base.HandleTypedEventRaised(data);
    }
}