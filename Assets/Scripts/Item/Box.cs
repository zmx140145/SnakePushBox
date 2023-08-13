using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Box : Item
{

    public Dir[] CanPutDir;
    public override void Init()
    {
    }

    
    public bool CanPush(SnakeMesh snakeMesh, Dir dir)
    {

        if (!CanPutDir.Contains(dir))
        {
            return false;
        }
        
        
        Point NextPos = CurPos + dir.TransferDir();
        if (NextPos.y >= _map.GetMapLength || NextPos.y < 0 || NextPos.x >= _map.GetMapWidth || NextPos.x < 0)
        {
            //箱子在边界无法移动箱子
            return false;
        }
        else
        {
            if (_map.IfMoveThisPos(NextPos) == MoveState.NoGround)
            {
                return false;
            }

            bool snakeBlock =
                !GameManager.Instance.isPosCanPushBoxNextFrame(dir, NextPos, new List<SnakeMesh>() { snakeMesh });
            if (snakeBlock)
            {
                return false;
            }

            Item[] its = _map.FindItem(NextPos);
            bool canMoveIn = true;
            foreach (var it in its)
            {
                canMoveIn = it.CanMoveIn(new object[1]{Dir.End-dir});
                if (!canMoveIn)
                {
                    return false;
                }

                
            }
            Item[] its1 = _map.FindItem(CurPos);
            foreach (var it in its1)
            {
                if (it is Fance || it is Door)
                {
                    canMoveIn = it.CanMoveIn(new object[1]{dir});
                    if (!canMoveIn)
                    {
                        return false;
                    }
                }
               

                
            }
            return true;
        }
    }

    private IEnumerator PushSelf(SnakeMesh snake,Point pos)
    {
        IsEffectRun = true;
        _map.ChangeItemPos(this, pos);
        float prograss = 0;
        Vector3 oldPos = transform.position;
        while (prograss < snake.MoveTime)
        {
            prograss += Time.fixedDeltaTime;
            var curPercent = snake._Curve.Evaluate(prograss / snake.MoveTime);
           var newpos = new Vector3(CurPos.x * GameManager.Instance.Size, oldPos.y, CurPos.y * GameManager.Instance.Size)+GameManager.Instance._Map.transform.position;
           transform.position = Vector3.Lerp(oldPos, newpos, curPercent);
           yield return new WaitForFixedUpdate();
        }
        IsEffectRun = false;
    }


    public void Push(Point pos, SnakeMesh snakeMesh, bool noDetect = false)
    {
        if (noDetect)
        {
          StartCoroutine( PushSelf(snakeMesh,pos));
        }
        else
        {
           
        }
    }


    public override bool CanMoveIn(object[] args)
    {
        return false;
    }

    public override bool CanDestory(int type)
    {
        //如果是cube效果就销毁
        if (type == 2)
        {
            return true;
        }
        else
        {
            //其他物体覆盖不会导致销毁
            return false;
        }
    }

   

    public override MoveEffecState GetMoveEffect(object[] args)
    {
        Dir MoveDir = (Dir)args[1];
        Debug.Log("尝试推箱子");
        if (CanPush((SnakeMesh
                )args[2], MoveDir))
        {
            //如果可以推
            if ((bool)args[0])
            {
                Push(CurPos + MoveDir.TransferDir(), (SnakeMesh
                    )args[2], true);
            }

            return MoveEffecState.AddLength;
        }
        else
        {
            //如果不能推
            return MoveEffecState.MoveFail;
        }
    }
}