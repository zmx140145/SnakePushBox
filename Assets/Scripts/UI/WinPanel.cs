using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel :UIPanel
{
    public Text UseTimeText;
    public Text MapNumText;
    public Text BestTimeText;
    public void Start()
    {
        GameManager.Instance._GameMode = GameMode.EndGame;
        var time = GameManager.Instance.CurRemainGameTime;
        if (time < GameManager.Instance.GameMapMaxTime)
        {
            GameManager.Instance.GameMapMaxTime = time;
        }
        
        UseTimeText.text =string.Format(" {0:F2}", time)+"s";
        MapNumText.text = GameManager.Instance.CurRandomStateInGame.ToString();
        BestTimeText.text=string.Format(" {0:F2}", GameManager.Instance.GameMapMaxTime)+"s";
    }

    public void  ReturnStartPanel()
    {
        UIManager.Instance.ExitPanel(UIPanelType.Win);
    }
}
