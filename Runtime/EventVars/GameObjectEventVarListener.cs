//alex@bardicbytes.com

namespace BardicBytes.BardicFramework.EventVars
{
    public class GameObjectEventVarListener : GenericEventVarListener<GameObjectEventVar>
    {
        protected override void HandleTypedEventRaised(GameObjectEventVar data) => base.HandleTypedEventRaised(data);
    }
}