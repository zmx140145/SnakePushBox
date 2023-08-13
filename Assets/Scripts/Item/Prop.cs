using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropEffect
{
    Add,Remove,Cut,Restart,Reverse
}
public class Prop : Item,CanIn
{

    public PropEffect propEffect;
    public override void Init()
    {
        
    }

    public override bool CanMoveIn(object[] arg)
    {
        return true;
    }

    public override bool CanDestory(int type)
    {
        return true;
    }

    public override MoveEffecState GetMoveEffect(object[] args)
    {
        if ((bool)args[0])
        {
            RoleMoveIn((SnakeMesh)args[1]);
        }
        return MoveEffecState.NoEffect;
    }

    public void RoleMoveIn(SnakeMesh _role)
    {
        switch (propEffect)
        {
            case PropEffect.Cut:
                GameManager.Instance.CutCount++;
                break;
            case PropEffect.Add:
                GameManager.Instance.IncreaseCount++;
                break;
            case PropEffect.Remove:
                GameManager.Instance.RemoveCount ++;
                break;
            case PropEffect.Restart:
                GameManager.Instance.RestartCount ++;
                break;
            case PropEffect.Reverse:
                GameManager.Instance.ReverseCount ++;
                break;
        }
    }

    public void RoleMoveOut(SnakeMesh _role)
    {
     
    }
}
