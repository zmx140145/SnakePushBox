using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

public class GroundButton : Item, CanIn
{
    public bool IsClickDown = false;
    private bool lastClickState = false;
    public Action<bool> OnClickedListener;
    public GameObject Open;
    public GameObject Close;
    public bool isOneSwitch = false;
    public void SetClickState(bool state)
    {
        lastClickState = state;
        if (isOneSwitch)
        {
            if (lastClickState)
            {
                IsClickDown = IsClickDown ? false : true;
                OnClickedListener?.Invoke(IsClickDown);
                if (IsClickDown)
                {
                
                
                
                    Open.SetActive(true);
                    Close.SetActive(false);

                    SoundManager.Instance.PlaySound(GlobalSounds.S_DoorButtonIn);
                    // MMSoundManager.Instance.PlaySound(GlobalSounds.S_DoorButtonIn.GetSound(), GlobalSounds.GetMMSoundOption(this.transform,1f));
                }
                else
                {
                    SoundManager.Instance.PlaySound(GlobalSounds.S_DoorButtonOut);
                    Open.SetActive(false);
                    Close.SetActive(true);
                }
            }
        }
        else
        {
            
            //普通开关
            if (IsClickDown != lastClickState)
            {
                IsClickDown = lastClickState;
                    OnClickedListener?.Invoke(IsClickDown);
                    if (IsClickDown)
                    {
                
                
                
                        Open.SetActive(true);
                        Close.SetActive(false);

                        SoundManager.Instance.PlaySound(GlobalSounds.S_DoorButtonIn);
                        // MMSoundManager.Instance.PlaySound(GlobalSounds.S_DoorButtonIn.GetSound(), GlobalSounds.GetMMSoundOption(this.transform,1f));
                    }
                    else
                    {
                        SoundManager.Instance.PlaySound(GlobalSounds.S_DoorButtonOut);
                        Open.SetActive(false);
                        Close.SetActive(true);
                    }
                
            
         
            }  
        }
        
        
       
    }

    private bool lastItemInThisPos = false;
    public void CheckObj(MapMesh map)
    {
        Item[] its = map.FindItem(CurPos);
        bool a = false;
        foreach (var it in its)
        {
            if (it is Box)
            {
                a = true;
                if (!lastItemInThisPos )
                {
                    lastItemInThisPos =true;
                    SetClickState(true);
                }
               
            }
        }

        foreach (var it in GameManager.Instance._Snake)
        {
            if (it.IsHaveSelfPos(CurPos))
            {
                a = true;
                if (!lastItemInThisPos)
                {
                    lastItemInThisPos = it;
                    SetClickState(true);
                }
            }
        }
        if (!a)
        {
            lastItemInThisPos =false;
            SetClickState(false);
        }
   
    }
    public void CheckObj()
    {
        Item[] its = GameManager.Instance._Map.FindItem(CurPos);
        bool a = false;
        foreach (var it in its)
        {
            if (it is Box)
            {
                a = true;
                if (!lastItemInThisPos )
                {
                    lastItemInThisPos =true;
                    SetClickState(true);
                }
               
            }
        }

        foreach (var it in GameManager.Instance._Snake)
        {
            if (it.IsHaveSelfPos(CurPos))
            {
                a = true;
                if (!lastItemInThisPos)
                {
                    lastItemInThisPos = it;
                    SetClickState(true);
                }
            }
        }
        if (!a)
        {
            lastItemInThisPos =false;
            SetClickState(false);
        }
   
    }

    public override void Init()
    {
    }

    public override bool CanMoveIn(object[] args)
    {
        return true;
    }

    public override bool CanDestory(int type)
    {
        return false;
    }

    public override MoveEffecState GetMoveEffect(object[] args)
    {
        if ((bool)args[0])
        {
            RoleMoveIn((SnakeMesh)args[1]);
        }

        return MoveEffecState.NoEffect;
    }


    public void CheckRole(SnakeMesh _role)
    {
        // if (!_role.IsHaveSelfPos(CurPos))
        // {
        //     isNeedSetState = true;
        //    Item[] its= GameManager.Instance._Map.FindItem(CurPos);
        //    foreach (var it in its)
        //    {
        //        if (it is Box)
        //        {
        //            isNeedSetState = false;
        //        }
        //    }
        //
        //    foreach (var sn in GameManager.Instance._Snake)
        //    {
        //        if (sn.IsHaveSelfPos(CurPos))
        //        {
        //            isNeedSetState = false;
        //        }
        //    }
        //    
        //     RoleMoveOut(_role);
        // }
    }

    private bool isNeedSetState = false;

    public void RoleMoveIn(SnakeMesh _role)
    {
        // if (!IsClickDown)
        // {
        //     if(isNeedSetState)
        //     SetClickState(true);
        //    
        // }
        // _role.MoveEffectAction += CheckRole;
    }

    public void RoleMoveOut(SnakeMesh _role)
    {
        // Debug.Log("退出");
        // if (IsClickDown)
        // {
        //     if(isNeedSetState)
        //     SetClickState(false);
        //     
        // }
        // if (_role.MoveEffectAction != null)
        // _role.MoveEffectAction -= CheckRole;
    }
}