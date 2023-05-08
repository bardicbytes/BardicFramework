// alex@bardicbytes.com

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace BardicBytes.BardicFramework
{
    public class SliderMixerControl : MonoBehaviour
    {
        [SF] private string playerPrefKey = "volKey0";
        [SF] private AudioMixer mixer = default;
        [SF] private Slider slider = default;
        [SF] private string mixerParam = "MasterVolume";
        [SF] private float multiplier = 32;

        private void OnValidate()
        {
            if (slider == null) slider = GetComponent<Slider>();
        }

        private void Awake()
        {
            slider.onValueChanged.AddListener(HandleSliderChange);
        }

        private void OnDisable()
        {
            PlayerPrefs.SetFloat(playerPrefKey, slider.value);
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void HandleSliderChange(float val)
        {
            //Debug.Log(name + " HandleSliderChange "+ val);
            SetMixerVol(val);
        }

        public void Refresh()
        {
            float v = PlayerPrefs.GetFloat(playerPrefKey, 1);
            
            //Debug.Log(name + " Refresh " + v);

            slider.value = v;
            SetMixerVol(v);
        }

        private void SetMixerVol(float val)
        {
            var v = Mathf.Approximately(0, val) ? -80 : (Mathf.Log10(val) * multiplier);
            //Debug.Log(name + " SetMixerVol db:" + v);
            mixer.SetFloat(mixerParam, v);
        }
    }
}