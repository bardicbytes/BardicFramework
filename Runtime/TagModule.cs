//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework
{
    public class TagModule : ActorModule
    {
        [field: SerializeField]
        public ActorTag[] Tags { get; protected set; } = default;

        protected override void OnValidate()
        {
            base.OnValidate();
            bool refresh = false;
            for (int i = 0; !refresh && i < Tags.Length; i++)
            {
                if (Tags[i] == null)
                {
                    refresh = true;
                }
            }
            if (refresh)
            {
                List<ActorTag> tagList = new List<ActorTag>(Tags);
                for (int i = 0; i < tagList.Count; i++)
                {
                    if (tagList[i] == null)
                    {
                        tagList.RemoveAt(i);
                        i--;
                    }
                }
                Tags = tagList.ToArray();
            }
        }

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