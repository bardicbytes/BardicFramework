//alex@bardicbytes.com

namespace BB.BardicFramework.EventVars
{
    public class StringEventVarListener : GenericBaseEventVarListener<string>
    {
        protected override void HandleTypedEventRaised(string data)
        {
            base.HandleTypedEventRaised(data);
        }
    }
}