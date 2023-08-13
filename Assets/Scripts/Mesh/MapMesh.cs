using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;

public enum MoveState
{
Success,NoGround,Win
}
public enum MoveEffecState
{
  NoEffect=0,AddLength=1,MoveFail=2
}

public enum ColorGroup
{
   NoColor,Red,Blue,Yellow,Green,End
}
[Serializable]
public class Col
{
   public int[] list;
}
public class MapMesh : MonoBehaviour
{
   public virtual void InitMyTutorial(int state)
   { }

   public Col[] _Cols;
   public Col[] _ObjCols;
   public Point StartPoint;
   public List<List<GroundCube>> _GroundMap = new List<List<GroundCube>>();
   public List<List<List<Item>>> _ItemMap=new List<List<List<Item>>>();
   public List<List<Item>> _ItemGroup = new List<List<Item>>();
   private int MapLength;
   private int MapWidth;
   public int MapStartCount = -1;
   public int GetMapLength => MapLength;
   public int GetMapWidth => MapWidth;
   public int NextMapIndex=0;
   public int ThisMapIndex;
   public Action<float> MapInitAction;

   public void RemoveItemInMap(Item it)
   {
      _ItemMap[it.CurPos.y][it.CurPos.x].Remove(it);
      _ItemGroup[(int)it._ColorGroup].Remove(it);
      Destroy(it.gameObject);
   }
   public void ChangeItemPos(Item item, Point pos)
   {
      _ItemMap[item.CurPos.y][item.CurPos.x].Remove(item);
      List<Item> NeedRemove = new List<Item>();
      foreach (var it in _ItemMap[pos.y][pos.x])
      {
         if (it.CanDestory(1))
         {
            NeedRemove.Add(it);
         }
      }

      foreach (var it in NeedRemove)
      {
       RemoveItemInMap(it);
      }
      _ItemMap[pos.y][pos.x].Add(item);
      item.CurPos = pos;
   }

   public MoveEffecState TryMoveThisPos(SnakeMesh snake, Dir dir, Point pos)
   {
      Debug.Log("TryMoveThisPos");
      Item[] its = FindItem(pos);
      MoveEffecState curEff = MoveEffecState.NoEffect;
      Dir lastDir = (int)Dir.End - dir;
      
      
      Item[] its2 = FindItem(pos + lastDir.TransferDir());
      foreach (var it in its2)
      {
         MoveEffecState eff2 = MoveEffecState.NoEffect;
         if (it)
         {
            #region Door

            if (it is Door)
            {
               var cDoor = it as Door;
               eff2 = cDoor.GetMoveEffect(new object[2] { true, dir });
            }
            else
            {
               if (it is Fance)
               {
                  var cFance = it as Fance;
                  eff2 = cFance.GetMoveEffect(new object[2] { true, dir });
               }
            }

            #endregion

            //上一个也要判断
            if ((int)eff2 > (int)curEff)
            {
               curEff = eff2;
            }

         }
      }

      if (curEff == MoveEffecState.MoveFail)
      {
         return MoveEffecState.MoveFail;
      }
      
      foreach (var it in its)
      {
         MoveEffecState eff = MoveEffecState.NoEffect;
         if (it)
         {
           


            #region Button

            if (it is GroundButton)
            {
               var cButton = it as GroundButton;

               eff = cButton.GetMoveEffect(new object[2] { true, snake });
            }

            #endregion
            
            #region Door

            if (it is Door)
            {
               var cDoor = it as Door;
               eff = cDoor.GetMoveEffect(new object[2] { true, lastDir });
            }

            #endregion
            #region Fance

            if (it is Fance)
            {
               var cFance = it as Fance;
               eff = cFance.GetMoveEffect(new object[2] { true, lastDir });
            }

            #endregion
            #region BOX

            if (it is Box)
            {
               var box = it as Box;

               eff = box.GetMoveEffect(new object[3] { true, dir, snake });
            }

            #endregion
            
            //上一个也要判断



            if ((int)eff > (int)curEff)
            {
               curEff = eff;
            }

         }
      }

    

         return curEff;


      
   }

