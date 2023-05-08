//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.ActorModules
{
    [RequireComponent(typeof(Rigidbody))]
    public class VirtualKillPlaneModule : ActorModule
    {
        [field: SerializeField]
        public FloatEventVar.Field Height { get; protected set; }
        [field: SerializeField]
        public ActorTag RepositionTarget { get; protected set; }

        protected override void OnValidate()
        {
            base.OnValidate();
            //Height.Validate(this);
        }
        protected override void ActorUpdate()
        {
            if (Actor.Rigidbody.position.y >= Height) return;
            if (RepositionTarget.Value != null)
            {
                Actor.Rigidbody.velocity = Vector3.zero;
                Actor.transform.position = RepositionTarget.Actor0.Center;
                return;
            }
            Actor.SelfDestruct();
        }
    }
}