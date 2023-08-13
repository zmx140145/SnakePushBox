using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : UIPanel
{
    public ItemCard[] _ItemCards;
    public Transform LastPos, FirstPos, StartPos;
    public AnimationCurve MoveAnimCurve;
    public Text _MapText;
    public Text _TimeText;
    private MainViewCameraController _ViewController;

    private void Update()
    {
        _MapText.text = $"关卡{GameManager.Instance.CurRandomStateInGame + 1}";
        _TimeText.text= "用时:"+string.Format(" {0:F2}", GameManager.Instance.CurRemainGameTime)+"s";
    }

    private void Start()
    {
        GameManager.Instance.CurRemainGameTime = 0f;
        _ViewController=Camera.main.transform.parent.GetComponent<MainViewCameraController>();
        GameManager.Instance._GameMode = 3000;
        DisplayCard();
        InitGame();
        
    }

    public override void OnEnter()
    {
        GameManager.Instance.IsCalculatrRemainGameTime = true;
        base.OnEnter();
    }

    public override void OnPause()
    {
        GameManager.Instance.IsCalculatrRemainGameTime = false;
        base.OnPause();
    }

    public override void OnRecovery()
    {
        
        GameManager.Instance.IsCalculatrRemainGameTime = true;
        base.OnRecovery();
    }

    public override void OnExit()
    {
        GameManager.Instance.IsCalculatrRemainGameTime = false;
        base.OnExit();
    }
    
    
    
    public void InitGame()
    {
        //开始游戏 并且设置摄像机位置
        int index= GameManager.Instance.RandomSelectMapIndex[0];
        GameManager.Instance.CurRandomStateInGame = 0;
        GameManager.Instance.MapStartSnakeLength = 1;
       GameManager.Instance.InitMap(index);
       var map=GameManager.Instance._Map;
       float ViewSize = Mathf.Max(map.GetMapLength / 2f, map.GetMapWidth / 2f)*1.8f;
       _ViewController.SetViewTarget(map.transform.position+new Vector3(map.GetMapWidth/2f,2f,map.GetMapLength/2f),ViewSize);
    }
    public void DisplayCard()
    {
        if (_ItemCards.Length == 0)
        {
            return;
        }

        float space = (LastPos.position.y - FirstPos.position.y) / (_ItemCards.Length - 1);
        for (int i = 0; i < _ItemCards.Length; i++)
        {
            string Key = "", illustrate = "";
            Sprite sprite = null;
            int CurCount=0;
            switch (i)
            {
                case 0:
                    sprite = Resources.Load<Sprite>("Picture/Cut");
                    Key = "C";
                    illustrate = "分裂";
                    CurCount = GameManager.Instance.CutCount;
                    break;
                case 1:
                    sprite = Resources.Load<Sprite>("Picture/Increase");
                    Key = "V";
                    illustrate = "伸长";
                    CurCount = GameManager.Instance.IncreaseCount;
                    break;
                case 2:
                    sprite = Resources.Load<Sprite>("Picture/Remove");
                    Key = "F";
                    illustrate = "缩短";
                    CurCount = GameManager.Instance.RemoveCount;
                    break;
                case 3:
                    sprite = Resources.Load<Sprite>("Picture/Restart");
                    Key = "R";
                    illustrate = "重启";
                    CurCount = GameManager.Instance.RestartCount;
                    break;
                case 4:
                    sprite = Resources.Load<Sprite>("Picture/Reverse");
                    Key = "E";
                    illustrate = "反转";
                    CurCount = GameManager.Instance.ReverseCount;
                    break;
            }

            _ItemCards[i].InitCard(MoveAnimCurve, StartPos.position,
                new Vector3(FirstPos.position.x , FirstPos.position.y+ space * i, transform.position.z), Key,
                illustrate, CurCount , sprite);
        }
    }
    
}
