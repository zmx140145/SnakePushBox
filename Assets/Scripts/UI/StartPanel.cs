using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : UIPanel
{
    public StartGameButton _StartGameButton;
    public WordAssistant _Assistant;
    public MapMesh _StartPanelMap;


    private void Start()
    {
        GameManager.Instance.HavePropCount = 5;
        GameManager.Instance.isStartReadyGame = false;
        SoundManager.Instance.PlayBGM(GlobalSounds.S_BGM1);
        _StartGameButton._StartPanel = this;
        _StartGameButton.ClickPointCount = 0;
        _StartGameButton.MaxClickCount = 4;
        GameManager.Instance.InitMap(0);
    }


    public override void OnRecovery()
    {
 
        if (GameManager.Instance._GameMode >= GameMode.GameReady)
        {
            GameManager.Instance.isStartReadyGame = false;
            GameManager.Instance.HavePropCount = 5;
            var curmap = GameManager.Instance._Map;
            if (curmap)
            {
                Destroy(curmap.gameObject);
                GameManager.Instance._Map = null;
            }

            if (GameManager.Instance._Snake.Count > 0)
            {
                foreach (var sn in GameManager.Instance._Snake)
                {
                    if (sn)
                        Destroy(sn.gameObject);
                }

                GameManager.Instance._Snake.Clear();
            }

            if (_StartPanelMap)
            {
                _StartPanelMap.gameObject.SetActive(true);
                GameManager.Instance._Map = _StartPanelMap;
                GameManager.Instance.CurSelectMap = _StartPanelMap.ThisMapIndex;
                GameObject s = Instantiate(GameManager.Instance._SnakePrefabs[0]);
                var _Snake = new List<SnakeMesh>() { s.GetComponent<SnakeMesh>() };
                _Snake[0]._Map = GameManager.Instance._Map;
                _Snake[0].SnakeSize = GameManager.Instance.SnakeSize;
                _Snake[0].Init(_StartPanelMap.StartPoint, GameManager.Instance.Size, 1);
                GameManager.Instance._Snake = _Snake;
                GameManager.Instance.InputActive = true;
                GameManager.Instance.IsGameStart = true;

                var map = GameManager.Instance._Map;
                //视角聚焦
                MainViewCameraController MainViewCamera =
                    Camera.main.transform.parent.GetComponent<MainViewCameraController>();
                // float ViewSize = Mathf.Max(map.GetMapLength / 2f, map.GetMapWidth / 2f)*1.8f;
                // MainViewCamera.SetViewTarget(map.transform.position+new Vector3(map.GetMapWidth/2f,2f,map.GetMapLength/2f),ViewSize);
                // MainViewCamera.SetStartPos(GameManager.Instance.CameraOriginalTransform);
                GameManager.Instance._GameMode = GameMode.TutorialStart;
            }
        }
        GameManager.Instance._GameMode = GameMode.TutorialStart;

        base.OnRecovery();
    }

    public void SayWord()
    {
        if (_Assistant != null)
        {
            _StartGameButton.SayWord();
        }
    }

    public void StartSetting()
    {
        UIManager.Instance.PushPanel(UIPanelType.Settiing);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void StartReadyGame()
    {
        //先关窗

        GameManager.Instance.InputActive = false;
        GameManager.Instance.StartReadyGame();
    }
}