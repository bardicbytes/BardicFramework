//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    public class InteractionEvent : UnityEngine.Events.UnityEvent<Actor> { }

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

    public class InteractionModule : ActorModule
    {
        [SerializeField] protected InteractionDefinition[] interactions;

        protected Dictionary<ActorTag, InteractionDefinition> interactionLookup;

        protected void Awake()
        {
            interactionLookup = new Dictionary<ActorTag, InteractionDefinition>();
            for(int i = 0; i < interactions.Length; i++)
            {
                interactionLookup.Add(interactions[i].ActualizationTag, interactions[i]);
            }
        }

        public void RequestInteraction(Actor requester, ActorTag actualizationTag)
        {
            if (!interactionLookup.ContainsKey(actualizationTag)) return;
            interactionLookup[actualizationTag].TriggerInteraction(requester);
        }

        public Ray GetInteractPoint(ActorTag actualizationTag)
        {
            if(interactionLookup.ContainsKey(actualizationTag))
                return interactionLookup[actualizationTag].InteractionPoint;
            else
                return new Ray(transform.position, Vector3.zero);
        }
    }
}