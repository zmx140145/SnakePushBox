using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
   public Point CurPos;
   public int Kind;
   public MapMesh _map;
   public ColorGroup _ColorGroup;
   public bool IsEffectRun = false;
   public abstract void Init();
   public abstract bool CanMoveIn(object[] arg);
   //1 changPos 2 CubeEffect
   public abstract bool CanDestory(int type);
   public abstract MoveEffecState GetMoveEffect(object[] args);

}
