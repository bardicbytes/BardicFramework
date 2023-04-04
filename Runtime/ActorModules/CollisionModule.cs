using UnityEngine;
using UnityEngine.Events;

namespace BardicBytes.BardicFramework.ActorModules
{
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionModule : ActorModule
    {
        [field: SerializeField]
        public bool LogOnCollision = false;

        [field: SerializeField]
        public UnityEvent<Collision> CollisionEntered { get; protected set; }
        [field: SerializeField]
        //public UnityEvent<Collision> CollisionStayed { get; protected set; }
        //[field: SerializeField]
        public UnityEvent<Collision> CollisionExited { get; protected set; }


        protected override void OnValidate()
        {
            base.OnValidate();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            CollisionEntered.Invoke(collision);
            if (LogOnCollision) LogCollision(collision);
        }

        //protected virtual void OnCollisionStay(Collision collision)
        //{
        //    CollisionStayed.Invoke(collision);
        //}

        protected virtual void OnCollisionExit(Collision collision)
        {
            CollisionExited.Invoke(collision);
        }

        public void LogCollision(Collision collision)
        {
            if (!Debug.isDebugBuild) return;

            var otherRB = collision.collider.attachedRigidbody;
            var actor = otherRB.GetComponent<Actor>();
            var s = string.Format("[f{2}] Collision! {0} Has Actor: {1}", otherRB.gameObject.name, actor != null, Time.frameCount);

            Debug.Log(s, otherRB.gameObject);
        }
    }
}