using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NormalCubeType
{
    Sand,Snow,Ice,Rock,Fire
}

public class NormalCube : GroundCube
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
        return MoveState.Success;
    }

    public override void CheckCurrentItem()
    {
        
    }
}
