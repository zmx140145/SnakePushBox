using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCube : GroundCube
{
    private void Start()
    {
        _CubeType = CubeType.NoWalk;
    }

    public override void MoveIn(SnakeMesh _snake)
    {
     
    }

    public override MoveState CanMoveIn()
    {
        return MoveState.NoGround;
    }

    public override void CheckCurrentItem()
    {
        
    }
}
