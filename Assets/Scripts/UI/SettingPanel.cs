using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : UIPanel
{
    public Slider _Slider;
    public Slider _SliderBGM;
    public Button _ExitButton;

    private void Update()
    {
        if (InputManager.Instance.GetEscDown)
        {
            Exit();
        }
    }

    private void Start()
    {
        _Slider.onValueChanged.AddListener(OnVolumeChange);
        _SliderBGM.onValueChanged.AddListener(OnVolumeChangeBGM);
        _ExitButton.onClick.AddListener(Exit);
    }

    private void Exit()
    {
        GameManager.Instance.InputActive = true;
        UIManager.Instance.ExitPanel(UIPanelType.Settiing);
    }
    private void OnEnable()
    {
        GameManager.Instance.InputActive = false;
        _Slider.value = SoundManager.Instance.CurSoundVolume;
    }

    private void OnVolumeChange(float value)
    {
        SoundManager.Instance.CurSoundVolume = value;
    }
    private void OnVolumeChangeBGM(float value)
    {
        SoundManager.Instance.CurBGMVolume = value;
    }
}
