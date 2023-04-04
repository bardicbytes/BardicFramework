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

        [SerializeField]
        private bool matchPosition = true;
        [SerializeField]
        private bool matchRotation = true;
        [SerializeField]
        private bool matchScale = false;

        protected override void ActorUpdate()
        {
            if (MatchTarget.Count == 0) return;

            if (UseRigidbody && Actor.HasRigidbody)
            {
                if (matchPosition)
                    Actor.Rigidbody.MovePosition(MatchTarget.Actor0.transform.position);
                if (matchRotation)
                    Actor.Rigidbody.MoveRotation(MatchTarget.Actor0.transform.rotation);
                if (matchScale)
                    transform.localScale = MatchTarget.Actor0.transform.localScale;
            }
            else
            {
                if (matchPosition)
                    transform.position = MatchTarget.Actor0.transform.position;
                if (matchRotation)
                    transform.rotation = MatchTarget.Actor0.transform.rotation;
                if (matchScale)
                    transform.localScale = MatchTarget.Actor0.transform.localScale;
            }
        }
    }

}