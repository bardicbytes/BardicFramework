using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsSlider : MonoBehaviour
{
    public Slider slider;

    public string playerPrefsKey = "asjihdfaikda";

    private void OnValidate()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(playerPrefsKey));
        }
        slider.onValueChanged.AddListener(HandleChange);
        slider.value = QualitySettings.GetQualityLevel();
    }

    private void HandleChange(float arg)
    {
        int v = Mathf.RoundToInt(arg);
        QualitySettings.SetQualityLevel(v, true);
        PlayerPrefs.SetInt(playerPrefsKey, v);
    }

}
