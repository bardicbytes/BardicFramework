//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects+"Special Effect")]
    public class SpecialEffect : SimpleGenericEventVar<SpecialEffect.PlayRequest>
    {
        [SerializeField]
        private StringEventVar debugMsg = default;

        [System.Serializable]
        public struct PlayRequest
        {
            public System.Action<ActiveHandle> receiveHandleCallback;
            public SpecialEffect fx;
            public Transform target;
            public Vector3 pos;

            public Vector3 Pos => target == null ? pos : target.position;
            public Quaternion Rot => target == null ? Quaternion.identity : target.rotation;
            public bool HasInactiveTarget => target != null && !target.gameObject.activeInHierarchy;

            public PlayRequest(SpecialEffect fx, Vector3 pos, Transform target, System.Action<ActiveHandle> receiveHandleCallback)
            {
                this.receiveHandleCallback = receiveHandleCallback;
                this.fx = fx;
                this.target = target;
                if (target != null)
                    this.pos = target.transform.position;
                else
                    this.pos = pos;
            }

            public PlayRequest(SpecialEffect fx, Transform target) : this(fx, target.position, target, null) { }

            public PlayRequest(SpecialEffect fx, Vector3 pos) : this(fx, pos, null, null) { }

            public PlayRequest(SpecialEffect fx) : this(fx, null) { }
        }

        [System.Serializable]
        public class ActiveHandle
        {
            public PlayRequest request;
            //index coresponds beat index
            public Pool[] particlePools;
            public float startTime;
            //public System.Action<ActiveHandle> completeCallback;
            public bool[] beatFired;
            public ParticleSystem[] spawnedParticles;
            public SoundEffect.ActiveHandle[] soundEffectHandles;
            public bool[] beatComplete;
            public bool wasConstructed;

            public ActiveHandle(PlayRequest request)
            {
                beatComplete = new bool[request.fx.beats.Length];
                //Debug.Log("Active handle made for FX "+request.fx.name);
                //this.completeCallback = request.receiveHandleCallback;
                this.request = request;
                this.startTime = Time.time;
                soundEffectHandles = new SoundEffect.ActiveHandle[request.fx.beats.Length];
                spawnedParticles = new ParticleSystem[request.fx.beats.Length];
                beatFired = new bool[request.fx.beats.Length];
                particlePools = new Pool[request.fx.beats.Length];
                for (int i = 0; i < request.fx.beats.Length; i++)
                {
                    if (request.fx.beats[i].particlePrefab == null)
                    {
                        //leave the pool array element null if there is no prefab to pool
                        continue;
                    }
                    particlePools[i] = Pool.GetCreatePool(request.fx.beats[i].particlePrefab, request.fx.initCount, request.fx.maxCount, Pool.LimitMode.Unlimited);
                    if(particlePools[i] == null)
                    {
                        request.fx.debugMsg?.Raise("ActiveHandle constructor for "+request.fx.name+". particle pool created is null!?");
                    }

                    //Debug.Log("pool getcreated" + particlePools[i]);
                }
                wasConstructed = true;
            }

            public override string ToString()
            {
                return  "SpecialEffect.ActiveHandle: " + request.fx.name + " Handle. "+(wasConstructed ? "Constructed " : "NOT Constructed!?") + startTime;
            }
        }

        [System.Serializable]
        public class Beat
        {
            public bool stopAudioOnSkip = true;
            public bool waitForPrev;
            [FormerlySerializedAs("t")]
            public float delay;
            public SoundEffect sfx;
            public ParticleSystem particlePrefab;
            public bool matchTargetRot = true;
        }

        [SerializeField]
        private float maxLifetime = 30f;
        [SerializeField]
        private int initCount = 1;
        [SerializeField]
        private int maxCount = 5;
        [SerializeField]
        private ParticleSystemStopBehavior particleStopBehaviour = ParticleSystemStopBehavior.StopEmitting;
        [Space]

        [SerializeField]
        private Beat[] beats = default;

        public float MaxLife => maxLifetime;
        public int BeatCount => beats.Length;
        public ParticleSystemStopBehavior ParticleStopBehaviour => particleStopBehaviour;

        private List<Pool> pools;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying) return;
            //precache the pools right away so they don't get created the first time they are used
            //hold onto a ref in the list so they don't disappear in memory before they are used
            pools = new List<Pool>();
            for (int i = 0; beats != null && i < beats.Length; i++)
            {
                Pool p = null;
                if (beats[i].particlePrefab != null)
                    p = Pool.GetCreatePool<ParticleSystem>(beats[i].particlePrefab);
                pools.Add(p);
            }
        }


        public Beat GetBeat(int index)
        {
            return beats[index];
        }

        public override void Raise()
        {
            Play();
        }

        public void Play()
        {
            Play(null, null);
        }

        public void Play(Vector3 pos)
        {
            Play(null, pos, null);
        }

        public void Play(Transform target)
        {
            Play(target, null);
        }

        public void Play(Transform target, System.Action<ActiveHandle> receiveHandleCallback)
        {
            Play(target, target == null ? Vector3.zero : target.position, receiveHandleCallback);
        }

        public void Play(System.Action<ActiveHandle> receiveHandleCallback)
        {
            Play(null, Vector3.zero, receiveHandleCallback);
        }

        public void Play(Transform target, Vector3 pos, System.Action<ActiveHandle> receiveHandleCallback)
        {
            Vector3 p = target == null ? pos : target.position;
            Play(new PlayRequest(this, p, target, receiveHandleCallback));
        }

        public void Play(PlayRequest request)
        {
                typedEvent.Invoke(request);
                untypedEvent.Invoke();
        }

        public override PlayRequest To(EVInstData bc) => (PlayRequest)bc.SystemObjectValue;
#if UNITY_EDITOR

        protected override void SetInitialvalueOfInstanceConfig(PlayRequest val, EventVars.EVInstData config) => config.SystemObjectValue = val;
#endif
    }
}