//alex@bardicbytes.com
//https://github.com/bardicbytes/BardicFramework
using BB.BardicFramework.EventVars;
using System.Collections.Generic;
using UnityEngine;

namespace BB.BardicFramework
{
    /// <summary>
    /// The Actor is the head component of the Bardic Framework, connecting all the modules together.
    /// Like a GameObject needs Components, the Actor needs ActorModules.
    /// </summary>
    public class Actor : MonoBehaviour
    {
        private static Director director;

        [field:SerializeField]
        [field:HideInInspector]
        public Component[] AllComponents { get; protected set; } = default;

        [SerializeField]
        [HideInInspector]
        public bool[] compisModule = default;

        public EventVarInstancer Instancer => GetModule<EventVarInstancer>();
        public Rigidbody Rigidbody => GetModule<Rigidbody>();
        public bool HasRigidbody => HasModule<Rigidbody>();

        /// <summary>
        /// The center of mass in world space.
        /// </summary>
        public Vector3 Center => transform.position + (HasRigidbody ? Rigidbody.centerOfMass : Vector3.zero);

#if UNITY_EDITOR
        [SerializeField]
        [HideInInspector]
        private int[] compCache;        
#endif

        private Dictionary<System.Type, List<int>> modulesLookup;
        private Dictionary<System.Type, List<int>> ModulesLookup
        {
            get
            {
                if (modulesLookup == null) PopulateModuleLookup();
                return modulesLookup;
            }
        }

        public delegate bool DestructionDelayDelegate();

        /// <summary>
        /// subscribe to this delegate, and return false to delay the destruction of the actor.
        /// </summary>
        public DestructionDelayDelegate Destructing;
        public event System.Action Updated;
        public event System.Action FixedUpdated;

#if UNITY_EDITOR

        private void OnValidate()
        {            
            RefreshComps();
        }

        private void RefreshComps()
        {
            var foundComps = gameObject.GetComponents<Component>();
            bool refreshCompCache = false;

            //make sure the compCache is initialized and
            //do a count comparison to check if we should refresh
            if (compCache == null || compCache.Length != foundComps.Length - 1)
            {
                refreshCompCache = true;
            }

            if (!refreshCompCache)
            {
                UnityEditor.ArrayUtility.Remove(ref foundComps, this);
                //compare the cache to the current
                for (int i = 0; i < foundComps.Length; i++)
                {
                    if (foundComps[i].GetType().ToString().GetHashCode() != compCache[i])
                    {
                        refreshCompCache = true;
                        break;
                    }
                }
            }

            if (refreshCompCache)
            {
                UnityEditor.ArrayUtility.Remove(ref foundComps, this);
                AllComponents = foundComps;
                compCache = new int[AllComponents.Length];
                compisModule = new bool[AllComponents.Length];
                for (int i = 0; i < AllComponents.Length; i++)
                {
                    compCache[i] = AllComponents[i].GetType().GetHashCode();
                    compisModule[i] = AllComponents[i] is ActorModule;
                }
                PopulateModuleLookup();
            }
        }
        
#endif
        private void Awake()
        {
            PopulateModuleLookup();
        }

        protected virtual void OnEnable()
        {
            if(director == null)
            {
                director = new GameObject("Actor Director").AddComponent<Director>();
            }
            director.Register(this);
        }

        protected virtual void OnDisable()
        {
            director.Deregister(this);
        }

        public void ActorUpdate() => Updated?.Invoke();
        public void ActorFixedUpdate() => FixedUpdated?.Invoke();

        private void PopulateModuleLookup()
        {
            modulesLookup = new Dictionary<System.Type, List<int>>();
            for (int i = 0; i < AllComponents.Length; i++)
            {
                var key = AllComponents[i].GetType();
                if (!ModulesLookup.ContainsKey(key)) ModulesLookup.Add(key, new List<int>());
                ModulesLookup[key].Add(i);
            }
        }

        /// <typeparam name="T">Any Component</typeparam>
        /// <returns>The first instance of the component or null</returns>
        public T GetModule<T>() where T : Component
        {
            System.Type key = typeof(T);
            if (!HasModule<T>()) return null;
            return AllComponents[ModulesLookup[key][0]] as T;
        }

        public bool HasModule<T>() where T : Component => ModulesLookup.ContainsKey(typeof(T));


        /// <typeparam name="T">Any Component</typeparam>
        /// <returns>A list of all instances of Type T</returns>
        public List<T> GetModules<T>() where T : Component
        {
            System.Type key = typeof(T);
            if (!HasModule<T>()) return null;
            var modList = new List<T>();
            for(int i =0; i < ModulesLookup[key].Count ;i++)
            {
                modList.Add(AllComponents[ModulesLookup[key][i]] as T);
            }
            return modList;
        }

        /// <summary>
        /// Convenience for Instancer.GetInstance(eventVarAssetRef)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventVarAssetRef"></param>
        /// <returns>NULL if there is no actor instance</returns>
        public T GetInstance<T>(T eventVarAssetRef) where T : EventVar
        {
            return Instancer == null ? null : Instancer.GetInstance(eventVarAssetRef);
        }

        //convenience method
        public bool HasActorInstance(EventVar eventVarAssetRef) => Instancer != null &&  Instancer.HasInstance(eventVarAssetRef);

        /// <summary>
        /// allows destroy to be delayed modularly
        /// </summary>
        public virtual async void SelfDestruct()
        {
            while (Destructing != null && !Destructing.Invoke())
            {
                await System.Threading.Tasks.Task.Yield();
            }
            Destroy(gameObject);
        }

    }
}