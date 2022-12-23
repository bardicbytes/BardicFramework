//alex@bardicbytes.com
using BardicBytes.BardicFramework;
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicSamples.ActorModules
{
    /// <summary>
    /// Moves the actor toward's the first Tagged Target Actor, the position is offset by center of mass.
    /// </summary>
    public class SpinTransformModule : ActorModule
    {
        [field: SerializeField]
        public virtual Transform Target { get; protected set; } = default;

        [field: SerializeField]
        public virtual Space Space { get; protected set; } = default;

        [field: SerializeField]
        private Vector3EventVar.Field Speed { get; set; } = default;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (Target == null) Target = Target = transform;
            Speed.Validate(this);
        }
        protected override void ActorUpdate()
        {
            Vector3 s = Speed;
            Target.Rotate(s * Time.deltaTime, Space);
        }
    }
}