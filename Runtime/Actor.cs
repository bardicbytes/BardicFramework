//alex@bardicbytes.com
//https://github.com/bardicbytes/BardicFramework
using BardicBytes.BardicFramework.EventVars;
using BardicBytes.BardicFramework.Utilities;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BardicBytes.BardicFramework
{

    /// <summary>
    /// The Actor is the head component of the Bardic Framework, connecting all the modules together.
    /// Like a GameObject needs Components, the Actor needs ActorModules.
    /// </summary>
    public class Actor : MonoBehaviour, IBardicEditorable
    {
        [System.Serializable]
        public class GizmosMode
        {
            public bool cube = false;
            public bool sphere = true;
            public bool solidWhenSelected = true;
            public bool enableWire = true;
            public Color color = new Color(1, 1, 1.5f);
            public float radius = .5f;
        }

        public delegate bool DestructionDelayDelegate();

        private static Director director;

        [field: SerializeField]
        [Tooltip("When true, the actor game object is deactivated on SelfDestruct, but will be destroyed when false.")]
        public BoolEventVar.Field DeactivateOnSelfDestruct { get; protected set; }
        [field: Space]
        [field: SerializeField]
        public StringEventVar DebugEvent { get; protected set; }


        /// <summary>
        /// The center of mass in world space.
        /// </summary>
        public Vector3 Center => transform.position + (HasRigidbody ? Rigidbody.centerOfMass : Vector3.zero);

#if UNITY_EDITOR
        [field: SerializeField]
        private GizmosMode DebugGizmo { get; set; } = default;

        [SerializeField]
        private int[] compCache;
#endif

        [field: SerializeField]
        public Component[] AllComponents { get; protected set; } = default;


        [SerializeField]
        public bool[] compIsModule = default;

        public EventVarInstancer Instancer => GetModule<EventVarInstancer>();
        public Rigidbody Rigidbody => GetModule<Rigidbody>();
        public bool HasRigidbody => HasModule<Rigidbody>();

        private StringBuilder debugSB;
        private Dictionary<System.Type, List<int>> modulesLookup;
        private Dictionary<System.Type, List<int>> ModulesLookup
        {
            get
            {
                if (modulesLookup == null) PopulateModuleLookup();
                return modulesLookup;
            }
        }

        /// <summary>
        /// subscribe to this delegate, and return false to delay the destruction of the actor.
        /// </summary>
        public DestructionDelayDelegate DestructionDelay;
        public bool IsDestructing { get; protected set; }


        public event System.Action Updated;
        public event System.Action FixedUpdated;

#if UNITY_EDITOR
        public string[] EditorFieldNames => new string[] { StringFormatting.GetBackingFieldName("DeactivateOnSelfDestruct") };
        public bool DrawOtherFields => true;

        private void OnDrawGizmos()
        {
            if (DebugGizmo.enableWire) DrawGizmos(true);
        }

        private void OnDrawGizmosSelected()
        {
            if (DebugGizmo.solidWhenSelected) DrawGizmos(false);
        }

        private void DrawGizmos(bool wire)
        {
            Gizmos.color = DebugGizmo.color;
            if (DebugGizmo.cube)
            {
                if (wire) Gizmos.DrawWireCube(Center, Vector3.one * DebugGizmo.radius * 2);
                if (!wire) Gizmos.DrawCube(Center, Vector3.one * DebugGizmo.radius * 2);
            }
            if (DebugGizmo.sphere)
            {
                if (wire) Gizmos.DrawWireSphere(Center, DebugGizmo.radius);
                if (!wire) Gizmos.DrawSphere(Center, DebugGizmo.radius);
            }
        }

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
                compIsModule = new bool[AllComponents.Length];

                Debug.Assert(AllComponents.Length == foundComps.Length);

                for (int i = 0; i < AllComponents.Length; i++)
                {
                    if (AllComponents[i] == null)
                    {
                        Debug.LogWarning("null ?");
                    }
                    compCache[i] = AllComponents[i].GetType().GetHashCode();
                    compIsModule[i] = AllComponents[i] is ActorModule;
                }
                PopulateModuleLookup();
            }
        }
#endif
        private void Awake()
        {
            if (debugSB == null) debugSB = new StringBuilder();
            PopulateModuleLookup();
        }

        protected virtual void OnEnable()
        {
            if (director == null)
            {
                director = new GameObject("Actor Director").AddComponent<Director>();
            }
            director.Register(this);
        }

        protected virtual void OnDisable()
        {
            director.Deregister(this);
        }

        public void ActorUpdate()
        {
            Updated.Invoke();
            DoDebug();

            void DoDebug()
            {
                if (DebugEvent == null) return;
                if (debugSB == null) return;
                if (DebugEvent.TotalListeners == 0) return;
                debugSB.Clear();
                for (int i = 0; i < AllComponents.Length; i++)
                {
                    if (!compIsModule[i]) continue;
                    var am = AllComponents[i] as ActorModule;
                    am.CollectActorDebugInfo(debugSB);
                }
                if (debugSB.Length > 0)
                {
                    debugSB.Insert(0, name + "\n");
                    DebugEvent.Raise(debugSB.ToString());
                }
            }
        }

        public void ActorFixedUpdate()
        {
            FixedUpdated?.Invoke();
        }

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

        /// <typeparam name="ComponentT">Any Component</typeparam>
        /// <returns>The first instance of the component or null</returns>
        public ComponentT GetModule<ComponentT>() where ComponentT : Component
        {
            System.Type key = typeof(ComponentT);
            if (!HasModule<ComponentT>()) return null;
            return AllComponents[ModulesLookup[key][0]] as ComponentT;
        }

        public bool HasModule<T>() where T : Component => ModulesLookup.ContainsKey(typeof(T));


        /// <typeparam name="ComponentT">Any Component</typeparam>
        /// <returns>A list of all instances of Type T</returns>
        public List<ComponentT> GetModules<ComponentT>() where ComponentT : Component
        {
            System.Type key = typeof(ComponentT);
            if (!HasModule<ComponentT>()) return null;
            var modList = new List<ComponentT>();
            for (int i = 0; i < ModulesLookup[key].Count; i++)
            {
                modList.Add(AllComponents[ModulesLookup[key][i]] as ComponentT);
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

        /// <summary>
        /// allows destroy to be delayed modularly.
        /// </summary>
        public virtual async void SelfDestruct()
        {
            IsDestructing = true;
            enabled = false;
            while (DestructionDelay != null && !DestructionDelay.Invoke())
            {
                await System.Threading.Tasks.Task.Yield();
            }

            if (DeactivateOnSelfDestruct) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }
}