   public MoveEffecState TryKnowMoveThisPosDoorAndFance(SnakeMesh snake,Dir dir,Point pos)
   {
    
      Item[] its = FindItem(pos);
      MoveEffecState curEff=MoveEffecState.NoEffect;
      foreach (var it in its)
      {
         MoveEffecState eff=MoveEffecState.NoEffect;
         if (it)
         {

            #region Door
            if (it is Door)
            {
               var cDoor= it as Door;
            
               eff= cDoor.GetMoveEffect(new object[2]{false,Dir.End-dir});
            }
            #endregion
            #region Fance
            if (it is Fance)
            {
               var cFance= it as Fance;
            
               eff= cFance.GetMoveEffect(new object[2]{false,Dir.End-dir});
            }
            #endregion

            if ((int)eff > (int)curEff)
            {
               curEff = eff;
            }
      
         }

       

      }
      
      return curEff;
    


   }
   public MoveEffecState TryKnowMoveThisPos(SnakeMesh snake,Dir dir,Point pos)
   {
    
      Item[] its = FindItem(pos);
      MoveEffecState curEff=MoveEffecState.NoEffect;
      foreach (var it in its)
      {
         MoveEffecState eff=MoveEffecState.NoEffect;
         if (it)
         {
            #region BOX
            if (it is Box)
            {
               var box= it as Box;
              
               eff= box.GetMoveEffect(new object[3]{false,dir,snake});
            }
            #endregion
            
            
            #region Button
            if (it is GroundButton)
            {
               var cButton= it as GroundButton;
            
               eff= cButton.GetMoveEffect(new object[2]{false,snake});
            }
            #endregion
            
            
            #region Door
            if (it is Door)
            {
               var cDoor= it as Door;
            
               eff= cDoor.GetMoveEffect(new object[2]{false,Dir.End-dir});
            }
            #endregion
            #region Fance
            if (it is Fance)
            {
               var cFance= it as Fance;
            
               eff= cFance.GetMoveEffect(new object[2]{false,Dir.End-dir});
            }
            #endregion

            if ((int)eff > (int)curEff)
            {
               curEff = eff;
            }
      
         }

       

      }
      
      return curEff;
    


   }
   public MoveState IfMoveThisPos(Point pos)
   {
      if (pos.y >= GetMapLength || pos.x < 0)
      {
         return MoveState.NoGround;
      }

      if ( pos.x >= GetMapWidth || pos.y < 0)
      {
         return MoveState.NoGround;
      }

     return _GroundMap[pos.y][pos.x].CanMoveIn();
   }
  public void InitWorld()
   {
      for (int i = 0; i < (int)(ColorGroup.End); i++)
      {
         _ItemGroup.Add(new List<Item>());
      }
      
      #region 初始化地形

      for (int i = 0; i < _Cols.Length; i++)
      {
         
         List<GroundCube> _grounds = new List<GroundCube>();
        
         for (int j = 0; j < _Cols[i].list.Length;j++)
         {
            int kind = _Cols[i].list[j];
            
            GameObject obj = null;
            obj = Instantiate(GameManager.Instance.Prefabs[kind], new Vector3(j+transform.position.x, -1+transform.position.y, i+transform.position.z)*GameManager.Instance.Size, Quaternion.identity, transform);
         
            GroundCube gc = obj.GetComponent<GroundCube>();
            gc._CurPos = new Point(j,i);
            _grounds.Add(gc);
         }
         _GroundMap.Add(_grounds);
      }


      #endregion


      #region 初始化物体

      for (int i = 0; i < _ObjCols.Length; i++)
      {
         List<List<Item>> _objs = new List<List<Item>>();
         for (int j = 0; j < _ObjCols[i].list.Length;j++)
         {
            int kind = _ObjCols[i].list[j];
            bool isAddthistime = false;
            while (kind>=0)
            {
               var RealKind = kind % 100;
               kind -= RealKind;
               kind = kind / 100;
               GameObject obj = null;
               if (RealKind > 0)
               {
                  obj = Instantiate(GameManager.Instance.Objs[RealKind], new Vector3(j+transform.position.x, 0+transform.position.y, i+transform.position.z)*GameManager.Instance.Size, Quaternion.identity, transform);
               }

               if (obj)
               {
              
                  Item it= obj.GetComponent<Item>();
                  it.Kind = RealKind;
                  it.CurPos = new Point() { x = j, y = i };
                  it._map = this;
                  if (!isAddthistime)
                  {
                     _objs.Add(new List<Item>(){it});
                     isAddthistime = true;
                  }
                  else
                  {
                     _objs[_objs.Count - 1].Add(it);
                  }
                  
                  _ItemGroup[(int)it._ColorGroup].Add(it);
               }
               else
               {
                  if (!isAddthistime)
                  {
                     _objs.Add(new List<Item>());
                     isAddthistime = true;
                  }
                  else
                  {
                     _objs[_objs.Count - 1].Add(null);
                  }
                
               }

               if (kind == 0)
               {
                  break;
               }
            }
           
           
            
            
         }
         
         _ItemMap.Add(_objs);
      }

      #endregion

      #region 所有物体自身初始化
      
      for (int i = 0; i < _ObjCols.Length; i++)
      {
         for (int j = 0; j < _ItemMap[i].Count; j++)
         {
            if (_ItemMap[i][j].Count > 0)
            {
               for (int k = 0; k < _ItemMap[i][j].Count; k++)
               {
                  Item it = _ItemMap[i][j][k];
                  if (it)
                  {
                     it.Init();
                  }
               }
              
            }
           
         }
      }
      
      #endregion
      
     
      MapWidth = _Cols[0].list.Length;
      MapLength = _Cols.Length;
      MapInitAction?.Invoke(GameManager.Instance.Size);
      foreach (var it in _ItemGroup)
      {
         foreach (var item in it)
         {
            if (item is GroundButton)
            {
               GroundButton gb = item as GroundButton;
               gb.CheckObj(this);
            }
         }
      }
   }
  
