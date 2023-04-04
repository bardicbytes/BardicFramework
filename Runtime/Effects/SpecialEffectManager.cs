//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SF = UnityEngine.SerializeField;

namespace BardicBytes.BardicFramework.Effects
{
    public class SpecialEffectManager : MonoBehaviour
    {
        public const string DebugMusicOff_Key = "debugMusicOff";
        public static bool debugMusicOff = false;

        [SF] private AudioSource sfxSourcePrefab = default;
        [SF] private AudioSource musicSourcePrefab = default;

        [SF] private SFXHandleEventVar musicSourceChangeEventVar = default;

        [SF] private IntEventVar stopMusicChannelEv = default;

        [SF] private EffectsBank bank = default;

        [SF] private SpecialEffectHandleEvent skipBeat = default;
        [SF] private float min2DBlend = .25f;
        [SF] private int musicChannels = 4;
        [SF] private float fadeOutDur = 1f;

        [SF] private StringEventVar debugMsg = default;

        private Pool sfxSourcePool;
        private Pool musicSourcePool;

        private SoundEffect.PlayRequest queuedRequest = default;
        private bool hasQueuedRequest = false;

        private List<AudioSource> spawnedSources;
        private List<SoundEffect.ActiveHandle> spawnedSfxHandles;
        private List<SpecialEffect.ActiveHandle> activeFX;
        private Dictionary<SoundEffect, int> lastPlay;
        private SoundEffect.ActiveHandle[] musicHandles;

        private void Awake()
        {
            bank.Initialize();
            lastPlay = new Dictionary<SoundEffect, int>();
            spawnedSources = new List<AudioSource>();
            spawnedSfxHandles = new List<SoundEffect.ActiveHandle>();
            activeFX = new List<SpecialEffect.ActiveHandle>();
            musicHandles = new SoundEffect.ActiveHandle[musicChannels];

            stopMusicChannelEv?.AddListener(HandleStopMusicChannel);

            bank.onSfxAdded += HandleOnSFXAdded;
            bank.onSfxRemoved += HandleOnSFXRemoved;

            sfxSourcePool = Pool.GetCreatePool(sfxSourcePrefab, 20, 0, Pool.LimitMode.Unlimited);
            musicSourcePool = Pool.GetCreatePool(musicSourcePrefab, 1, musicChannels, Pool.LimitMode.Ignore);

            for (int i = 0; i < bank.SoundCount; i++)
            {
                bank.GetSound(i).AddListener(HandlePlayRequest);
            }

            skipBeat?.AddListener(HandleBeatSkipRequest);

            for (int i = 0; i < bank.EffectCount; i++)
            {
                bank.GetEffect(i).AddListener(HandlePlayRequest);
            }
        }


        private void HandleOnSFXRemoved(SoundEffect sfx)
        {
            sfx.RemoveListener(HandlePlayRequest);
        }

        private void HandleOnSFXAdded(SoundEffect sfx)
        {
            sfx.AddListener(HandlePlayRequest);
        }

        private void Update()
        {

            UpdateSFX();
            UpdateMusic();

            for (int i = 0; i < activeFX.Count; i++)
            {
                UpdateActiveFX(i);
            }
        }

        private void UpdateSFX()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UpdateSFX");

            for (int i = 0; i < spawnedSources.Count; i++)
            {
                if (spawnedSources[i].isPlaying) continue;
                var sfx = spawnedSfxHandles[i].request.sfx;

                spawnedSources[i].clip = null;
                spawnedSources[i].gameObject.SetActive(false);
                spawnedSfxHandles.RemoveAt(i);
                spawnedSources.RemoveAt(i);
                i--;
                //todo: invoke audio complete callback
            }
            UnityEngine.Profiling.Profiler.EndSample();

        }
        private bool hackmutemusic = false;
        private void UpdateMusic()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UpdateMusic");
            bool hasMusic = false;
            for (int i = 0; i < musicHandles.Length; i++)
            {
                if (musicHandles[i] == null) continue;
                hasMusic |= true;
                if (musicHandles[i].spawnedSource != null)
                {
                    musicHandles[i].spawnedSource.mute = hackmutemusic;
                }
            }
            if (!hasMusic) return;
#if UNITY_EDITOR
            if (Time.frameCount % 100 == 0)
                debugMusicOff = UnityEditor.EditorPrefs.GetBool(DebugMusicOff_Key);
#endif

