//alex@bardicbytes.com
using BardicBytes.BardicFramework;
using BardicBytes.BardicFramework.ActorModules;
using UnityEngine;

namespace BardicBytes.BardicSamples
{
    [RequireComponent(typeof(MovePositionModule))]
    [RequireComponent(typeof(CollisionModule))]
    public class HunterModule : ActorModule
    {
        [field: SerializeField]
        [Tooltip("MovePositionModule's Target will be overridden by this field.")]
        public ActorTag Target { get; set; } = default;
        private MovePositionModule Mover => Actor.GetModule<MovePositionModule>();
        private CollisionModule CollisionModule => Actor.GetModule<CollisionModule>();

        protected virtual void Reset()
        {
            if (Target == null && Mover.Target != null)
            {
                Target = Mover.Target;
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();
#if UNITY_EDITOR
            if (enabled && Target != null && Mover.Target != Target)
            {
                Mover.EDITORONLY_SetTarget(Target);
                UnityEditor.EditorUtility.SetDirty(Mover);
            }
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CollisionModule.CollisionEntered.AddListener(HandleCollision);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            CollisionModule.CollisionEntered.RemoveListener(HandleCollision);
        }

        void Update()
        {
            if (Target == null || !Target.HasActiveActors) return;
            Mover.SetTargetOverride(Target);
        }

        public void HandleCollision(Collision collision)
        {
            if (collision.collider.attachedRigidbody == Target.Actor0.Rigidbody)
            {
                Target.Actor0.SelfDestruct();
            }
        }
    }
}