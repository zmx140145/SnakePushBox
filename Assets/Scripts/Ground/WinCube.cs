using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCube : GroundCube
{
    private void Start()
    {
        _CubeType = CubeType.CanWork;
    }

    public override void MoveIn(SnakeMesh _snake)
    {
       
    }

    public override MoveState CanMoveIn()
    {
        return MoveState.Win;
    }

    public override void CheckCurrentItem()
    {
        
    }
}
