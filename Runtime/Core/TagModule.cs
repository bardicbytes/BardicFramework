//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core
{
    public class TagModule : ActorModule
    {
        [field: SerializeField]
        public ActorTag[] Tags { get; protected set; } = default;

        protected override void OnEnable()
        {
            base.OnEnable();
            SetTagRegistration(true);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            SetTagRegistration(false);
        }

        private void SetTagRegistration(bool register)
        {
            for (int i = 0; i < Tags.Length; i++)
            {
                if (register) Tags[i].Register(this);
                else Tags[i].Deregister(this);
            }
        }

    }


}