  public void Win(Point pos)
  {
     GameManager.Instance.InputActive = false;
     GameManager.Instance.IsGameStart = false;
     GameManager.Instance.MapStartSnakeLength = 0;
     StartCoroutine(WinThisMap(pos));
  }

  public SnakeMesh isHaveSnakeInWin(Point pos)
  {
     foreach (var snake in GameManager.Instance._Snake)
     {
        if (snake.IsHaveSelfPos(pos))
        {
           return snake;
        }
     }

     return null;
  }
 public IEnumerator WinThisMap(Point pos)
 {
     SnakeMesh sm = isHaveSnakeInWin(pos);
     while (sm!=null)
     {
        sm.LoseLastLength();
        GameManager.Instance.MapStartSnakeLength += 1;
        yield return new WaitForSeconds(0.4f);
        sm = isHaveSnakeInWin(pos);
     }
     GameManager.Instance.SnakeWinMap();
  }
   public Item[] FindItem(Point NextPos)
   {
      if (NextPos.y >= GetMapLength || NextPos.y < 0 || NextPos.x >= GetMapWidth || NextPos.x < 0)
      {
         return null;
      }
      else
      {
         return _ItemMap[NextPos.y][NextPos.x].ToArray();
      }
   }
   public GroundCube FindGround(Point pos)
   {
      if (pos.y >= GetMapLength || pos.y < 0 || pos.x >= GetMapWidth || pos.x < 0)
      {
         return null;
      }
      else
      {
         return _GroundMap[pos.y][pos.x];
      }
   }
}
