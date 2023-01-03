//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    [System.Serializable]
    public class InteractionDefinition
    {
        public const string DEFAULT_INTERACTION_PARAM = "interacting";
        [SerializeField] protected ActorTag actualizationTag;
        [SerializeField] protected InteractionEvent onInteractRequested;
        [SerializeField] protected string animatorStateBool = DEFAULT_INTERACTION_PARAM;
        [SerializeField] protected Transform interactionPoint;

        public Ray InteractionPoint
        {
            get
            {
                return new Ray(interactionPoint.position, interactionPoint.forward);
            }
        }

        public ActorTag ActualizationTag { get { return actualizationTag; } }

        public void TriggerInteraction(Actor requester)
        {
            onInteractRequested.Invoke(requester);
            if (requester.HasModule<Animator>() && !string.IsNullOrEmpty(animatorStateBool))
                requester.GetModule<Animator>().SetBool(animatorStateBool, true);
        }
    }
}