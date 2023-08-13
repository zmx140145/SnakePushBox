using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPanel : UIPanel
{
   public Text UseTimeText;
   public Text MapNumText;

   public void Start()
   {
      GameManager.Instance._GameMode = GameMode.EndGame;
      var time = GameManager.Instance.CurRemainGameTime;
    
      UseTimeText.text =string.Format(" {0:F2}", time)+"s";
      MapNumText.text = GameManager.Instance.CurRandomStateInGame.ToString();
   }

   public void  ReturnStartPanel()
   {
      UIManager.Instance.ExitPanel(UIPanelType.End);
   }
}