            //iterate over the fixed number of music channels
            for (int i = 0; i < musicHandles.Length; i++)
            {

                //the queued request is for THIS channel, and the channel is open
                //todo: make this "musicHandles[i].spawnedSource == null || !musicHandles[i].spawnedSource.isPlaying" bit more reusable. it's in like 3 places
                if (hasQueuedRequest && queuedRequest.sfx.MusicChannel == i
                    && (musicHandles[i].spawnedSource == null || !musicHandles[i].spawnedSource.isPlaying))
                {
                    this.HandlePlayRequest(queuedRequest);
                    hasQueuedRequest = false;
                }

                if (musicHandles[i].spawnedSource == null || musicHandles[i].spawnedSource.isPlaying) continue;
                var sfx = musicHandles[i].request.sfx;


                if (sfx.HasOutro && sfx.OutroNotPlayed)
                {
                    //Debug.Log(i+"th channel. "+sfx.name+" outro not yet played. next clip = "+ sfx.NextClip);
                    musicHandles[i].spawnedSource.clip = sfx.NextClip;
                    //Debug.Log("Play 119 " + sfx.name);
                    musicHandles[i].spawnedSource.Play();
                    continue;
                }
                else if (sfx.HasOutro && !sfx.OutroNotPlayed)
                {
                    //Debug.Log(i + "th channel. " + sfx.name + " outro played. cleaning up");
                    musicHandles[i].request.sfx.MusicReset();
                    musicHandles[i].spawnedSource.clip = null;
                    musicHandles[i].spawnedSource.gameObject.SetActive(false);
                    musicHandles[i] = default;
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

        }

        public void UpdateActiveFX(int fxIndex)
        {
            UnityEngine.Profiling.Profiler.BeginSample("UpdateActiveFX " + fxIndex);

            SpecialEffect.ActiveHandle handle = activeFX[fxIndex];

            bool allFired = true;
            bool allComplete = true;
            SpecialEffect fx = handle.request.fx;
            //go over every beat.
            //update what's been fired
            //fire everything can can be fired
            for (int beatIndex = 0; beatIndex < fx.BeatCount; beatIndex++)
            {
                SpecialEffect.Beat b = fx.GetBeat(beatIndex);

                //if the beat's been fired
                if (handle.beatFired[beatIndex] && !handle.beatComplete[beatIndex])
                {
                    UpdateBeat(handle, beatIndex);
                }
                else if (!handle.beatFired[beatIndex]
                        && Time.time >= handle.startTime + b.delay
                        && (beatIndex == 0 || /*we don't have to wait OR prev beat is complete*/(!b.waitForPrev || handle.beatComplete[beatIndex - 1])))
                {
                    //Debug.Log("f" + Time.frameCount + ". t" + Time.time + ". BeatFired: " + beatIndex + " of " + fx.name);
                    PlayBeat(handle, beatIndex);
                }

                allComplete &= handle.beatComplete[beatIndex];
                allFired &= handle.beatFired[beatIndex];
            }

            //every beat has fired and their sounds or particles have stopped or finished... or the timeout lapsed
            if (allComplete || Time.time >= handle.startTime + fx.MaxLife)
            {
                for (int j = 0; j < handle.spawnedParticles.Length; j++)
                {
                    if (handle.spawnedParticles[j] == null) continue;
                    handle.spawnedParticles[j].gameObject.SetActive(false);
                }
                //Debug.Log("Removing handle "+handle.request.fx.name);
                activeFX.Remove(handle);
            }
            UnityEngine.Profiling.Profiler.EndSample();

        }

        private void UpdateBeat(SpecialEffect.ActiveHandle handle, int beatIndex)
        {
            if (handle.beatComplete[beatIndex]) return;

            SpecialEffect fx = handle.request.fx;
            SpecialEffect.Beat b = fx.GetBeat(beatIndex);

            var ps = handle.spawnedParticles[beatIndex];
            bool hasParticles = ps != null;
            bool hasNoPlayingParticles = !hasParticles || !ps.isPlaying && ps.particleCount == 0;
            bool hasPlayingParticles = (hasParticles && handle.spawnedParticles[beatIndex].isPlaying);

            if (hasPlayingParticles && handle.request.HasInactiveTarget)
            {
                //stop particles if the target they are tracking is gone
                ps.Stop(true, fx.ParticleStopBehaviour);
            }
            else if (hasPlayingParticles)
            {
                Vector3 p = handle.request.Pos;
                Quaternion r = b.matchTargetRot || b.particlePrefab == null ? handle.request.Rot : b.particlePrefab.transform.rotation;
                //the target is active, update the transform
                ps.transform.position = p;
                ps.transform.rotation = r;
            }

            //recycle finished particles
            if (hasParticles && hasNoPlayingParticles)
                ps.gameObject.SetActive(false);

            bool hasNoPlayingSound = handle.soundEffectHandles[beatIndex] == null || handle.soundEffectHandles[beatIndex].spawnedSource == null || !handle.soundEffectHandles[beatIndex].spawnedSource.isActiveAndEnabled;
            if (hasNoPlayingParticles && hasNoPlayingSound)
            {
                //Debug.Log("beat complete! " + handle.request.fx.name + ", beat " + beatIndex);
                handle.beatComplete[beatIndex] = true;
            }
        }

        private void PlayBeat(SpecialEffect.ActiveHandle handle, int beatIndex)
        {
            //debugMsg?.Raise("PlayBeat "+handle.request.fx.name+". beat "+beatIndex);
            SpecialEffect fx = handle.request.fx;
            SpecialEffect.Beat b = fx.GetBeat(beatIndex);

            handle.beatFired[beatIndex] = true;
            Debug.Assert(handle.spawnedParticles[beatIndex] == null);

            if (handle.particlePools[beatIndex] != null)
            {
                Quaternion r = b.matchTargetRot || b.particlePrefab == null ? handle.request.Rot : b.particlePrefab.transform.rotation;

                handle.spawnedParticles[beatIndex] = handle.particlePools[beatIndex].Spawn<ParticleSystem>(handle.particlePools[beatIndex].transform, handle.request.Pos, r);
            }
            else if (handle.particlePools[beatIndex] == null && b.particlePrefab != null)
            {
                debugMsg?.Raise(handle + " PlayBeat " + beatIndex + " has no particle pool, but has a prefab! " + b.particlePrefab);
            }

            if (b.sfx != null)
            {
                b.sfx.Play(handle.request.target, handle.request.Pos,
                   (SoundEffect.ActiveHandle sfxHandle) =>
                   {
                       handle.soundEffectHandles[beatIndex] = sfxHandle;
                   });
            }
        }


        private void HandleStopMusicChannel(int index)
        {
            var h = musicHandles[index];
            if (h.request.sfx == null || h.spawnedSource == null) return;

            //Debug.Log("HandleStopMusicChannel "+index+" "+h.request.sfx.name);

            for (int i = 0; i < h.request.sfx.MusicAdjustments.Length; i++)
            {
                var a = h.request.sfx.MusicAdjustments[i];
                var handleToAdj = musicHandles[a.channel];
                //if (handleToAdj.spawnedSource == null) continue;
                handleToAdj.spawnedSource.volume /= a.volMult;
            }
            if (h.request.sfx.HasOutro && h.request.sfx.OutroNotPlayed)
            {
                //Debug.Log("HandleStopMusicChannel --- do outro " + h.request.sfx.name);
                //h.request.sfx.DoOutro = true; //this doesn't make sense cause we release the handle in the next few lines
                //musicHandles[index] = h;
            }
            else if (!h.request.sfx.HasOutro)
            {
                //Debug.Log("stopping music-no outro: "+h.request.sfx.name);
                //h.spawnedSource.Stop();
                //h.spawnedSource.clip = null;
                //h.spawnedSource.gameObject.SetActive(false);
            }
            StartCoroutine(FadeMusicOut(h.spawnedSource));
            musicHandles[index] = default;
        }

        private IEnumerator FadeMusicOut(AudioSource source)
        {
            //Debug.Log("fading out: " + source.clip.name);
            float startTime = Time.realtimeSinceStartup;
            float endTime = Time.realtimeSinceStartup + fadeOutDur;
            float sv = source.volume;
            while (Time.realtimeSinceStartup < endTime)
            {
                yield return null;
                float t = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / fadeOutDur);
                source.volume = Mathf.Lerp(sv, 0, t);
            }
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
        }

