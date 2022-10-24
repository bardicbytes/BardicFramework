//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEngine;

namespace BB.BardicFramework.ActorModules
{
    [RequireComponent(typeof(EventVarInstancer))]
    /// <summary>
    /// Moves the actor toward's the first Tagged Target Actor, the position is offset by center of mass.
    /// </summary>
    public class MovePositionModule : ActorModule
    {
        [field: SerializeField]
        public virtual ActorTag Target { get; protected set; } = default;
        [field: SerializeField]
        public virtual bool DisableOnCollision { get; protected set; } = false;

        [field: SerializeField]
        private FloatEventVar.Field Speed { get; set; } = default;

        [SerializeField]
        private bool debug_warnIfTagIsNotUnique = true;

        private ActorTag initialTarget = null;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (debug_warnIfTagIsNotUnique && Target != null && !Target.UniqueTag)
            {
                Debug.LogWarning(name+"'s Target " + Target.name + " may have multiple instances. Check \"UniqueTag\" field", this);
            }

            if (DisableOnCollision && !Actor.HasModule<CollisionModule>())
            {
                gameObject.AddComponent<CollisionModule>();
            }
            //todo: remove the need to validate EV fields manually in modules
            Speed.Validate(this);
        }

        private void Awake()
        {
            initialTarget = Target;
            if (DisableOnCollision)
            {
                var cm = Actor.GetModule<CollisionModule>();
                cm.CollisionEntered.AddListener(HandleCollision);
            }

            void HandleCollision(Collision collision)
            {
                enabled = false;
            }
            if (Target == null) enabled = false;
        }

        protected virtual void FixedUpdate()
        {
            if(Target == null && initialTarget != null)
            {
                Target = initialTarget;
            }
            if (Target == null)
            {
                enabled = false;
                return;
            }
            if(initialTarget != null 
                && !Target.HasActiveActors 
                && initialTarget != Target)
            {
                Target = initialTarget;
            }
            if (!Target.HasActiveActors) return;

            var p = Vector3.MoveTowards(Actor.Center, Target.Actor0.Center, Speed * Time.fixedDeltaTime);
            Actor.Rigidbody.MovePosition(p - (Actor.HasRigidbody ? Actor.Rigidbody.centerOfMass : Vector3.zero));
        }

        public void SetTargetOverride(ActorTag newTarget)
        {
            this.Target = newTarget;
            enabled = true;
        }

        public void ReleaseTargetOverride()
        {
            this.Target = initialTarget;
            if (Target == null) enabled = false;
        }

#if UNITY_EDITOR
        public void EDITORONLY_SetTarget(ActorTag at)
        {
            this.Target = at;
        }
#endif
    }
}