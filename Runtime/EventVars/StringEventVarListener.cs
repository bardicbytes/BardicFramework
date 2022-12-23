//alex@bardicbytes.com

namespace BardicBytes.BardicFramework.EventVars
{
    public class StringEventVarListener : GenericBaseEventVarListener<string>
    {
        protected override void HandleTypedEventRaised(string data)
        {
            base.HandleTypedEventRaised(data);
        }
    }
}