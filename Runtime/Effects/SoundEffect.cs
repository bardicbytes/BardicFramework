//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects + "Sound Effect")]
    public class SoundEffect : SimpleGenericEventVar<SoundEffect.PlayRequest>
    {
        [System.Serializable]
        public struct MusicLevelAdjustment
        {
            public int channel;
            public float volMult;
        }

        public const float DEFAULT_VOLUME = .75f;
        [System.Serializable]
        public class PlayRequest
        {
            public System.Action<ActiveHandle> receiveHandleCallback;
            public SoundEffect sfx;
            public float volume;
            public Transform target;
            public Vector3 pos;
            public bool twoD;
            public bool loop;
            public AudioClip clip;
            public float requestTime;

            //used by Unity Events from inspector
            public PlayRequest(SoundEffect sfx) : this(sfx, Vector3.zero, null, DEFAULT_VOLUME, true, false, null) { }
            public PlayRequest(SoundEffect sfx, Vector3 pos, Transform target, float volume, bool twoD, bool loop, System.Action<ActiveHandle> receiveHandleCallback)
            {
                this.requestTime = Time.time;
                this.clip = sfx.NextClip;
                this.loop = loop && !(sfx.isMusic && sfx.HasOutro);
                this.receiveHandleCallback = receiveHandleCallback;
                this.sfx = sfx;
                this.target = target;
                if (target != null)
                    this.pos = target.transform.position;
                else
                    this.pos = pos;
                this.twoD = twoD;
                this.volume = volume;
            }
        }

        public class ActiveHandle
        {
            public bool constructed;
            public PlayRequest request;
            public float realPlayTime;
            public AudioSource spawnedSource;

            public ActiveHandle(PlayRequest request, AudioSource source)
            {
                this.constructed = true;
                this.request = request;
                this.realPlayTime = Time.realtimeSinceStartup; ;
                this.spawnedSource = source;
            }

            public void StopAndRecycle()
            {
                spawnedSource.Stop();
                spawnedSource.gameObject.SetActive(false);
            }
        }

        public enum ClipSelectionMode { Random = 0, LoopInOrder = 1, RandomNonRepeat = 2, IntroLoopOutro = 3 }

        [SerializeField]
        private AudioClip[] clips = default;
        [SerializeField]
        private float volume = .75f;
        [SerializeField]
        private bool force2d = false;
        [SerializeField]
        private bool loop = false;
        [Space]
        [SerializeField]
        private bool isMusic = false;
        [SerializeField]
        private int autoIncrementAfterLoops = 2;
        [SerializeField]
        private int musicChannel = 0;
        [SerializeField]
        private MusicLevelAdjustment[] musicAdjustments = default;
        [Space]
        [SerializeField]
        private float randomPitchRange = 0;
        [SerializeField]
        private float pitchOffset = 0;
        [SerializeField]
        private ClipSelectionMode selectionMode = ClipSelectionMode.Random;
        [SerializeField]
        private float minInterval = 0;
        [SerializeField]
        [Range(0f, 1f)]
        private float startTime = 0f;

        private int indexPlayed = -1;
        private int loopCount = 0;
        private bool playedOutro = false;

        public bool DoNextLoop { get; set; } = false;
        public bool DoOutro { get; set; } = false;
        public bool HasOutro => selectionMode == ClipSelectionMode.IntroLoopOutro;
        public bool OutroNotPlayed => selectionMode != ClipSelectionMode.IntroLoopOutro || !playedOutro;
        public bool IsMusic => isMusic;
        public int MusicChannel => musicChannel;
        public float StartTime => startTime;
        public float RandomPitchRange => randomPitchRange;
        public float PitchOffset => pitchOffset;
        public MusicLevelAdjustment[] MusicAdjustments => musicAdjustments;

        protected override void OnValidate()
        {
            if (IsMusic && !force2d) force2d = true;
            if (IsMusic && clips != null && clips.Length > 1) selectionMode = ClipSelectionMode.IntroLoopOutro;
            DoNextLoop = false;
            DoOutro = false;
            playedOutro = false;
            indexPlayed = -1;
            loopCount = 0;
            base.OnValidate();
        }


        public override string ToString()
        {
            return name;
        }

        public override void Raise()
        {
            Play();
        }

        [ContextMenu("Play_PLAYMODE")]
        public void Play()
        {
            Play(null, Vector3.zero);
        }

        public void Play(System.Action<ActiveHandle> receiveHandleCallback)
        {
            Play(null, Vector3.zero, force2d, receiveHandleCallback);
        }

        public void Play(Vector3 pos)
        {
            Play(null, pos, false, null);
        }

        public void Play(Transform target)
        {
            Play(target, target == null ? Vector3.zero : target.position);
        }

        public void Play(Transform target, Vector3 pos)
        {
            Play(target, pos, force2d, null);
        }

        public void Play(Transform target, Vector3 pos, System.Action<ActiveHandle> receiveHandleCallback)
        {
            Play(target, pos, force2d, receiveHandleCallback);
        }

        public void Play(Transform target, Vector3 pos, bool twoD, System.Action<ActiveHandle> receiveHandleCallback)
        {
            Play(new PlayRequest(this, pos, target, volume, twoD || force2d, loop, receiveHandleCallback));
        }

        public void Play(PlayRequest request)
        {
            if (Time.realtimeSinceStartup <= lastRaiseTime + minInterval) return;
            //if (isMusic) Debug.Log("Playing Music "+name, this);
            lastRaiseTime = Time.realtimeSinceStartup;

            Initialize();

            if (selectionMode == ClipSelectionMode.IntroLoopOutro)
            {
                DoNextLoop = false;
                DoOutro = false;
                playedOutro = false;
                loopCount = 0;
                //indexPlayed = -1;
            }

            typedEvent.Invoke(request);
            untypedEvent.Invoke();
        }

        public AudioClip NextClip
        {
            get
            {
                if (selectionMode == ClipSelectionMode.RandomNonRepeat && clips.Length >= 2)
                {
                    int nextIndex = indexPlayed;
                    while (nextIndex == indexPlayed) nextIndex = Random.Range(0, clips.Length);
                    indexPlayed = nextIndex;
                }
                else if (selectionMode == ClipSelectionMode.LoopInOrder)
                {
                    if (indexPlayed + 1 == clips.Length) indexPlayed = 0;
                    else indexPlayed++;
                }
                else if (selectionMode == ClipSelectionMode.IntroLoopOutro)
                {
                    bool loopCountValid = this.autoIncrementAfterLoops > 0 && loopCount >= autoIncrementAfterLoops;
                    //not played OR played outro
                    if (indexPlayed == -1 || indexPlayed == clips.Length - 1)
                    {
                        //Debug.Log("starting " + name);
                        indexPlayed = 0;
                    }
                    else if (DoOutro)
                    {
                        //Debug.Log("ending " + name);
                        playedOutro = true;
                        DoOutro = false;
                        indexPlayed = clips.Length - 1;
                    }
                    else if (indexPlayed == 0 || ((DoNextLoop || loopCountValid) && indexPlayed >= 1 && indexPlayed < clips.Length - 2))
                    {
                        //played intro OR next loop is queued up
                        //Debug.Log("incrementing music loop "+name);
                        indexPlayed++;
                        DoNextLoop = false;
                        loopCount = 0;
                    }
                    else if ((DoNextLoop || loopCountValid) && indexPlayed == clips.Length - 2)
                    {
                        //Debug.Log("resetting music loop " + name);
                        indexPlayed = 1;
                        loopCount = 1;
                        DoNextLoop = false;
                    }
                    else
                    {
                        loopCount++;
                        //Debug.Log("music track staying "+indexPlayed+" "+name+"");
                    }
                }
                else
                {
                    indexPlayed = Random.Range(0, clips.Length);
                }

                return clips[indexPlayed];
            }
        }

        public void MusicReset()
        {
            DoNextLoop = false;
            DoOutro = false;
            playedOutro = false;
            indexPlayed = -1;
        }
        public override PlayRequest To(EVInstData bc) => (PlayRequest)bc.SystemObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(PlayRequest val, EventVars.EVInstData config)
        {
            config.SystemObjectValue = val;
        }
#endif
    }
}