using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UIPanelType{
    Start,
    Menu,
    Game,
    Win,
    Settiing,
    End,
}
[Serializable]
public abstract class UIPanel : MonoBehaviour
{
    public bool isActive=false;
    [SerializeField]protected bool isKeep;
    [SerializeField] private UIPanelType uiPanelType;
    public UIPanelType GetUIPanelType
    {
        get{return uiPanelType;}
    }
    public virtual void OnEnter()
    {
  
        Debug.Log ("进入"+uiPanelType.ToString());
        isActive=true;
        gameObject.SetActive(true);
        OnStart();
      
    }
    public virtual void OnPause()
    {
        Debug.Log ("暂停"+uiPanelType.ToString());
        isActive=false;
        gameObject.SetActive(false);
     
    }
    public virtual void OnRecovery()
    {

        Debug.Log ("恢复"+uiPanelType.ToString());
        isActive=true;
        gameObject.SetActive(true);
        OnStart();
    }
    public virtual void OnExit()
    {
        Debug.Log ("退出"+uiPanelType.ToString());
        isActive=false;
        Destroy(gameObject);
    }
    protected virtual void OnStart()
    {

    }
  
}