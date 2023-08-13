using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireCube : GroundCube
{
    public int FirCount = 3;
    private int curCount;
    public GameObject[] FireObjs;
    public GameObject GroundObj;
    private void Start()
    {
        curCount = FirCount;
        SetApperanceByCurCount(curCount);
    }

    private void SetApperanceByCurCount(int curCount)
    {
        if (curCount > 0 && curCount <= FirCount)
        {
            var index = FirCount - curCount;
            for (int i = 0; i < FirCount; i++)
            {
                if (i == index)
                {
                    FireObjs[i].SetActive(true);
                }
                else
                {
                    FireObjs[i].SetActive(false);
                }
            
            }
            GroundObj.SetActive(false);
        }
        else
        {
            if (curCount == 0)
            {
                for (int i = 0; i < FirCount; i++)
                {
                    FireObjs[i].SetActive(false);
                }
                GroundObj.SetActive(true);
            }
        }
        
    }
    public override void MoveIn(SnakeMesh _snake)
    {
        if (curCount > 0)
        {
            if (_snake._list.Count > 0)
            {
                GameManager.Instance.PlayHurtAnim(_snake,_snake._list.First.Value);
                _snake._list.RemoveFirst();
                _snake.ChangePos();
                _snake.CurLength -= 1;
              
            }

            if (_snake.CurLength <= 0)
            {
                GameManager.Instance._Snake.Remove(_snake);
            }
            curCount--;
            SetApperanceByCurCount(curCount);
        }
        
    }
    

    public override MoveState CanMoveIn()
    {
        return MoveState.Success;
    }

    public override void CheckCurrentItem()
    {
        var map = GameManager.Instance._Map;
       Item[] its= map.FindItem(_CurPos);
       foreach (var it in its)
       {
           if (curCount > 0)
           {
               if (it)
               {
                   if (it.CanDestory(2))
                   {
                       map.RemoveItemInMap(it);
                       curCount--;
                       SetApperanceByCurCount(curCount);
                   }
               }
           }
          
       }
    }
}
