using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartGameButton :MonoBehaviour
{
    public StartPanel _StartPanel;
    public MMF_Player[] mmf_Playrs;
    public Text _Text;
    public Button _Button;
    public Color ClickColor;
    public Color NormalColor;
    public int ClickPointCount;
    public int MaxClickCount;
    public BetterOutline Eff1;
    public DepthEffect Eff2;
    private void Start()
    {
        ClickPointCount = 0;
        _Button.onClick.AddListener(OnClick);
    }


  
    public void OnClick()
    {
  
        if (mmf_Playrs.Length > 0)
        {
            if (ClickPointCount >= MaxClickCount)
            {
                mmf_Playrs[MaxClickCount-1].Initialization();
                mmf_Playrs[MaxClickCount-1].PlayFeedbacks();
            }
            else
            {
                mmf_Playrs[ClickPointCount].Initialization();
                mmf_Playrs[ClickPointCount].PlayFeedbacks();
            }
        }

        if (ClickPointCount == 0)
        {
            ClickPointCount = 1;
        }
    }

    public void SayWord()
    {
        if (_StartPanel._Assistant == null)
        {
            return;
        }
        if (ClickPointCount == 1)
        {
            ClickPointCount = 2;
        }
        WordMessage a = _StartPanel._Assistant.PlayState(_StartPanel._Assistant._PlayMode);
        if (a == null)
        {
            return;
        }
        
        if (a.ToState >= 2000)
        {
            ClickPointCount = 3;
        }
        else
        {
            if (a.ToState >= 1000 )
            {
                ClickPointCount = 2;
            }
            else
            {
                if (a.ToState >= 1)
                {
                    int i = a.ToState;
                    i= i * 1000;
                    if (ClickPointCount != 3)
                    {
                        //变更关卡到开始游戏
                        WordManager.Instance.NoticeOperaNewState(i);  
                        ClickPointCount = 3;
                    }
                }
                else
                {
                    //特殊处理
                }
         
            }
        }
       
    }
}
