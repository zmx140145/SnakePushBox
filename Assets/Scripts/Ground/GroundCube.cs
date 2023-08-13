using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeType
{
NoWalk,CanWork,Water,Fire  
}
public abstract class GroundCube : MonoBehaviour
{
   public NormalCubeType _NormalCubeType;
   public Point _CurPos;
   [HideInInspector]public CubeType _CubeType;

   public abstract void MoveIn(SnakeMesh _snake);
   public abstract MoveState CanMoveIn();
   public abstract void CheckCurrentItem();

}
