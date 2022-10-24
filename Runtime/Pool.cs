using System.Collections.Generic;
using UnityEngine;

namespace BB.BardicFramework
{
    /// <summary>
    /// This class, through it's static functions, allows for the quick pooling and spawning of any component instance without the need for any setup beforhand. 
    /// </summary>
    public class Pool : MonoBehaviour
    {
        public enum LimitMode { Unlimited = 0, RecycleOldest = 1, Ignore = 2 }

        private const int DEFAULT_INITIAL_COUNT = 1;
        private const int DEFAULT_MAX_COUNT = 5;

        private static Transform poolsParent;
        public static Dictionary<Component, Pool> pools;

        public bool destroyOnAllRecycle = false;

        #region Static Functions
        /// <summary>
        /// Unlimited
        /// </summary>
        /// <typeparam name="T">The type of component</typeparam>
        /// <param name="prefab"></param>
        /// <param name="initCount"></param>
        /// <returns></returns>
        public static Pool GetCreatePool<T>(T prefab, int initCount = 2) where T : Component
        {
            return Pool.GetCreatePool<T>(prefab, initCount, initCount + 1, LimitMode.Unlimited);
        }

        public static Pool GetCreatePool<T>(T prefab, int initCount, int maxCount, LimitMode limitMode) where T : Component
        {
            if (prefab == null)
            {
                Debug.Log("Can't create a pool for a null prefab");
            }
            if (!Application.isPlaying)
            {
                Debug.LogError("don't create pools when the game's not playing");
                return null;
            }
            if (pools == null) pools = new Dictionary<Component, Pool>();
            if (!pools.ContainsKey(prefab) && prefab != null) pools.Add(prefab, null);
            if (pools[prefab] == null)
            {
                pools[prefab] = CreatePool(prefab);
                pools[prefab].Initialize(prefab, initCount, maxCount);
                pools[prefab].limitMode = limitMode;
            }
            return pools[prefab];
        }

        private static Pool CreatePool(Component prefab)
        {
            if (poolsParent == null) poolsParent = new GameObject("PoolsParent").transform;
            //Debug.Log("Creating pool for "+template.name);
            Pool p = new GameObject(prefab + "_Pool").AddComponent<Pool>();
            p.transform.SetParent(poolsParent);
            return p;
        }



        public static T Spawn<T>(T template, Vector3 pos) where T : Component
        {
            Pool p = GetCreatePool(template, DEFAULT_INITIAL_COUNT, DEFAULT_MAX_COUNT, LimitMode.Unlimited);
            return p.Spawn<T>(p.transform, pos, Quaternion.identity);
        }

        public static T Spawn<T>(T template, Transform parent) where T : Component
        {
            Pool p = GetCreatePool(template, DEFAULT_INITIAL_COUNT, DEFAULT_MAX_COUNT, LimitMode.Unlimited);
            return p.Spawn<T>(parent, parent.position);
        }

        public static T Spawn<T>(T template, Transform parent, Vector3 pos, Quaternion rot) where T : Component
        {
            Pool p = GetCreatePool(template, DEFAULT_INITIAL_COUNT, DEFAULT_MAX_COUNT, LimitMode.Unlimited);
            return p.Spawn<T>(parent, pos, rot);
        }

        #endregion

        [SerializeField]
        private Component template;
        [SerializeField]
        private LimitMode limitMode;


        private System.Type poolType = default;
        private Queue<Component> availableInstances = default;
        private List<Component> spawnedInstances = default;
        private Dictionary<Component, float> spawnedTimes;
        private int max = 0;
        private bool isInitialized = false;

        public event System.Action<Component> onRecycled;


        public bool DebuggingEnabled { get; set; }
        public int SpawnedCount => spawnedInstances.Count;

        private void Awake()
        {
            spawnedInstances = new List<Component>();
            availableInstances = new Queue<Component>();
        }

        private void Update()
        {
            //recycle one every frame
            for (int i = 0; i < spawnedInstances.Count; i++)
            {
                if (!spawnedInstances[i].gameObject.activeSelf)
                {
                    Recycle(spawnedInstances[i]);
                    //break;
                }
            }
        }

        public void Initialize<T>(T template, int initialCount, int maxCount) where T : Component
        {
            if (template == null) Debug.LogError("Can't init pool with a null template");
            this.max = maxCount;
            availableInstances = new Queue<Component>();
            spawnedTimes = new Dictionary<Component, float>();
            this.poolType = typeof(T);
            this.template = template;
            for (int i = 0; i < initialCount; i++)
            {
                T c = Instantiate((T)(this.template), transform, true);
                c.gameObject.name = template.name + "c" + availableInstances.Count;
                c.gameObject.SetActive(false);
                availableInstances.Enqueue(c);
            }
            isInitialized = true;
        }

