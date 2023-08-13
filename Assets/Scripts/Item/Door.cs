using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;


public class Door : Item,CanIn
{
    public GameObject Open;
    public GameObject Close;
   
    public bool IsOpen = false;
 
    private List<GroundButton> _Button=new List<GroundButton>();
    public Dir[] AcceptDir;
    public bool IsButtonOneToOne = false;
    private void OnDoorButtonClick(bool state)
    {
        if (IsButtonOneToOne)
        {
            //任意按钮都能开门
            if (state == false)
            {
                bool a = false;
                for (int i = 0; i < _Button.Count; i++)
                {
                    if (_Button[i].IsClickDown)
                    {
                        a = true;
                    }
              
                }
                if (!a)
                {
                    if (IsOpen)
                    {
                        IsOpen = state;
                        ChangeMapAndState(state);
                    }
                  
                }
               
            }
            else
            {
                if (!IsOpen)
                {
                    IsOpen = state;
                    ChangeMapAndState(state); 
                }
               
            }
        }
        else
        {
            //按钮多对一个门
            if (state == false)
            {
                if (IsOpen)
                {
                    IsOpen = state;
                    ChangeMapAndState(state);
                }
            }
            else
            {
                bool a = true;
                for (int i = 0; i < _Button.Count; i++)
                {
                    if (!_Button[i].IsClickDown)
                    {
                        a = false;
                    }
              
                }
                if (a)
                {
                    if (!IsOpen)
                    {
                        IsOpen = state;
                        ChangeMapAndState(state);
                    }
                    
                }
            }
        }
       
        
    }

    private bool isCancel = false;
    private void ChangeMapAndState(bool state)
    {
        if (state)
        {
           Open.SetActive(true);
           Close.SetActive(false);
           SoundManager.Instance.PlaySound(GlobalSounds.S_MetalDoorOpen);
           isCancel = true;
        }
        else
        {
      
            Close.SetActive(true);
            Open.SetActive(false);
            SoundManager.Instance.PlaySound(GlobalSounds.S_MetalDoorClose);
            if (!IsEffectRun)
            {
                isCancel = false;
                StartCoroutine("IsToCutSnakeWait");
            }
            
        }
        
    }

    public IEnumerator IsToCutSnakeWait()
    {
        IsEffectRun = true;
        yield return new WaitForFixedUpdate();
        if (!isCancel)
        {
            StartCoroutine("CutSnakeAndDestroyObj");
        }
      
    }
    public IEnumerator CutSnakeAndDestroyObj()
    {
      
        yield return new WaitUntil(() => { return GameManager.Instance.IsCanDoEffect; });
        SnakeMesh snake=null;
        foreach (var sn in GameManager.Instance._Snake)
        {
            if (sn.IsHaveSelfPos(CurPos))
            {
                snake = sn;
            }
        }

        if (snake)
        {
         
           GameManager.Instance.PlayHurtAnim(snake,CurPos);
           GameManager.Instance.CutSnake(snake,CurPos);
        }
       
        Item[] its = GameManager.Instance._Map.FindItem(CurPos);
        foreach (var it in its)
        {
            if (it is Box)
            {
                //出表 加 删除
                GameManager.Instance._Map.RemoveItemInMap(it);
              
            }
        }
        IsEffectRun = false;
    }

   
    
    public override void Init()
    {
        foreach (var it in _map._ItemGroup[(int)_ColorGroup])
        {
            if (it is GroundButton)
            {
                var button = it as GroundButton;
                _Button.Add(button);
                button.OnClickedListener += OnDoorButtonClick;
            }
        }
    }
 
    public override bool CanMoveIn(object[] args)
    {
        if (IsOpen)
        {
            Dir dir = (Dir)args[0];
            foreach (var it in AcceptDir)
            {
                if (it == dir)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
        
    }

    public override bool CanDestory(int type)
    {
        return false;
    }

    public override MoveEffecState GetMoveEffect(object[] args)
    {
        if (IsOpen)
        {
            Dir FromDir = (Dir)args[1];
            foreach (var it in AcceptDir)
            {
                if (it == FromDir)
                {
                    return MoveEffecState.NoEffect;
                }
            }
            return MoveEffecState.MoveFail; 
        }
        else
        {
            return MoveEffecState.MoveFail; 
        }
        
    }


    public void RoleMoveIn(SnakeMesh _role)
    {
      
    }

    public void RoleMoveOut(SnakeMesh _role)
    { 
        
      
    }
}
