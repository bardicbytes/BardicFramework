using BardicBytes.BardicFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.ActorModules
{
    public class MatchTransformModule : ActorModule
    {
        [field: SerializeField]
        public ActorTag MatchTarget { get; protected set; }
        [field: SerializeField]
        public Vector3 Offset { get; protected set; }
        [field: SerializeField]
        public bool UseRigidbody { get; protected set; } = true;

        protected override void ActorUpdate()
        {
            if (MatchTarget.Count == 0) return;

            if(UseRigidbody && Actor.HasRigidbody)
            {
                Actor.Rigidbody.MovePosition(MatchTarget.Actor0.transform.position);
                Actor.Rigidbody.MoveRotation(MatchTarget.Actor0.transform.rotation);

                transform.localScale = MatchTarget.Actor0.transform.localScale;
            }
            else
            {
                transform.position = MatchTarget.Actor0.transform.position;
                transform.rotation = MatchTarget.Actor0.transform.rotation;
                transform.localScale = MatchTarget.Actor0.transform.localScale;
            }
        }
    }

}