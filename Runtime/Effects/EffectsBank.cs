//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;

namespace BB.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects + "Bank")]
    public class EffectsBank : ScriptableObject
    {
        [SerializeField]
        private SoundEffect[] soundEffectBank = default;
        [SerializeField]
        private SpecialEffect[] specialEffectBank = default;

        public int EffectCount => runtimeEffectBank.Count;
        public int SoundCount => runtimeSoundBank.Count;

        private List<SoundEffect> runtimeSoundBank;
        private List<SpecialEffect> runtimeEffectBank;

        public event System.Action<SoundEffect> onSfxAdded;
        public event System.Action<SoundEffect> onSfxRemoved;

        private void OnValidate()
        {
            for (int i = 0; i < EffectCount; i++)
            {
#if UNITY_EDITOR
                if (specialEffectBank[i] == null) UnityEditor.ArrayUtility.RemoveAt(ref specialEffectBank, i);
#endif
            }

            for (int i = 0; i < SoundCount; i++)
            {
#if UNITY_EDITOR
                if (soundEffectBank[i] == null) UnityEditor.ArrayUtility.RemoveAt(ref soundEffectBank, i);
#endif
            }
        }

        private void OnEnable()
        {
            runtimeSoundBank = new List<SoundEffect>(soundEffectBank);
            runtimeEffectBank = new List<SpecialEffect>(specialEffectBank);
        }

        public void Add(SoundEffect sfx)
        {
            if (sfx == null) Debug.LogError("SFX is null ? "+sfx);
            if (runtimeSoundBank.Contains(sfx)) return;
            runtimeSoundBank.Add(sfx);
            onSfxAdded?.Invoke(sfx);
        }

        public void Remove(SoundEffect sfx)
        {
            if (!runtimeSoundBank.Contains(sfx)) return;
            runtimeSoundBank.Remove(sfx);
            onSfxRemoved?.Invoke(sfx);
        }

        public SoundEffect GetSound(int index) => runtimeSoundBank[index];
        public SpecialEffect GetEffect(int index) => runtimeEffectBank[index];

    }
}