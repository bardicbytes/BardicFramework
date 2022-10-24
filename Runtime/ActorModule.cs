//alex@bardicbytes.com
using UnityEngine;

namespace BB.BardicFramework
{
    [RequireComponent(typeof(Actor))]
    /// <summary>
    /// Remember to Override ActorUpdate instead of implementing the Unit's Update() message handler.
    /// </summary>
    public abstract class ActorModule : MonoBehaviour
    {
        public static implicit operator Actor(ActorModule am) => am.Actor;

        [field:HideInInspector]
        [field: SerializeField]
        public Actor Actor { get; protected set; }

        public string ActorName => Actor.name;

#if UNITY_EDITOR
        private bool warned;
#endif
        public static bool IsModule<T>() where T : Component => typeof(T).IsAssignableFrom(typeof(ActorModule));

        protected virtual void OnValidate()
        {
            if(Actor == null) Actor = GetComponent<Actor>();
        }

        protected virtual void OnEnable()
        {
            Actor.Updated += ActorUpdate;
            Actor.FixedUpdated += ActorFixedUpdate;
        }

        protected virtual void OnDisable()
        {
            Actor.Updated -= ActorUpdate;
            Actor.FixedUpdated -= ActorFixedUpdate;

        }

        /// <summary>
        /// No need to call Base.ActorUpdate when extending ActorModule.
        /// </summary>
        protected virtual void ActorUpdate() {}
        /// <summary>
        /// No need to call Base.ActorFixedUpdate when extending ActorModule.
        /// </summary>
        protected virtual void ActorFixedUpdate() { }
        
        public T GetModule<T>() where T : Component => Actor.GetModule<T>();
        public System.Collections.Generic.List<T> GetModules<T>() where T : Component => Actor.GetModules<T>();

    }


}