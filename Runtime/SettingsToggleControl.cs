// alex@bardicbytes.com

using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace BardicBytes.BardicFramework
{
    public class SettingsToggleControl : MonoBehaviour
    {
        [SF] private string playerPrefKey = "toggle0";
        [SF] private Toggle toggle = default;

        private void OnValidate()
        {
            if (toggle == null) toggle = GetComponent<Toggle>();
        }
        private void Awake()
        {
            toggle.onValueChanged.AddListener(HandleToggleChange);
        }

        private void OnDisable()
        {
            PlayerPrefs.SetInt(playerPrefKey, toggle.isOn ? 1 : 0);
        }

        private void OnEnable()
        {
            toggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(playerPrefKey, 1) > 0);
        }

        private void HandleToggleChange(bool val)
        {
            float v = PlayerPrefs.GetFloat(playerPrefKey, 1);
            PlayerPrefs.SetInt(playerPrefKey, val ? 1 : 0);
        }
    }
}