        public T GetSpawned<T>(int index) where T : Component
        {
            return spawnedInstances[index] as T;
        }

        public T Spawn<T>() where T : Component => Spawn<T>(transform, Vector3.zero, Quaternion.identity);
        public T Spawn<T>(Transform parent) where T : Component => Spawn<T>(parent, parent.position, parent.rotation);

        public T Spawn<T>(Vector3 position) where T : Component => Spawn<T>(transform, position, Quaternion.identity);

        public T Spawn<T>(Vector3 position, Quaternion rotation) where T : Component => Spawn<T>(transform, position, rotation);

        public T Spawn<T>(Transform parent, Vector3 position) where T : Component => Spawn<T>(parent, position, Quaternion.identity);

        public T Spawn<T>(Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            if (spawnedInstances == null) spawnedInstances = new List<Component>();
            if (availableInstances.Count == 0)
            {
                switch (limitMode)
                {
                    case LimitMode.Ignore:
                        if (spawnedInstances.Count >= max)
                        {
                            Debug.Log("Spawn Request Ignored: " + template.name);
                            return null;
                        }
                        else AddInstance<T>();
                        break;
                    case LimitMode.Unlimited:
                        AddInstance<T>();
                        break;
                    case LimitMode.RecycleOldest:
                        if (spawnedInstances.Count >= max) Recycle(spawnedInstances[0]);
                        else AddInstance<T>();
                        break;
                }
            }
            T next = availableInstances.Dequeue() as T;
            if (parent != transform && !template.gameObject.name.Contains("Block"))
            {
                //Debug.LogFormat(parent, "spawning {0} parented to {1}", template.gameObject.name, parent.gameObject.name);
            }
            //Vector3 preScale = next.transform.localScale;
            next.transform.SetParent(parent, true);
            //next.transform.localScale = preScale;
            next.transform.position = position;
            next.transform.rotation = rotation;
            next.transform.localScale = template.transform.localScale;
            spawnedInstances.Add(next);
            spawnedTimes.Add(next, Time.realtimeSinceStartup);
            next.gameObject.SetActive(true);
            return next;
        }

        /// <summary>
        /// Deactivates the pool spawn and makes it available for respawn immediately
        /// </summary>
        /// <param name="c"></param>
        public void Recycle(Component c)
        {
            if (spawnedTimes == null)
            {
                Debug.LogError("Trying to recycle, but spawnedTimes is null. Is the pool initialized? " + isInitialized);
            }
            if (!isInitialized)
            {
                Debug.LogWarning("Trying to REcycle before pool is initialized? " + this.template.name);
                return;
            }
            float aliveTime = Time.realtimeSinceStartup - spawnedTimes[c];
            //if(Time.frameCount > 4 && Time.timeScale <= 1.1f && Time.frameCount > 2 && aliveTime <= (Time.deltaTime * 2f) * 1.1f)
            //    Debug.LogWarning("FastRecycle: " + c.gameObject.name + ". "+ aliveTime.ToString("0.00") + "s. " + "(<=2f)\nFrame:"+ Time.frameCount, c);
            //if (DebuggingEnabled)
            //    Debug.Log(Time.time + " Recycling "+c.gameObject.name+" after "+(Time.realtimeSinceStartup - spawnedTimes[c]).ToString(".000")+" sec");
            if (spawnedInstances == null) spawnedInstances = new List<Component>();
            if (spawnedInstances.Contains(c) && !availableInstances.Contains(c))
            {
                c.gameObject.SetActive(false);
                c.transform.SetParent(transform);
                spawnedInstances.Remove(c);
                spawnedTimes.Remove(c);
                availableInstances.Enqueue(c);
            }
            onRecycled?.Invoke(c);
            if (spawnedInstances.Count == 0 && destroyOnAllRecycle)
            {
                Destroy(gameObject);
            }
        }

        public void StartSlowDeath()
        {
            destroyOnAllRecycle = true;
        }

        public void RecycleAllThenDestroy()
        {
            var all = spawnedInstances.ToArray();
            for (int i = 0; i < all.Length; i++)
            {
                Recycle(all[i]);
            }
            Destroy(gameObject);
        }

        private void AddInstance<T>() where T : Component
        {
            if (this.template == null) Debug.Log("Can't Add instance, template is null");
            T original = this.template as T;
            try
            {
                T c = Instantiate(original, transform, true);
                c.gameObject.name = original.name + "c" + availableInstances.Count;
                availableInstances.Enqueue(c);
            }
            catch (MissingReferenceException mre)
            {
                throw mre;
            }
        }
    }
}