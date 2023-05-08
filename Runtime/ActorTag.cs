// alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework
{
    [CreateAssetMenu(menuName = Prefixes.BardicBase + "Actor Tag")]
    public class ActorTag : SimpleGenericEventVar<List<TagModule>>
    {
        [field: SerializeField] public string DisplayName { get; set; }
        [field: SerializeField] public bool UniqueTag { get; set; }

        public Actor Actor0 => ActiveActors[0].Actor;
        public int Count => Value.Count;

        public List<TagModule> ActiveActors { get => base.Value; set => base.Raise(value); }
        public bool HasActiveActors => ActiveActors.Count > 0;
        protected override void Reset()
        {
            base.Reset();
            if (DisplayName == "") DisplayName = name;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            ActiveActors = null;
            if (DisplayName == "") DisplayName = name;
        }

        public void Register(TagModule tagMod)
        {
            InitActorList();
            Debug.Assert(!ActiveActors.Contains(tagMod));
            if (UniqueTag && ActiveActors.Count >= 1)
            {
                throw new System.Exception(string.Format("Register() called twice on {0}, but it's multiple Tag registrations are not allowed. ", name));
            }
            ActiveActors.Add(tagMod);
        }

        private void InitActorList()
        {
            if (ActiveActors != null) return;
            base.SetInitialValue(new List<TagModule>());
        }

        public void Deregister(TagModule tagMod)
        {
            InitActorList();

            Debug.Assert(ActiveActors != null && ActiveActors.Contains(tagMod));
            ActiveActors.Remove(tagMod);
        }

        public bool IsTagged(TagModule tagMod)
        {
            InitActorList();

            return ActiveActors.Contains(tagMod);
        }

        public void SetTaggedActorsActive(bool active)
        {
            for (int i = 0; i < ActiveActors.Count; i++)
            {
                ActiveActors[i].gameObject.SetActive(active);
            }
        }

        public List<T> GetTaggedModules<T>() where T : Component
        {
            var modules = new List<T>();
            for (int i = 0; i < ActiveActors.Count; i++)
            {
                var m = ActiveActors[i].GetModule<T>();
                if (m == null) continue;
                modules.Add(m);
            }
            return modules;
        }

        public override List<TagModule> To(EventVars.EVInstData bc) => bc.SystemObjectValue as List<TagModule>;

#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(List<TagModule> val, EventVars.EVInstData config) => config.SystemObjectValue = val;

#endif
    }
}