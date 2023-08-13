using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscPanel : MonoBehaviour
{
    public Button _ReturnBtn;
    public Button _ExitBtn;
    public Button _FailBtn;
    public Button _SettingBtn;

    private void Start()
    {
        _ReturnBtn.onClick.AddListener(ReturnBtnClick);
        _ExitBtn.onClick.AddListener(ExitBtnClick);
        _FailBtn.onClick.AddListener(FailBtnClick);
        _SettingBtn.onClick.AddListener(SettingBtnClick);
    }

    private void OnEnable()
    {
        GameManager.Instance.IsCalculatrRemainGameTime = false;
    }

    private void OnDisable()
    {
        if(GameManager.Instance)
        GameManager.Instance.IsCalculatrRemainGameTime = true;
    }

    private void ReturnBtnClick()
    {
        gameObject.SetActive(false);
        GameManager.Instance.InputActive = true;
    }

    private void ExitBtnClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void FailBtnClick()
    {
        GameManager.Instance.InputActive = true;
        GameManager.Instance.GameOver1();
        gameObject.SetActive(false);
    }

    private void SettingBtnClick()
    {
        UIManager.Instance.PushPanel(UIPanelType.Settiing);
        gameObject.SetActive(false);
    }
}