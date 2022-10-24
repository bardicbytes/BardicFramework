//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using System.Collections.Generic;
using UnityEngine;

namespace BB.BardicFramework
{
    public class EventVarInstancer : ActorModule
    {
        [SerializeField]
        private List<EventVar> eventVars;
        [SerializeField]
        private EventVarInstanceField[] eventVarInstances;

#if UNITY_EDITOR

        [HideInInspector]
        [SerializeField]
        private int[] evHashCache;
#endif
        private Dictionary<EventVar, int> evInstanceLookup = default;

        protected override void OnValidate()
        {
            base.OnValidate();

            bool refreshEvCache = false;

            if (eventVars != null &&
                (evHashCache == null || evHashCache.Length != eventVars.Count
                || eventVarInstances == null || eventVarInstances.Length != eventVars.Count))
            {
                //null or miscount
                refreshEvCache = true;
            }

            if (!refreshEvCache && eventVars != null && eventVars.Count > 0)
            {
                for (int i = 0; i < eventVars.Count; i++)
                {
                    if (eventVars[i] == null) continue;
                    if (eventVarInstances[i] == null)
                    {
                        refreshEvCache = true;
                        break;
                    }
                    //compare each event var to the cache
                    if (evHashCache[i] != eventVars[i].GUID.GetHashCode())
                    {
                        refreshEvCache = true;
                        break;
                    }
                }
            }

            if (refreshEvCache && eventVars != null && eventVars.Count > 0)
            {
                var instBackup = eventVarInstances;
                var cacheBackup = evHashCache;
                eventVarInstances = new EventVarInstanceField[eventVars.Count];
                evHashCache = new int[eventVars.Count];
                for (int i = 0; i < eventVars.Count; i++)
                {
                    if (eventVars[i] == null)
                    {
                        evHashCache[i] = 0;
                        continue;
                    }
                    if (instBackup == null || cacheBackup == null || instBackup.Length != cacheBackup.Length)
                    {
                        eventVarInstances[i] = eventVars[i].CreateInstanceConfig();
                    }
                    else
                    {
                        var found = Restorebackup(eventVars[i], instBackup, cacheBackup);
                        eventVarInstances[i] = found == null ? eventVars[i].CreateInstanceConfig() : found;
                    }
                    evHashCache[i] = eventVars[i].GUID.GetHashCode();
                }
            }

            EventVarInstanceField Restorebackup(EventVar src, EventVarInstanceField[] instBackup, int[] cacheBackup)
            {
                Debug.Assert(instBackup != null);
                Debug.Assert(cacheBackup != null);
                Debug.Assert(instBackup.Length == cacheBackup.Length);

                EventVarInstanceField c = null;
                for (int i = 0; i < instBackup.Length; i++)
                {
                    if (src.GUID.GetHashCode() == cacheBackup[i]) c = instBackup[i];
                }
                return c;
            }
        }

        public void Awake()
        {
            evInstanceLookup = new Dictionary<EventVar, int>();
            for (int i = 0; i < eventVars.Count; i++)
            {
                eventVarInstances[i] = eventVars[i].Clone<EventVar>(this).CreateInstanceConfig();
                eventVarInstances[i].BaseRuntimeInstance.SetInitialValue(eventVarInstances[i]);
                eventVarInstances[i].BaseRuntimeInstance.name = eventVars[i].name + "--" + Actor.gameObject.name;
                evInstanceLookup.Add(eventVars[i], i);
            }
        }


        public T GetInstance<T>(T eventVarAssetRef) where T : EventVar
        {
            if (!HasInstance(eventVarAssetRef))
            {
                Debug.LogWarning("no instance found, Check with HasInstance first.");
                return null;
            }
            T t = eventVarInstances[evInstanceLookup[eventVarAssetRef]] as T;
            return t;
        }

        public bool HasInstance(EventVar ev) => evInstanceLookup.ContainsKey(ev);
    }
}