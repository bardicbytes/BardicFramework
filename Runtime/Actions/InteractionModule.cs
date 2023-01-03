//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
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