        private void HandlePlayRequest(SpecialEffect.PlayRequest request)
        {
            if (!enabled) return;

            var handle = new SpecialEffect.ActiveHandle(request);
            activeFX.Add(handle);
            if (request.receiveHandleCallback != null)
                request.receiveHandleCallback.Invoke(handle);
        }

        private void HandlePlayRequest(SoundEffect.PlayRequest request)
        {
            if (!enabled) return;

            // requested music track is already playing
            if (request.sfx.IsMusic && request.sfx == musicHandles[request.sfx.MusicChannel].request.sfx)
            {
                //this requested music is playing
                //Debug.Log("handle play request...doNextLoop "+request.sfx.name);
                //Debug.Log("HandlePlayRequest, already playing, doing next loop" + request.sfx.name);
                musicHandles[request.sfx.MusicChannel].request.sfx.DoNextLoop = true;
                return;
            }
            // music track is not yet playing
            else if (request.sfx.IsMusic)
            {
                HandleStopMusicChannel(request.sfx.MusicChannel);

                //the requested channel has music playing, transitioning, or fading out
                //queue the request.
                if (musicHandles[request.sfx.MusicChannel].spawnedSource != null
                    && musicHandles[request.sfx.MusicChannel].spawnedSource.isPlaying)
                {
                    //Debug.Log("HandlePlayRequest, queueing" + request.sfx.name);
                    //queue up
                    //Debug.Log("Queuing next track " + request.sfx.name);
                    queuedRequest = request;
                    hasQueuedRequest = true;
                    return;
                }

                for (int i = 0; i < request.sfx.MusicAdjustments.Length; i++)
                {
                    var a = request.sfx.MusicAdjustments[i];
                    var handleToAdj = musicHandles[a.channel];
                    if (handleToAdj.spawnedSource == null) continue;
                    handleToAdj.spawnedSource.volume *= a.volMult;
                }

            }

            bool isOverplay = false;
            if (!lastPlay.ContainsKey(request.sfx))
            {
                lastPlay.Add(request.sfx, Time.frameCount);
            }
            else if (Time.frameCount == lastPlay[request.sfx])
            {
                isOverplay = true;
            }
            else
            {
                lastPlay[request.sfx] = Time.frameCount;
            }

            float volume = request.volume;

#if UNITY_EDITOR
            debugMusicOff = UnityEditor.EditorPrefs.GetBool(DebugMusicOff_Key);
#endif
            bool doDebugMute = debugMusicOff && Debug.isDebugBuild && request.sfx.IsMusic;



            if (doDebugMute) volume = 0f;
            else if (isOverplay) volume *= .1f;

            Pool pool = request.sfx.IsMusic ? musicSourcePool : sfxSourcePool;
            AudioSource s = pool.Spawn<AudioSource>(request.pos);
            if (s != null)
            {
                s.clip = request.clip;
                s.volume = volume;
                s.loop = request.loop;
                s.time = s.clip.length * request.sfx.StartTime;
                s.spatialBlend = request.twoD ? 0 : min2DBlend;
                s.pitch = 1 - request.sfx.RandomPitchRange / 2f + UnityEngine.Random.Range(0, request.sfx.RandomPitchRange) + request.sfx.PitchOffset;
                if (!request.sfx.IsMusic) spawnedSources.Add(s);
                //if(Debug.isDebugBuild && request.sfx.IsMusic) Debug.LogFormat("Play {0}, outro? {1}", request.sfx.name, request.sfx.HasOutro);
                s.Play();
            }
            SoundEffect.ActiveHandle sfx_handle = new SoundEffect.ActiveHandle(request, s);


            if (request.sfx.IsMusic)
            {
                musicHandles[request.sfx.MusicChannel] = sfx_handle;
                if (request.sfx.MusicChannel == 0) musicSourceChangeEventVar.Raise(sfx_handle);
            }
            else
            {
                spawnedSfxHandles.Add(sfx_handle);
            }
            if (request.receiveHandleCallback != null) request.receiveHandleCallback.Invoke(sfx_handle);
        }

        private void HandleBeatSkipRequest(SpecialEffect.ActiveHandle handle)
        {
            if (!handle.wasConstructed)
            {
                Debug.LogWarning("Avoid this: non constructed skip beat request reaised for handle \n" + handle);
                return;
            }
            //single beat? just stop it
            try
            {
                if (handle.request.fx.BeatCount == 1)
                {
                    handle.spawnedParticles[0]?.Stop(true, handle.request.fx.ParticleStopBehaviour);
                    handle.soundEffectHandles[0].spawnedSource?.Stop();
                    return;
                }
            }
            catch (NullReferenceException nre)
            {
                Debug.LogError(nre);
            }


            //multiple beats, skip should end looping audio/fx
            for (int beatIndex = 0; beatIndex < handle.request.fx.BeatCount; beatIndex++)
            {
                //skip over beats that haven't fired or are completed
                if (!handle.beatFired[beatIndex] || handle.beatComplete[beatIndex])
                {
                    //Debug.Log("3 ~~~~~~~~~~~~~~~~~ " + "skipping beat " + beatIndex);
                    continue;
                }

                //Debug.Log("f"+Time.frameCount +", t"+ Time.time+". SkipBeat! "+handle.request.fx.name+", beat "+beatIndex);
                //mark beat as complete
                handle.beatComplete[beatIndex] = true;

                //stop any spawned particles
                if (handle.spawnedParticles[beatIndex] != null)
                {
                    //Debug.Log("stopping particles at index "+i);
                    handle.spawnedParticles[beatIndex].Stop(true);
                    if (handle.request.fx.ParticleStopBehaviour == ParticleSystemStopBehavior.StopEmittingAndClear)
                        handle.spawnedParticles[beatIndex].Clear();
                }

                //stop any soundeffects
                if (handle.soundEffectHandles[beatIndex].spawnedSource != null &&
                    (handle.request.fx.GetBeat(beatIndex).stopAudioOnSkip || handle.soundEffectHandles[beatIndex].spawnedSource.loop))
                {
                    handle.soundEffectHandles[beatIndex].spawnedSource.Stop();
                }

                //stop looping, we only do this once if at all
                break;
            }

        }
    }
}