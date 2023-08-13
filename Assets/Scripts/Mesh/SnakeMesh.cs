using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using LinkList;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

[Serializable]
public struct Point
{
    public int x;
    public int y;

    public static Point operator +(Point b, Point c)
    {
        Point a = new Point();
        a.x = b.x + c.x;
        a.y = b.y + c.y;
        return a;
    }

    public static bool operator ==(Point b, Point c)
    {
        if (b.x == c.x && b.y == c.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool operator !=(Point b, Point c)
    {
        return !(b == c);
    }

    public Point(int cx, int cy)
    {
        x = cx;
        y = cy;
    }

    public Point(Point a)
    {
        x = a.x;
        y = a.y;
    }

    public Vector3 GetV3(float size)
    {
        return new Vector3(x * size, 0f, y * size) ;
    }
}
[Serializable]
public enum Dir
{
    Normal,
    Right,
    Up,
    Down,
    Left,
    End
}

public static class SnakeHelper
{
    public static Point TransferDir(this Dir dir)
    {
        Point v2 = new Point();
        switch (dir)
        {
            case Dir.Normal:
                v2.x = 0;
                v2.y = 0;
                break;

            case Dir.Left:
                v2.x = -1;
                v2.y = 0;
                break;

            case Dir.Right:
                v2.x = 1;
                v2.y = 0;
                break;
            case Dir.Up:
                v2.x = 0;
                v2.y = 1;
                break;
            case Dir.Down:
                v2.x = 0;
                v2.y = -1;
                break;
        }

        return v2;
    }
}

public class SnakeMesh : MonoBehaviour
{
    public LinkedList<Point> _list = new LinkedList<Point>();
    public MeshFilter _MeshFilter;
    public MapMesh _Map;
    public int CurLength = 0;
    public float Size;

    public float SnakeSize = 0.8f;
    private float heightPosOffset;
    public Action<SnakeMesh> MoveEffectAction;

    public void Init(Point StartPos, float size, int curLength)
    {
        Size = size;
        _list.Clear();
        _list.AddFirst(new Point { x = StartPos.x, y = StartPos.y });
        CurLength = curLength;
        ChangePos();
    }

    private MeshCollider _meshCollider;
    private void Update()
    {
        if (!_meshCollider)
        {
            _meshCollider = GetComponent<MeshCollider>();
        }
if(_MeshFilter)
        _meshCollider.sharedMesh = _MeshFilter.mesh;
    }


    private bool TryCanActiveMoveToDir(Dir dir, List<SnakeMesh> _snakelist)
    {
        if (dir == Dir.Normal)
        {
            return false;
        }

        // if (_snakelist.Contains(this))
        // {
        //     return true;
        // }
        if (CanMove(dir, _snakelist))
        {
            Point pos = _list.First.Value;
            switch (TryKnowMoveEffectDoorAndFance((int)Dir.End-dir, pos))
            {
                case MoveEffecState.NoEffect:
                     break;
                case MoveEffecState.AddLength:
                    return false;
                case MoveEffecState.MoveFail:
                    return false;
            }
            pos += dir.TransferDir();
            switch (TryKnowMoveEffect(dir, pos))
            {
                case MoveEffecState.NoEffect:
                    return true;
                //区别在AddLength
                case MoveEffecState.AddLength:
                    return false;

                case MoveEffecState.MoveFail:

                    return false;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    private bool TryCanActiveMoveToDirNoCircle(Dir dir, List<SnakeMesh> _snakelist)
    {
        if (dir == Dir.Normal)
        {
            return false;
        }

        // if (_snakelist.Contains(this))
        // {
        //     return true;
        // }
        if (CanMoveNoCircle(dir, _snakelist))
        {
            Point pos = _list.First.Value;
            switch (TryKnowMoveEffectDoorAndFance((int)Dir.End-dir, pos))
            {
                case MoveEffecState.NoEffect:
                    break;
                case MoveEffecState.AddLength:
                    return false;
                case MoveEffecState.MoveFail:
                    return false;
            }
            pos +=dir.TransferDir();
            switch (TryKnowMoveEffect(dir, pos))
            {
                case MoveEffecState.NoEffect:
                    return true;

                case MoveEffecState.AddLength:
                    return false;

                case MoveEffecState.MoveFail:

                    return false;
            }
        }
        else
        {
            return false;
        }

        return false;
    }

    public bool TryKnowMove(Dir dir)
    {
        if (dir == Dir.Normal)
        {
            return false;
        }

        if (CanMove(dir, new List<SnakeMesh>() { this }))
        {
            Point pos = _list.First.Value;
            switch (TryKnowMoveEffectDoorAndFance((int)Dir.End-dir, pos))
            {
                case MoveEffecState.NoEffect:
                    break;
                case MoveEffecState.AddLength:
                    break;
                case MoveEffecState.MoveFail:
                    return false;
            }
            pos += dir.TransferDir();
            switch (TryKnowMoveEffect(dir, pos))
            {
                case MoveEffecState.NoEffect:
                    break;
                case MoveEffecState.AddLength:
                    break;
                case MoveEffecState.MoveFail:
                    return false;
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public void JudgeWin()
    {
        foreach (var pos in _list)
        {
            if (_Map.IfMoveThisPos(pos) == MoveState.Win)
            {
                _Map.Win(pos);
                break;
            }
        }
        
    }
    public bool TryMove(Dir dir)
    {
        if (dir == Dir.Normal)
        {
            return false;
        }

        if (CanMove(dir, new List<SnakeMesh>() { this }))
        {
            Point pos = _list.First.Value;
            switch (TryKnowMoveEffectDoorAndFance((int)Dir.End-dir, pos))
            {
                case MoveEffecState.NoEffect:
                    break;
                case MoveEffecState.AddLength:
                    break;
                case MoveEffecState.MoveFail:
                    return false;
            }
            pos +=dir.TransferDir();
            switch (TryApplyMoveEffect(dir, pos))
            {
                case MoveEffecState.NoEffect:
                    Move(pos, dir); //正常行走 
                    break;
                case MoveEffecState.AddLength:
                    MoveAddLength(pos,dir);
                    break;
                case MoveEffecState.MoveFail:

                    return false;
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

   

    private MoveEffecState TryKnowMoveEffect(Dir dir, Point pos)
    {
        return _Map.TryKnowMoveThisPos(this, dir, pos);
    }
    private MoveEffecState TryKnowMoveEffectDoorAndFance(Dir dir, Point pos)
    {
        return _Map.TryKnowMoveThisPosDoorAndFance(this, dir, pos);
    }
    private MoveEffecState TryApplyMoveEffect(Dir dir, Point pos)
    {
        return _Map.TryMoveThisPos(this, dir, pos);
    }

    private bool CanMoveNoCircle(Dir dir, List<SnakeMesh> _snakeList)
    {
        if (_list.Count < 1)
        {
            return false;
        }


        if (dir == Dir.Normal)
        {
            return false;
        }

        Point pos = dir.TransferDir();
        pos += _list.First.Value;
        if (_snakeList == null)
        {
            return false;
        }

        if (!GameManager.Instance.isPosCanPushBoxNextFrame(dir, pos, _snakeList))
        {
            return false;
        }

        if (isBlockByMap(pos))
        {
            Debug.Log("被地图遮挡");
            return false;
        }

        return true;
    }

    private bool CanMove(Dir dir, List<SnakeMesh> _snakeList)
    {
        if (_list.Count < 1)
        {
            return false;
        }


        if (dir == Dir.Normal)
        {
            return false;
        }

        Point pos = dir.TransferDir();
        pos += _list.First.Value;
        if (_snakeList == null)
        {
            return false;
        }

        if (GameManager.Instance.isPosHaveSnakeNextFrame(dir, pos, _snakeList))
        {
            return false;
        }

        if (isBlockByMap(pos))
        {
            Debug.Log("被地图遮挡"+$"{(int)dir}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 不能通过首尾相连来作为可移动标志
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pos"></param>
    /// <param name="_BeforeList"></param>
    /// <returns></returns>
    public bool isPosHaveSelfNextFramNoCycle(Dir dir, Point pos, List<SnakeMesh> _BeforeList)
    {
        if (_BeforeList == null)
        {
            return true;
        }

        if (_list.Last.Value == pos && _list.Count == CurLength)
        {
            if (!_BeforeList.Contains(this))
            {
                _BeforeList.Add(this);
                //如果这个位置是一个尾巴
                return !TryCanActiveMoveToDirNoCircle(dir, _BeforeList);
            }
            else
            {
                return true;
            }
        }
        else
        {
            return _list.Contains(pos);
        }
    }

    /// <summary>
    /// 被自己阻挡
    /// 可以通过首尾相连来作为可移动标志
    /// </summary>
    /// <param name="pos">传入要判断的点</param>
    /// <returns></returns>
    public bool isPosHaveSelfNextFrame(Dir dir, Point pos, List<SnakeMesh> _BeforeList)
    {
        if (_BeforeList == null)
        {
            return true;
        }

        if (_list.Last.Value == pos && _list.Count == CurLength)
        {
            if (!_BeforeList.Contains(this))
            {
                _BeforeList.Add(this);
                //如果这个位置是一个尾巴
                return !TryCanActiveMoveToDir(dir, _BeforeList);
            }
            else
            {
                if (_list.Count == 2 && _BeforeList.Count == 1)
                {
                    return true;
                }

                return false;
            }
        }
        else
        {
            return _list.Contains(pos);
        }
    }

    public bool IsHaveSelfPos(Point pos)
    {
        return _list.Contains(pos);
    }
/// <summary>
/// 地图是否可以走
/// </summary>
/// <param name="pos"></param>
/// <returns></returns>
    private bool isBlockByMap(Point pos)
    {
        switch (_Map.IfMoveThisPos(pos))
        {
            case MoveState.Success:
                return false;
            case MoveState.Win:
                return false;
            case MoveState.NoGround:
                return true;
        }

        return true;
    }

  

    public void LoseLastLength()
    {
        _list.RemoveLast();
        ChangePos();
    }

    public void LoseFirstLength()
    {
        _list.RemoveFirst();
        ChangePos();
    }

    public bool isInMove = false;
    public void Move(Point pos, Dir dir)
    {
        
        
        if (!isInMove)
        {
            if (CurLength != _list.Count)
            {
                MoveAddLength(pos,dir, false);
                return;
            }

            if (GameManager.Instance.Prop_NeedIncreaseThisMove)
            {
                GameManager.Instance.Prop_NeedIncreaseThisMove = false;
                MoveAddLength(pos,dir);
                return;
            }

            isInMove = true;
            
            StartCoroutine(MoveToDirTruerMeshPlayAnim(pos, dir));
             GroundCube Cube=_Map.FindGround(pos);
             if (Cube)
             {
                 SoundManager.Instance.PlaySound(GetRandomMoveSound(Cube._NormalCubeType),0.2f);
             }
          
            GameManager.Instance.CreateEffect(_list.First.Value+dir.TransferDir(),(int)Dir.End-dir,EffectType.Move);
        }
       
    }

    public string GetRandomMoveSound(NormalCubeType type)
    {
        Random.InitState((int)(Time.time*1000));
        
        switch (type)
        {
            case NormalCubeType.Sand: 
                int a = Random.Range(0, 3);
            switch (a)
            {
                case 0:
                    return GlobalSounds.S_SnakeMoveSand1;
                case 1:
                    return GlobalSounds.S_SnakeMoveSand2;
                case 2:
                    return GlobalSounds.S_SnakeMoveSand3;
                case 3:
                    return GlobalSounds.S_SnakeMoveSand4; 
            }
                break;
            case NormalCubeType.Ice:
                int b = Random.Range(0, 4);
                switch (b)
                {
                    case 0:
                        return GlobalSounds.S_SnakeMoveIce1;
                    case 1:
                        return GlobalSounds.S_SnakeMoveIce2;
                    case 2:
                        return GlobalSounds.S_SnakeMoveIce3;
                    case 3:
                        return GlobalSounds.S_SnakeMoveIce4; 
                }
                break;
            case NormalCubeType.Snow:
                int c = Random.Range(0, 5);
                switch (c)
                {
                    case 0:
                        return GlobalSounds.S_SnakeMoveSnow1;
                    case 1:
                        return GlobalSounds.S_SnakeMoveSnow2 ;
                    case 2:
                        return GlobalSounds.S_SnakeMoveSnow3;
                    case 3:
                        return GlobalSounds.S_SnakeMoveSnow4; 
                    case 4:
                        return GlobalSounds.S_SnakeMoveSnow5; 
                }
                break;
            case NormalCubeType.Fire:
                return GlobalSounds.S_SnakeMoveLave1;
                
            case NormalCubeType.Rock:
                
                break;

        }
       
        return GlobalSounds.S_SnakeMoveSand1;
    }
    
   
    public void MoveAddLength(Point pos, Dir dir, bool CalculateLength = true)
    {
       
        if (!isInMove)
        {
            if (GameManager.Instance._GameMode < 2000&& !GameManager.Instance.IsSayMoveAdd)
            {
                GameManager.Instance.IsSayMoveAdd = true;
                if (_Map is TutorialMap)
                {
                    var it = _Map as TutorialMap;
                    it.CurState = 1006;
                    it.PlayState(it._PlayMode);
                }
              
            }
            isInMove = true;
            GroundCube Cube=_Map.FindGround(pos);
            if (Cube)
            {
                SoundManager.Instance.PlaySound(GetRandomMoveSound(Cube._NormalCubeType),0.2f);
            }
            StartCoroutine(MoveToDirTrueAddLengthMeshPlayAnim(pos, dir, CalculateLength));
            GameManager.Instance.CreateEffect(_list.First.Value+dir.TransferDir(),(int)Dir.End-dir,EffectType.Move);
        }
       
    }
/// <summary>
/// 协程完之后调用 
/// </summary>
    private void IsMoveFinish(Point pos)
    {
        NoticeNewBlock(pos);
    }
private void NoticeNewBlock(Point pos)
{
        _Map.FindGround(pos)?.MoveIn(this);
}
    public void ChangePos()
    {
      
        MoveEffectAction?.Invoke(this);
        if (_list.Count == 0)
        {
            _MeshFilter.mesh = null;
            return;
        }

        LinkedListNode<Point> a = _list.First;
        Mesh TempMesh = new Mesh();

        List<Vector3> _ver = new List<Vector3>();
        List<int> _tri = new List<int>();


        List<Color> _colors = new List<Color>();
        Dir LastDir = Dir.Normal;
        Dir NextDir = Dir.Normal;
        Vector3 center;
        int cCount = 0;
        int TotalCount = _list.Count;
        while (a != _list.Last)
        {
            NextDir = CalculateDirToNext(a.Value, a.Next.Value);
            center = (new Vector3(a.Value.x, 0f, a.Value.y)+_Map.transform.position) * Size;
            float per1 = 1f - (float)cCount / TotalCount;
            if (LastDir != Dir.Normal)
            {
                //左下
                if ((LastDir == Dir.Left && NextDir == Dir.Down) || (LastDir == Dir.Down && NextDir == Dir.Left))
                {
                    AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        false,
                        false);
                }

                //左上
                if ((LastDir == Dir.Left && NextDir == Dir.Up) || (LastDir == Dir.Up && NextDir == Dir.Left))
                {
                    AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        true,
                        false);
                }

                //右上
                if ((LastDir == Dir.Right && NextDir == Dir.Up) || (LastDir == Dir.Up && NextDir == Dir.Right))
                {
                    AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        true,
                        true);
                }

                //右下
                if ((LastDir == Dir.Right && NextDir == Dir.Down) || (LastDir == Dir.Down && NextDir == Dir.Right))
                {
                    AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        false,
                        true);
                }

                if (LastDir == (int)Dir.End - NextDir)
                {
                    if (LastDir == Dir.Up || LastDir == Dir.Down)
                    {
                        AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white,
                            SnakeSize,
                            true,
                            false);
                    }

                    if (LastDir == Dir.Left || LastDir == Dir.Right)
                    {
                        AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white,
                            SnakeSize,
                            false,
                            true);
                    }
                }
            }
            else
            {
                //说明是头部
                if (NextDir == Dir.Up || NextDir == Dir.Down)
                {
                    AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        true,
                        false);
                }

                if (NextDir == Dir.Left || NextDir == Dir.Right)
                {
                    AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        false,
                        true);
                }

                if (NextDir == Dir.Normal)
                {
                    AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                        true,
                        true);
                }
            }

            LastDir = CalculateDirToNext(a.Next.Value, a.Value);
            a = a.Next;
            cCount++;
        }


        //说明只有一个点
        NextDir = Dir.Normal;
        center = (new Vector3(a.Value.x, 0f, a.Value.y)+_Map.transform.position) * Size;
        float per2 = 1f - (float)cCount / TotalCount;
        if (LastDir == Dir.Up || LastDir == Dir.Down)
        {
            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per2 * Color.white, SnakeSize, true,
                false);
        }

        if (LastDir == Dir.Left || LastDir == Dir.Right)
        {
            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per2 * Color.white, SnakeSize, false,
                true);
        }

        if (LastDir == Dir.Normal)
        {
            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per2 * Color.white, SnakeSize, true,
                true);
        }


        TempMesh.vertices = _ver.ToArray();
        TempMesh.triangles = _tri.ToArray();
        _MeshFilter.mesh = TempMesh;
        _MeshFilter.mesh.RecalculateNormals();
        _MeshFilter.mesh.RecalculateTangents();
        _MeshFilter.mesh.RecalculateBounds();
    }

    public Dir CalculateDirToNext(Point cur, Point next)
    {
        if (next.x == cur.x && next.y == cur.y)
        {
            return Dir.Normal;
        }

        if (next.x == cur.x)
        {
            if (next.y - cur.y == 1)
            {
                return Dir.Up;
            }

            if (cur.y - next.y == 1)
            {
                return Dir.Down;
            }

            return Dir.Normal;
        }

        if (next.y == cur.y)
        {
            if (next.x - cur.x == 1)
            {
                return Dir.Right;
            }

            if (cur.x - next.x == 1)
            {
                return Dir.Left;
            }

            return Dir.Normal;
        }

        return Dir.Normal;
    }

    #region 配合顶点处理的协程动画操作
    IEnumerator MoveToDirTrueRemoveLengthMeshPlayAnim(Point position)
    {
        yield break;
    }
    /// <summary>
    /// 增加长度的移动动画
    /// </summary>
    /// <param name="position"></param>
    /// <param name="movedir"></param>
    /// <returns></returns>
    IEnumerator MoveToDirTrueAddLengthMeshPlayAnim(Point position, Dir movedir,bool IsCalculateLength)
    {
        if (_list.Count == 0)
        {
            _MeshFilter.mesh = null;
            isInMove = false;
            yield break;
        }

        float prograss = 0;
        while (prograss < MoveTime)
        {
            prograss += Time.fixedDeltaTime;
            var curPercent = _Curve.Evaluate(prograss / MoveTime);

            #region 渲染网格

            Mesh TempMesh = new Mesh();

            List<Vector3> _ver = new List<Vector3>();
            List<int> _tri = new List<int>();


            List<Color> _colors = new List<Color>();
            Dir LastDir = Dir.Normal;
            Dir NextDir = Dir.Normal;
            Vector3 center;
            int cCount = 0;
            LinkedListNode<Point> a = _list.First;
            int TotalCount = _list.Count;
            Dir[] _RecordLastBodyState = new Dir[2];
            bool isConor = false;
            while (a != _list.Last)
            {
                NextDir = CalculateDirToNext(a.Value, a.Next.Value);
                center = (new Vector3(a.Value.x, 0f, a.Value.y)+_Map.transform.position) * Size;
                float per1 = 1f - (float)cCount / TotalCount;
                //用来记录最后一个的前一个是不是转角


                //是normal说明是头节点 不是就是其他节点
                if (LastDir != Dir.Normal)
                {
                    #region 转弯

                    //左下
                    if ((LastDir == Dir.Left && NextDir == Dir.Down) || (LastDir == Dir.Down && NextDir == Dir.Left))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            false,
                            false);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    //左上
                    if ((LastDir == Dir.Left && NextDir == Dir.Up) || (LastDir == Dir.Up && NextDir == Dir.Left))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            true,
                            false);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    //右上
                    if ((LastDir == Dir.Right && NextDir == Dir.Up) || (LastDir == Dir.Up && NextDir == Dir.Right))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            true,
                            true);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    //右下
                    if ((LastDir == Dir.Right && NextDir == Dir.Down) || (LastDir == Dir.Down && NextDir == Dir.Right))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            false,
                            true);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    #endregion

                    #region 长条

                    if (LastDir == (int)Dir.End - NextDir)
                    {
                        if (LastDir == Dir.Up || LastDir == Dir.Down)
                        {
                            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white,
                                SnakeSize,
                                true,
                                false);
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if (LastDir == Dir.Left || LastDir == Dir.Right)
                        {
                            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white,
                                SnakeSize,
                                false,
                                true);
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }
                    }

                    #endregion
                }
                else
                {
                    //说明是头部//如果下一个的反方向是运动方向 说明是直着走 
                    if (movedir == (Dir.End - (int)NextDir))
                    {
                        if (NextDir == Dir.Down || NextDir == Dir.Up)
                        {
                            AddAnimHeadStraightMesh(_ver, _tri, _colors, LastDir, NextDir, movedir, center, Size,
                                per1 * Color.white, SnakeSize, curPercent, true, false
                            );
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if (NextDir == Dir.Left || NextDir == Dir.Right)
                        {
                            AddAnimHeadStraightMesh(_ver, _tri, _colors, LastDir, NextDir, movedir, center, Size,
                                per1 * Color.white, SnakeSize, curPercent, false, true
                            );
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }
                    }
                    else
                    {
                        //不是直着走   //能进循环就不可能有单独的一个方块头节点 
                        if (movedir == Dir.Left && NextDir == Dir.Down)
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false
                                , false,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }


                        if ((movedir == Dir.Down && NextDir == Dir.Left))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true
                                , false,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }


                        if ((movedir == Dir.Left && NextDir == Dir.Up))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false
                                , true,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if ((movedir == Dir.Up && NextDir == Dir.Left))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true,
                                true,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        //右上
                        if ((movedir == Dir.Right && NextDir == Dir.Up))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false,
                                true,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if ((movedir == Dir.Up && NextDir == Dir.Right))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true,
                                true,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        //下右
                        if ((movedir == Dir.Right && NextDir == Dir.Down))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false,
                                false,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                       //右下
                        if ((movedir == Dir.Down && NextDir == Dir.Right))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true,
                                false,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }
                    }
                }

                LastDir = CalculateDirToNext(a.Next.Value, a.Value);
                a = a.Next;
                cCount++;
            }

            //最后一个点
            NextDir = Dir.Normal;
            center = (new Vector3(a.Value.x, 0f, a.Value.y)+_Map.transform.position) * Size;
            float per2 = 1f - (float)cCount / TotalCount;

            if (LastDir == Dir.Normal)
            {
                //只有一个点 
               AddAnimHeadStraightMeshAddLength(_ver,_tri,_colors,LastDir,NextDir,movedir,center,Size,per2 * Color.white,SnakeSize,curPercent);
            }
            else
            {
                
                #region 长条

                
                    if (LastDir == Dir.Up || LastDir == Dir.Down)
                    {
                        AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per2 * Color.white,
                            SnakeSize,
                            true,
                            false);
                        isConor = false;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    if (LastDir == Dir.Left || LastDir == Dir.Right)
                    {
                        AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per2 * Color.white,
                            SnakeSize,
                            false,
                            true);
                        isConor = false;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }
                

                #endregion
            }


            TempMesh.vertices = _ver.ToArray();
            TempMesh.triangles = _tri.ToArray();
            // TempMesh.colors = _colors.ToArray();
            _MeshFilter.mesh = TempMesh;

            _MeshFilter.mesh.RecalculateNormals();
            _MeshFilter.mesh.RecalculateTangents();
            _MeshFilter.mesh.RecalculateBounds();

            #endregion


            yield return new WaitForFixedUpdate();
        }

        
        
        
        if (IsCalculateLength)
               {
                   CurLength += 1;
               }
       
               _list.AddFirst(position);
               ChangePos();
               IsMoveFinish(position);
        isInMove = false;
    }

    public AnimationCurve _Curve;
    public float MoveTime = 2f;

    IEnumerator MoveToDirTruerMeshPlayAnim(Point position, Dir movedir)
    {
      
       
        if (_list.Count == 0)
        {
            _MeshFilter.mesh = null;
            isInMove = false;
            yield break;
        }

        float prograss = 0;
        while (prograss < MoveTime)
        {
            prograss += Time.fixedDeltaTime;
            var curPercent = _Curve.Evaluate(prograss / MoveTime);

            #region 渲染网格

            Mesh TempMesh = new Mesh();

            List<Vector3> _ver = new List<Vector3>();
            List<int> _tri = new List<int>();


            List<Color> _colors = new List<Color>();
            Dir LastDir = Dir.Normal;
            Dir NextDir = Dir.Normal;
            Vector3 center;
            int cCount = 0;
            LinkedListNode<Point> a = _list.First;
            int TotalCount = _list.Count;
            Dir[] _RecordLastBodyState = new Dir[2];
            bool isConor = false;
            while (a != _list.Last)
            {
                NextDir = CalculateDirToNext(a.Value, a.Next.Value);
                center = (new Vector3(a.Value.x, 0f, a.Value.y)+_Map.transform.position) * Size;
                float per1 = 1f - (float)cCount / TotalCount;
                //用来记录最后一个的前一个是不是转角


                //是normal说明是头节点 不是就是其他节点
                if (LastDir != Dir.Normal)
                {
                    #region 转弯

                    //左下
                    if ((LastDir == Dir.Left && NextDir == Dir.Down) || (LastDir == Dir.Down && NextDir == Dir.Left))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            false,
                            false);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    //左上
                    if ((LastDir == Dir.Left && NextDir == Dir.Up) || (LastDir == Dir.Up && NextDir == Dir.Left))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            true,
                            false);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    //右上
                    if ((LastDir == Dir.Right && NextDir == Dir.Up) || (LastDir == Dir.Up && NextDir == Dir.Right))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            true,
                            true);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    //右下
                    if ((LastDir == Dir.Right && NextDir == Dir.Down) || (LastDir == Dir.Down && NextDir == Dir.Right))
                    {
                        AddMeshConer(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white, SnakeSize,
                            false,
                            true);
                        isConor = true;
                        _RecordLastBodyState[0] = LastDir;
                        _RecordLastBodyState[1] = NextDir;
                    }

                    #endregion

                    #region 长条

                    if (LastDir == (int)Dir.End - NextDir)
                    {
                        if (LastDir == Dir.Up || LastDir == Dir.Down)
                        {
                            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white,
                                SnakeSize,
                                true,
                                false);
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if (LastDir == Dir.Left || LastDir == Dir.Right)
                        {
                            AddStraightMesh(_ver, _tri, _colors, LastDir, NextDir, center, Size, per1 * Color.white,
                                SnakeSize,
                                false,
                                true);
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }
                    }

                    #endregion
                }
                else
                {
                    //说明是头部//如果下一个的反方向是运动方向 说明是直着走 
                    if (movedir == (Dir.End - (int)NextDir))
                    {
                        if (NextDir == Dir.Down || NextDir == Dir.Up)
                        {
                            AddAnimHeadStraightMesh(_ver, _tri, _colors, LastDir, NextDir, movedir, center, Size,
                                per1 * Color.white, SnakeSize, curPercent, true, false
                            );
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if (NextDir == Dir.Left || NextDir == Dir.Right)
                        {
                            AddAnimHeadStraightMesh(_ver, _tri, _colors, LastDir, NextDir, movedir, center, Size,
                                per1 * Color.white, SnakeSize, curPercent, false, true
                            );
                            isConor = false;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }
                    }
                    else
                    {
                        //不是直着走   //能进循环就不可能有单独的一个方块头节点 
                        if (movedir == Dir.Left && NextDir == Dir.Down)
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false
                                , false,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }


                        if ((movedir == Dir.Down && NextDir == Dir.Left))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true
                                , false,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }


                        if ((movedir == Dir.Left && NextDir == Dir.Up))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false
                                , true,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if ((movedir == Dir.Up && NextDir == Dir.Left))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true,
                                true,
                                false);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        //右上
                        if ((movedir == Dir.Right && NextDir == Dir.Up))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false,
                                true,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        if ((movedir == Dir.Up && NextDir == Dir.Right))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true,
                                true,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                        //下右
                        if ((movedir == Dir.Right && NextDir == Dir.Down))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, false,
                                false,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }

                       //右下
                        if ((movedir == Dir.Down && NextDir == Dir.Right))
                        {
                            AddAnimHeadCornorMesh(_ver, _tri, _colors, center, Size, per1 * Color.white, SnakeSize,
                                curPercent, true,
                                false,
                                true);
                            isConor = true;
                            _RecordLastBodyState[0] = LastDir;
                            _RecordLastBodyState[1] = NextDir;
                        }
                    }
                }

                LastDir = CalculateDirToNext(a.Next.Value, a.Value);
                a = a.Next;
                cCount++;
            }

            //最后一个点
            NextDir = Dir.Normal;
            center = (new Vector3(a.Value.x, 0f, a.Value.y)+_Map.transform.position) * Size;
            float per2 = 1f - (float)cCount / TotalCount;

            if (LastDir == Dir.Normal)
            {
                AddAnimHeadStraightMesh(_ver, _tri, _colors, LastDir, NextDir, movedir, center, Size,
                    per2 * Color.white, SnakeSize, curPercent, false, false);
            }
            else
            {
                AddAnimButtomMesh(_ver, _tri, _colors, _RecordLastBodyState, movedir, LastDir, NextDir, center, Size,
                    per2 * Color.white, SnakeSize, curPercent, isConor);
            }


            TempMesh.vertices = _ver.ToArray();
            TempMesh.triangles = _tri.ToArray();
            // TempMesh.colors = _colors.ToArray();
            _MeshFilter.mesh = TempMesh;
            _MeshFilter.mesh.RecalculateNormals();
            _MeshFilter.mesh.RecalculateTangents();
            _MeshFilter.mesh.RecalculateBounds();
       
            #endregion


            yield return new WaitForFixedUpdate();
        }

        var cur = _list.Last;
        while (cur != _list.First)
        {
            cur.Value = cur.Previous.Value;
            cur = cur.Previous;
        }

        cur.Value = position;
        ChangePos();
        IsMoveFinish(position);
        isInMove = false;
    }

   

    #endregion


    #region 网格顶点处理

    /// <summary>
    /// 添加转角的网格 静态
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="LastDir"></param>
    /// <param name="NextDir"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="UDR"></param>
    /// <param name="LRR"></param>
    private void AddMeshConer(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir LastDir, Dir NextDir,
        Vector3 center, float size, Color color, float littleSize, bool UDR = false, bool LRR = false)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((UDR) ? -1 : 1);
        float XSize = size / 2 * ((LRR) ? -1 : 1);
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //0
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //1
        Vector3 _x_y_z =
            center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //2
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //3
        Vector3 mx_yz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //4
        Vector3 mxyz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //5
        Vector3 mx_ymz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //6
        Vector3 mxymz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //7
        Vector3 mx_y_z =
            center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //8
        Vector3 mxy_z = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //9
        Vector3 x_yz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //10
        Vector3 xyz = center +
                      new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //11
        Vector3 x_ymz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //12
        Vector3 xymz = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //13
        Vector3 x_y_z = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //14
        Vector3 xy_z = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //15
        int startCount = _ver.Count;
        _ver.Add(_x_yz); //0
        _ver.Add(_xyz); //1
        _ver.Add(_x_y_z); //2
        _ver.Add(_xy_z); //3
        _ver.Add(mx_yz); //4
        _ver.Add(mxyz); //5
        _ver.Add(mx_ymz); //6
        _ver.Add(mxymz); //7
        _ver.Add(mx_y_z); //8
        _ver.Add(mxy_z); //9
        _ver.Add(x_yz); //10
        _ver.Add(xyz); //11
        _ver.Add(x_ymz); //12
        _ver.Add(xymz); //13
        _ver.Add(x_y_z); //14
        _ver.Add(xy_z); //15
        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color
        });
        int s = 0;
        //上下表面
        //下表面
        if (UDR ^ LRR)
        {
            s = startCount + 1;
        }
        else
        {
            s = startCount;
        }

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s);

        _tri.Add(s + 2);
        _tri.Add(s + 6);
        _tri.Add(s + 4);

        _tri.Add(s + 4);
        _tri.Add(s + 6);
        _tri.Add(s + 12);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 10);

        _tri.Add(s + 12);
        _tri.Add(s + 6);
        _tri.Add(s + 8);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 14);

        //上表面
        if (UDR ^ LRR)
        {
            s = startCount;
        }
        else
        {
            s = startCount + 1;
        }

        _tri.Add(s + 2);
        _tri.Add(s);
        _tri.Add(s + 4);

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 10);
        _tri.Add(s + 12);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 6);

        _tri.Add(s + 12);
        _tri.Add(s + 14);
        _tri.Add(s + 8);

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //2
        _ver.Add(mx_ymz); //6
        _ver.Add(_xy_z); //3
        _ver.Add(mxymz); //7
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(mx_ymz); //6
        _ver.Add(mx_y_z); //8
        _ver.Add(mxymz); //7
        _ver.Add(mxy_z); //9
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        startCount = _ver.Count;
        _ver.Add(x_y_z); //14
        _ver.Add(x_ymz); //12
        _ver.Add(xy_z); //15
        _ver.Add(xymz); //13

        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);

            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_ymz); //12
        _ver.Add(x_yz); //10
        _ver.Add(xymz); //13
        _ver.Add(xyz); //11
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_yz); //10
        _ver.Add(mx_yz); //4
        _ver.Add(xyz); //11
        _ver.Add(mxyz); //5
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        startCount = _ver.Count;
        _ver.Add(mx_yz); //4
        _ver.Add(_x_yz); //0
        _ver.Add(mxyz); //5
        _ver.Add(_xyz); //1
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color, color, color, color, color, color, color, color, color
        });
    }

    /// <summary>
    /// 添加直的网格 静态
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="LastDir"></param>
    /// <param name="NextDir"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="isUD"></param>
    /// <param name="isLR"></param>
    private void AddStraightMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir LastDir, Dir NextDir,
        Vector3 center, float size, Color color, float littleSize, bool isUD, bool isLR)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((isLR) ? littleSize : 1f);
        float XSize = size / 2 * ((isUD) ? littleSize : 1f);
        Vector3 _x_y_z = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //0
        Vector3 x_y_z = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //1
        Vector3 x_yz = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //2
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //3
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //4
        Vector3 xy_z = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //5
        Vector3 xyz = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, ZSize); //6
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize); //7
        Vector3 v1, v2, v3;
        float NormalSize = size / 2 * littleSize;
        //如果是长条 不是正方形
        if (isLR || isUD)
        {
            if (LastDir == Dir.Normal)
            {
                switch (Dir.End - (int)NextDir)
                {
                    case Dir.Down:
                        //0
                        _x_y_z = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //0
                        //1
                        x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //1


                        //4
                        _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //4

                        //5
                        xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //5

                        break;
                    case Dir.Left:
                        //0
                        _x_y_z = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //0
                        //3
                        _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //3

                        //7
                        _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            NormalSize); //7
                        //4
                        _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //4
                        break;
                    case Dir.Right:
                        //1
                        x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //1
                        //2
                        x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //2

                        //5
                        xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //5
                        //6
                        xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                        break;
                    case Dir.Up:
                        //3-0
                        _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //3
                        //2-1
                        x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //2

                        //7-4
                        _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            NormalSize); //7
                        //6-5
                        xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                        break;
                }
            }

            if (NextDir == Dir.Normal)
            {
                switch (Dir.End - (int)LastDir)
                {
                    case Dir.Down:
                        //0
                        _x_y_z = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //0
                        //1
                        x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //1


                        //4
                        _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //4

                        //5
                        xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //5

                        break;
                    case Dir.Left:
                        //0
                        _x_y_z = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //0
                        //3
                        _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //3

                        //7
                        _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            NormalSize); //7
                        //4
                        _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //4
                        break;
                    case Dir.Right:
                        //1
                        x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //1
                        //2
                        x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //2

                        //5
                        xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset,
                            -NormalSize); //5
                        //6
                        xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                        break;
                    case Dir.Up:
                        //3-0
                        _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //3
                        //2-1
                        x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset,
                            NormalSize); //2

                        //7-4
                        _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset,
                            NormalSize); //7
                        //6-5
                        xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                        break;
                }
            }
        }

        //上下两个面是必须的
        int startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        //下表面
        _tri.Add(startCount);
        _tri.Add(startCount + 1);
        _tri.Add(startCount + 2);
        _tri.Add(startCount);
        _tri.Add(startCount + 2);
        _tri.Add(startCount + 3);

        //上表面
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 5);
        _tri.Add(startCount + 4);
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 6);
        _tri.Add(startCount + 5);


        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        if (LastDir != Dir.Right && NextDir != Dir.Right)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
        }

        if (LastDir != Dir.Left && NextDir != Dir.Left)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
        }

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });

        if (LastDir != Dir.Up && NextDir != Dir.Up)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        }

        if (LastDir != Dir.Down && NextDir != Dir.Down)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        }
    }

    /// <summary>
    ///  这个是不需要转弯的头部的伸长动画
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="LastDir"></param>
    /// <param name="NextDir"></param>
    /// <param name="MoveDir"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="curPercent">播放时长 0-1 </param>
    /// <param name="isUD"></param>
    /// <param name="isLR"></param>
    private void AddAnimHeadStraightMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir LastDir,
        Dir NextDir, Dir MoveDir,
        Vector3 center, float size, Color color, float littleSize, float curPercent, bool isUD, bool isLR)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((isLR) ? littleSize : 1f);
        float XSize = size / 2 * ((isUD) ? littleSize : 1f);
        if (!isLR && !isUD)
        {
            var point = MoveDir.TransferDir();
            ZSize = size / 2 * littleSize;
            XSize = size / 2 * littleSize;
            center = Vector3.Lerp(center, center + new Vector3(point.x, 0f, point.y) * size, curPercent);
        }

        Vector3 _x_y_z = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //0
        Vector3 x_y_z = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //1
        Vector3 x_yz = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //2
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //3
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //4
        Vector3 xy_z = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //5
        Vector3 xyz = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, ZSize); //6
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize); //7
        //对四个方向进行延长
        Vector3 v1, v2, v3;
        if (isLR || isUD)
        {
            switch (MoveDir)
            {
                case Dir.Down:
                    //0-3
                    v2 = _x_y_z;
                    v1 = _x_yz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    //1-2
                    v2 = x_y_z;
                    v1 = x_yz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    x_y_z = Vector3.Lerp(v2, v3, curPercent);

                    //4-7
                    v2 = _xy_z;
                    v1 = _xyz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _xy_z = Vector3.Lerp(v2, v3, curPercent);
                    //5-6
                    v2 = xy_z;
                    v1 = xyz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    xy_z = Vector3.Lerp(v2, v3, curPercent);
                    break;
                case Dir.Left:
                    //0-1
                    v2 = _x_y_z;
                    v1 = x_y_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    //3-2
                    v2 = _x_yz;
                    v1 = x_yz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _x_yz = Vector3.Lerp(v2, v3, curPercent);

                    //7-6
                    v2 = _xyz;
                    v1 = xyz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _xyz = Vector3.Lerp(v2, v3, curPercent);
                    //4-5
                    v2 = _xy_z;
                    v1 = xy_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _xy_z = Vector3.Lerp(v2, v3, curPercent);
                    break;
                case Dir.Right:
                    //1-0
                    v2 = x_y_z;
                    v1 = _x_y_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    //2-3
                    v2 = x_yz;
                    v1 = _x_yz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    x_yz = Vector3.Lerp(v2, v3, curPercent);

                    //5-4
                    v2 = xy_z;
                    v1 = _xy_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    xy_z = Vector3.Lerp(v2, v3, curPercent);
                    //6-7
                    v2 = xyz;
                    v1 = _xyz;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    xyz = Vector3.Lerp(v2, v3, curPercent);
                    break;
                case Dir.Up:
                    //3-0
                    v2 = _x_yz;
                    v1 = _x_y_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _x_yz = Vector3.Lerp(v2, v3, curPercent);
                    //2-1
                    v2 = x_yz;
                    v1 = x_y_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    x_yz = Vector3.Lerp(v2, v3, curPercent);

                    //7-4
                    v2 = _xyz;
                    v1 = _xy_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    _xyz = Vector3.Lerp(v2, v3, curPercent);
                    //6-5
                    v2 = xyz;
                    v1 = xy_z;
                      v3 = v1 + (v2 - v1)*(2f-heightPosOffset);
                    xyz = Vector3.Lerp(v2, v3, curPercent);
                    break;
            }
        }

        //上下两个面是必须的
        int startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        //下表面
        _tri.Add(startCount);
        _tri.Add(startCount + 1);
        _tri.Add(startCount + 2);
        _tri.Add(startCount);
        _tri.Add(startCount + 2);
        _tri.Add(startCount + 3);

        //上表面
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 5);
        _tri.Add(startCount + 4);
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 6);
        _tri.Add(startCount + 5);


        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        if (LastDir != Dir.Right && NextDir != Dir.Right)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
        }

        if (LastDir != Dir.Left && NextDir != Dir.Left)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
        }

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });

        if (LastDir != Dir.Up && NextDir != Dir.Up)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        }

        if (LastDir != Dir.Down && NextDir != Dir.Down)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        }
    }


      private void AddAnimHeadStraightMeshAddLength(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir LastDir,
        Dir NextDir, Dir MoveDir,
        Vector3 center, float size, Color color, float littleSize, float curPercent)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 *  littleSize ;
        float XSize = size / 2 *  littleSize ;
      

        Vector3 _x_y_z = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //0
        Vector3 x_y_z = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //1
        Vector3 x_yz = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //2
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //3
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //4
        Vector3 xy_z = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //5
        Vector3 xyz = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, ZSize); //6
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize); //7
        //对四个方向进行延长
        Vector3 v1, v2, v3;

        switch (MoveDir)
        {
            case Dir.Down:
                //0-3
                v2 = _x_y_z;
                v1 = _x_yz;
                v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _x_y_z = Vector3.Lerp(v2, v3, curPercent);
                //1-2
                v2 = x_y_z;
                v1 = x_yz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                x_y_z = Vector3.Lerp(v2, v3, curPercent);

                //4-7
                v2 = _xy_z;
                v1 = _xyz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _xy_z = Vector3.Lerp(v2, v3, curPercent);
                //5-6
                v2 = xy_z;
                v1 = xyz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                xy_z = Vector3.Lerp(v2, v3, curPercent);
                break;
            case Dir.Left:
                //0-1
                v2 = _x_y_z;
                v1 = x_y_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _x_y_z = Vector3.Lerp(v2, v3, curPercent);
                //3-2
                v2 = _x_yz;
                v1 = x_yz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _x_yz = Vector3.Lerp(v2, v3, curPercent);

                //7-6
                v2 = _xyz;
                v1 = xyz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _xyz = Vector3.Lerp(v2, v3, curPercent);
                //4-5
                v2 = _xy_z;
                v1 = xy_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _xy_z = Vector3.Lerp(v2, v3, curPercent);
                break;
            case Dir.Right:
                //1-0
                v2 = x_y_z;
                v1 = _x_y_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                x_y_z = Vector3.Lerp(v2, v3, curPercent);
                //2-3
                v2 = x_yz;
                v1 = _x_yz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                x_yz = Vector3.Lerp(v2, v3, curPercent);

                //5-4
                v2 = xy_z;
                v1 = _xy_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                xy_z = Vector3.Lerp(v2, v3, curPercent);
                //6-7
                v2 = xyz;
                v1 = _xyz;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                xyz = Vector3.Lerp(v2, v3, curPercent);
                break;
            case Dir.Up:
                //3-0
                v2 = _x_yz;
                v1 = _x_y_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _x_yz = Vector3.Lerp(v2, v3, curPercent);
                //2-1
                v2 = x_yz;
                v1 = x_y_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                x_yz = Vector3.Lerp(v2, v3, curPercent);

                //7-4
                v2 = _xyz;
                v1 = _xy_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                _xyz = Vector3.Lerp(v2, v3, curPercent);
                //6-5
                v2 = xyz;
                v1 = xy_z;
                  v3 = v1 + (v2 - v1)*(2f-2*heightPosOffset)/(1f-2*heightPosOffset);
                xyz = Vector3.Lerp(v2, v3, curPercent);
                break;
        }


        //上下两个面是必须的
        int startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        //下表面
        _tri.Add(startCount);
        _tri.Add(startCount + 1);
        _tri.Add(startCount + 2);
        _tri.Add(startCount);
        _tri.Add(startCount + 2);
        _tri.Add(startCount + 3);

        //上表面
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 5);
        _tri.Add(startCount + 4);
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 6);
        _tri.Add(startCount + 5);


        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        if (LastDir != Dir.Right && NextDir != Dir.Right)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
        }

        if (LastDir != Dir.Left && NextDir != Dir.Left)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
        }

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });

        if (LastDir != Dir.Up && NextDir != Dir.Up)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        }

        if (LastDir != Dir.Down && NextDir != Dir.Down)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        }
    }


    
    
    /// <summary>
    /// 这个是需要转弯的头部动画
    ///作用在长度大于等于三的时候
    /// 由于规划好，所以这里输入的LD_LR_RD_RU是和长度小于3的方法作用相反
    ///conor特殊处理 8 14 9 15
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="curPercent"></param>
    /// <param name="LD_LR_RD_RU">需要变化的方向是不是 LD 就是前进方向是LD</param>
    /// <param name="UDR"></param>
    /// <param name="LRR"></param>
    private void AddAnimHeadCornorMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Vector3 center,
        float size,
        Color color, float littleSize, float curPercent, bool LD_LR_RD_RU = true, bool UDR = false, bool LRR = false)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((UDR) ? -1 : 1);
        float XSize = size / 2 * ((LRR) ? -1 : 1);
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //0
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //1
        Vector3 _x_y_z =
            center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //2
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //3
        Vector3 mx_yz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //4
        Vector3 mxyz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //5
        Vector3 mx_ymz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //6
        Vector3 mxymz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //7
        Vector3 mx_y_z =
            center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //8
        Vector3 mxy_z = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //9
        Vector3 x_yz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //10
        Vector3 xyz = center +
                      new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //11
        Vector3 x_ymz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //12
        Vector3 xymz = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //13
        Vector3 x_y_z = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //14
        Vector3 xy_z = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //15
        Vector3 v3, v2, v1;
        if (LD_LR_RD_RU)
        {
            //重新计算点8-6
            v2 = mx_y_z;
            v1 = mx_ymz;
            v3 = v1 + (v2 - v1) / (heightPosOffset);
            mx_y_z = Vector3.Lerp(v1, v3, curPercent);
            //重新计算点9
            v2 = mxy_z;
            v1 = mxymz;
            v3 = v1 + (v2 - v1) / (heightPosOffset);
            mxy_z = Vector3.Lerp(v1, v3, curPercent);
            //重新计算点14
            v2 = x_y_z;
            v1 = x_ymz;
            v3 = v1 + (v2 - v1) / (heightPosOffset);
            x_y_z = Vector3.Lerp(v1, v3, curPercent);
            //重新计算点15
            v2 = xy_z;
            v1 = xymz;
            v3 = v1 + (v2 - v1) / (heightPosOffset);
            xy_z = Vector3.Lerp(v1, v3, curPercent);
        }
        else
        {
            //重新计算点0
            v2 = _x_yz;
            v1 = mx_yz;
            v3 = v1 + (v2 - v1) / ((1f - littleSize) * (size / 2));
            _x_yz = Vector3.Lerp(v1, v3, curPercent);
            //重新计算点1-5
            v2 = _xyz;
            v1 = mxyz;
            v3 = v1 + (v2 - v1) / ((1f - littleSize) * (size / 2));
            _xyz = Vector3.Lerp(v1, v3, curPercent);
            //重新计算点2
            v2 = _x_y_z;
            v1 = mx_ymz;
            v3 = v1 + (v2 - v1) / ((1f - littleSize) * (size / 2));
            _x_y_z = Vector3.Lerp(v1, v3, curPercent);
            //重新计算点3
            v2 = _xy_z;
            v1 = mxymz;
            v3 = v1 + (v2 - v1) / ((1f - littleSize) * (size / 2));
            _xy_z = Vector3.Lerp(v1, v3, curPercent);
        }

        int startCount = _ver.Count;
        _ver.Add(_x_yz); //0
        _ver.Add(_xyz); //1
        _ver.Add(_x_y_z); //2
        _ver.Add(_xy_z); //3
        _ver.Add(mx_yz); //4
        _ver.Add(mxyz); //5
        _ver.Add(mx_ymz); //6
        _ver.Add(mxymz); //7
        _ver.Add(mx_y_z); //8
        _ver.Add(mxy_z); //9
        _ver.Add(x_yz); //10
        _ver.Add(xyz); //11
        _ver.Add(x_ymz); //12
        _ver.Add(xymz); //13
        _ver.Add(x_y_z); //14
        _ver.Add(xy_z); //15
        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color
        });
        int s = 0;
        //上下表面
        //下表面
        if (UDR ^ LRR)
        {
            s = startCount + 1;
        }
        else
        {
            s = startCount;
        }

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s);

        _tri.Add(s + 2);
        _tri.Add(s + 6);
        _tri.Add(s + 4);

        _tri.Add(s + 4);
        _tri.Add(s + 6);
        _tri.Add(s + 12);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 10);

        _tri.Add(s + 12);
        _tri.Add(s + 6);
        _tri.Add(s + 8);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 14);

        //上表面
        if (UDR ^ LRR)
        {
            s = startCount;
        }
        else
        {
            s = startCount + 1;
        }

        _tri.Add(s + 2);
        _tri.Add(s);
        _tri.Add(s + 4);

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 10);
        _tri.Add(s + 12);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 6);

        _tri.Add(s + 12);
        _tri.Add(s + 14);
        _tri.Add(s + 8);

        
        
        if (!LD_LR_RD_RU)
        {
            //增加的面
            startCount = _ver.Count;
            _ver.Add(_x_yz); //0
            _ver.Add(_x_y_z); //2
            _ver.Add(_xyz); //1
            _ver.Add(_xy_z); //3
            s = startCount;
            if (UDR ^ LRR)
            {
                _tri.Add(s + 0);
                _tri.Add(s + 1);
                _tri.Add(s + 3);

                _tri.Add(s + 0);
                _tri.Add(s + 3);
                _tri.Add(s + 2);
            }
            else
            {
                _tri.Add(s + 0);
                _tri.Add(s + 3);
                _tri.Add(s + 1);

                _tri.Add(s + 0);
                _tri.Add(s + 2);
                _tri.Add(s + 3);
            }
        }
        
        
        startCount = _ver.Count;
        _ver.Add(_x_y_z); //2
        _ver.Add(mx_ymz); //6
        _ver.Add(_xy_z); //3
        _ver.Add(mxymz); //7
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(mx_ymz); //6
        _ver.Add(mx_y_z); //8
        _ver.Add(mxymz); //7
        _ver.Add(mxy_z); //9
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        if (LD_LR_RD_RU)
        {
            //增加的面
            startCount = _ver.Count;
            _ver.Add(mx_y_z); //8
            _ver.Add(x_y_z); //14
            _ver.Add(mxy_z); //9
            _ver.Add(xy_z); //15
            s = startCount;
            if (UDR ^ LRR)
            {
                _tri.Add(s + 2);
                _tri.Add(s + 0);
                _tri.Add(s + 1);

                _tri.Add(s + 2);
                _tri.Add(s + 1);
                _tri.Add(s + 3);
            }
            else
            {
                _tri.Add(s + 2);
                _tri.Add(s + 1);
                _tri.Add(s + 0);

                _tri.Add(s + 2);
                _tri.Add(s + 3);
                _tri.Add(s + 1);
            }
        }
        
        
        startCount = _ver.Count;
        _ver.Add(x_y_z); //14
        _ver.Add(x_ymz); //12
        _ver.Add(xy_z); //15
        _ver.Add(xymz); //13

        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);

            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_ymz); //12
        _ver.Add(x_yz); //10
        _ver.Add(xymz); //13
        _ver.Add(xyz); //11
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_yz); //10
        _ver.Add(mx_yz); //4
        _ver.Add(xyz); //11
        _ver.Add(mxyz); //5
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        startCount = _ver.Count;
        _ver.Add(mx_yz); //4
        _ver.Add(_x_yz); //0
        _ver.Add(mxyz); //5
        _ver.Add(_xyz); //1
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color, color, color, color, color, color, color, color, color
        });
    }


    /// <summary>
    /// 这是尾巴缩短的动画
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="LastDir"></param>
    /// <param name="NextDir"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="curPercent">播放时长 0-1</param>
    /// <param name="isUD"></param>
    /// <param name="isLR"></param>
    /// <param name="isConor"></param>
    private void AddAnimButtomMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir[] lastBodyDirState,
        Dir MoveDir, Dir LastDir, Dir NextDir,
        Vector3 center, float size, Color color, float littleSize, float curPercent, bool isConor)
    {
        //实现方法：因为是蛇的尾巴，进入这个方法说明前面一定有一个身体，要就先重新绘制前面的身体，抹去前面身体突出的那一小块
        //heightPosOffset,然后在绘制尾巴，把尾巴延长一个heightPosoffset,再进行动画操作。
        var point = LastDir.TransferDir();
        if (isConor) //判断上一个是不是转角
        {
           
            if (lastBodyDirState[0] == Dir.Normal)
            {
                //说明长度只有2
                //说明是头部 对应 AddAnimHeadCornorMesh的方法
                _ver.RemoveRange(_ver.Count - 44, 44);
                _tri.RemoveRange(_tri.Count - 78, 78);
       
                Debug.Log("长度只有2");
                //左下
                if ((MoveDir == Dir.Left && lastBodyDirState[1] == Dir.Down))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, true, false, false);
                }

                //下左
                if ((MoveDir == Dir.Down && lastBodyDirState[1] == Dir.Left))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, false, false, false);
                }

                //左上
                if ((MoveDir == Dir.Left && lastBodyDirState[1] == Dir.Up))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, true, true, false);
                }

                //上左
                if ((MoveDir == Dir.Up && lastBodyDirState[1] == Dir.Left))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, false, true, false);
                }

                //右上
                if ((MoveDir == Dir.Right && lastBodyDirState[1] == Dir.Up))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, true, true, true);
                }

                //上右
                if ((MoveDir == Dir.Up && lastBodyDirState[1] == Dir.Right))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, false, true, true);
                }

                //右下
                if ((MoveDir == Dir.Right && lastBodyDirState[1] == Dir.Down))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, true, false, true);
                }

                //下右
                if ((MoveDir == Dir.Down && lastBodyDirState[1] == Dir.Right))
                {
                    AddAnimBottomBeforeConorHeadMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y),
                        size, color, littleSize, curPercent, false, false, true);
                }
            }
            else
            {
                _ver.RemoveRange(_ver.Count - 40, 40);
                _tri.RemoveRange(_tri.Count - 72, 72);
                //说明是身体的中间 对应 AddMeshConer的方法
                if ((lastBodyDirState[0] == Dir.Left && lastBodyDirState[1] == Dir.Down))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, true, false, false);
                }

                //下左
                if ((lastBodyDirState[0] == Dir.Down && lastBodyDirState[1] == Dir.Left))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, false, false, false);
                }

                //左上
                if ((lastBodyDirState[0] == Dir.Left && lastBodyDirState[1] == Dir.Up))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, true, true, false);
                }

                //上左
                if ((lastBodyDirState[0] == Dir.Up && lastBodyDirState[1] == Dir.Left))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, false, true, false);
                }

                //右上
                if ((lastBodyDirState[0] == Dir.Right && lastBodyDirState[1] == Dir.Up))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, true, true, true);
                }

                //上右
                if ((lastBodyDirState[0] == Dir.Up && lastBodyDirState[1] == Dir.Right))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, false, true, true);
                }

                //右下
                if ((lastBodyDirState[0] == Dir.Right && lastBodyDirState[1] == Dir.Down))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, true, false, true);
                }

                //下右
                if ((lastBodyDirState[0] == Dir.Down && lastBodyDirState[1] == Dir.Right))
                {
                    AddAnimBottomBeforeConorMesh(_ver, _tri, _colors, center + new Vector3(point.x, 0f, point.y), size,
                        color, littleSize, false, false, true);
                }
            }
        }
        else
        {
            _ver.RemoveRange(_ver.Count - 24, 24);
            _tri.RemoveRange(_tri.Count - 24, 24);
            if (lastBodyDirState[0] == Dir.Normal)
            {
                Debug.LogWarning("长度只有2 进入了");
                if (lastBodyDirState[1] == Dir.Down || lastBodyDirState[1] == Dir.Up)
                {
                    AddAnimBottomBeforeStraightHeadMesh(_ver, _tri, _colors, lastBodyDirState[0], lastBodyDirState[1],
                        lastBodyDirState[1], center + new Vector3(point.x, 0f, point.y), size, color, littleSize,
                        curPercent, true,
                        false);
                }

                if (lastBodyDirState[1] == Dir.Left || lastBodyDirState[1] == Dir.Right)
                {
                    AddAnimBottomBeforeStraightHeadMesh(_ver, _tri, _colors, lastBodyDirState[0], lastBodyDirState[1],
                        lastBodyDirState[1], center + new Vector3(point.x, 0f, point.y), size, color, littleSize,
                        curPercent, false,
                        true);
                }
            }
            else
            {
                //长度大于2
                if (lastBodyDirState[1] == Dir.Down || lastBodyDirState[1] == Dir.Up)
                {
                    AddAnimBottomBeforeStraightMesh(_ver, _tri, _colors, lastBodyDirState[0], lastBodyDirState[1],
                        lastBodyDirState[1], center + new Vector3(point.x, 0f, point.y), size, color, littleSize, true,
                        false);
                }

                if (lastBodyDirState[1] == Dir.Left || lastBodyDirState[1] == Dir.Right)
                {
                    AddAnimBottomBeforeStraightMesh(_ver, _tri, _colors, lastBodyDirState[0], lastBodyDirState[1],
                        lastBodyDirState[1], center + new Vector3(point.x, 0f, point.y), size, color, littleSize, false,
                        true);
                }
            }
        }

        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * littleSize;
        float XSize = size / 2 * littleSize;
        Vector3 _x_y_z = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //0
        Vector3 x_y_z = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //1
        Vector3 x_yz = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //2
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //3
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //4
        Vector3 xy_z = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //5
        Vector3 xyz = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, ZSize); //6
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize); //7
        //对四个方向进行延长
        Vector3 v1, v2, v3;


        switch (LastDir)
        {
            //以向下为例子
            //向下就是 上一个身体部位在下面 先让自己下面顶点对其下面的那个身体的上面的顶点 再把上面的顶点 按时间来采样位置
            case Dir.Down:
                //0-3
                v2 = _x_y_z;
                v1 = _x_yz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _x_y_z = v3;
                _x_yz = Vector3.Lerp(v1, v3, curPercent);
                //1-2
                v2 = x_y_z;
                v1 = x_yz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                x_y_z = v3;
                x_yz = Vector3.Lerp(v1, v3, curPercent);

                //4-7
                v2 = _xy_z;
                v1 = _xyz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _xy_z = v3;
                _xyz = Vector3.Lerp(v1, v3, curPercent);

                //5-6
                v2 = xy_z;
                v1 = xyz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                xy_z = v3;
                xyz = Vector3.Lerp(v1, v3, curPercent);
                break;
            case Dir.Left:
                //0-1

                v2 = _x_y_z;
                v1 = x_y_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _x_y_z = v3;
                x_y_z = Vector3.Lerp(v1, v3, curPercent);
                //3-2


                v2 = _x_yz;
                v1 = x_yz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _x_yz = v3;
                x_yz = Vector3.Lerp(v1, v3, curPercent);
                //7-6


                v2 = _xyz;
                v1 = xyz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _xyz = v3;
                xyz = Vector3.Lerp(v1, v3, curPercent);

                //4-5


                v2 = _xy_z;
                v1 = xy_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _xy_z = v3;
                xy_z = Vector3.Lerp(v1, v3, curPercent);

                break;
            case Dir.Right:
                //1-0


                v2 = x_y_z;
                v1 = _x_y_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                x_y_z = v3;
                _x_y_z = Vector3.Lerp(v1, v3, curPercent);


                //2-3

                v2 = x_yz;
                v1 = _x_yz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                x_yz = v3;
                _x_yz = Vector3.Lerp(v1, v3, curPercent);


                //5-4
                v2 = xy_z;
                v1 = _xy_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                xy_z = v3;
                _xy_z = Vector3.Lerp(v1, v3, curPercent);

                //6-7


                v2 = xyz;
                v1 = _xyz;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                xyz = v3;
                _xyz = Vector3.Lerp(v1, v3, curPercent);
                break;
            case Dir.Up:
                //3-0


                v2 = _x_yz;
                v1 = _x_y_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _x_yz = v3;
                _x_y_z = Vector3.Lerp(v1, v3, curPercent);

                //2-1

                v2 = x_yz;
                v1 = x_y_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                x_yz = v3;
                x_y_z = Vector3.Lerp(v1, v3, curPercent);

                //7-4

                v2 = _xyz;
                v1 = _xy_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                _xyz = v3;
                _xy_z = Vector3.Lerp(v1, v3, curPercent);

                //6-5

                v2 = xyz;
                v1 = xy_z;
                v3 = v1 + (v2 - v1) / (1f - 2 * heightPosOffset);
                xyz = v3;
                xy_z = Vector3.Lerp(v1, v3, curPercent);
                break;
        }

        //上下两个面是必须的
        int startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        //下表面
        _tri.Add(startCount);
        _tri.Add(startCount + 1);
        _tri.Add(startCount + 2);
        _tri.Add(startCount);
        _tri.Add(startCount + 2);
        _tri.Add(startCount + 3);

        //上表面
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 5);
        _tri.Add(startCount + 4);
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 6);
        _tri.Add(startCount + 5);


        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        if (LastDir != Dir.Right && NextDir != Dir.Right)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
        }

        if (LastDir != Dir.Left && NextDir != Dir.Left)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
        }

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });

        if (LastDir != Dir.Up && NextDir != Dir.Up)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        }

        if (LastDir != Dir.Down && NextDir != Dir.Down)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        }
    }

    /// <summary>
    /// 头部动画 头部是倒数第二个 并且是直的
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="LastDir"></param>
    /// <param name="NextDir"></param>
    /// <param name="ShortDir"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="isUD"></param>
    /// <param name="isLR"></param>
    private void AddAnimBottomBeforeStraightHeadMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors,
        Dir LastDir, Dir NextDir, Dir ShortDir,
        Vector3 center, float size, Color color, float littleSize, float curPercent, bool isUD, bool isLR)
    {
        Dir MoveDir = (int)Dir.End - ShortDir;
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * littleSize;
        float XSize = size / 2 * littleSize;
        if (!isLR && !isUD)
        {
            Debug.LogWarning("不应该进入");
            var point = MoveDir.TransferDir();
            ZSize = size / 2 * littleSize;
            XSize = size / 2 * littleSize;
            center = Vector3.Lerp(center, center + new Vector3(point.x, 0f, point.y) * size, curPercent);
        }

        Vector3 _x_y_z = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //0
        Vector3 x_y_z = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //1
        Vector3 x_yz = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //2
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //3
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //4
        Vector3 xy_z = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //5
        Vector3 xyz = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, ZSize); //6
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize); //7

        //对四个方向进行缩短
        Vector3 v1, v2, v3;

        float NormalSize = size / 2 * littleSize;
        if (isLR || isUD)
        {
            switch (ShortDir)
            {
                case Dir.Down:
                    //0
                    v2 = _x_yz;
                    v1 = _x_y_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _x_yz = Vector3.Lerp(v2, v3, curPercent);
                    _x_y_z = center +
                             new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //0
                    //1
                    v2 = x_yz;
                    v1 = x_y_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    x_yz = Vector3.Lerp(v2, v3, curPercent);
                    x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //1


                    //4
                    v2 = _xyz;
                    v1 = _xy_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _xyz = Vector3.Lerp(v2, v3, curPercent);
                    _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //4

                    //5
                    v2 = xyz;
                    v1 = xy_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    xyz = Vector3.Lerp(v2, v3, curPercent);
                    xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //5

                    break;
                case Dir.Left:
                    //0
                    v2 = x_y_z;
                    v1 = _x_y_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    _x_y_z = center +
                             new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //0

                    //3
                    v2 = x_yz;
                    v1 = _x_yz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    x_yz = Vector3.Lerp(v2, v3, curPercent);
                    _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //3

                    //7
                    v2 = xyz;
                    v1 = _xyz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    xyz = Vector3.Lerp(v2, v3, curPercent);
                    _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //7
                    //4
                    v2 = xy_z;
                    v1 = _xy_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    xy_z = Vector3.Lerp(v2, v3, curPercent);
                    _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //4
                    break;
                case Dir.Right:
                    //1
                    v2 = _x_y_z;
                    v1 = x_y_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //1
                    //2
                    v2 = _x_yz;
                    v1 = x_yz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _x_yz = Vector3.Lerp(v2, v3, curPercent);
                    x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //2

                    //5
                    v2 = _xy_z;
                    v1 = xy_z;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _xy_z = Vector3.Lerp(v2, v3, curPercent);
                    xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //5
                    //6
                    v2 = _xyz;
                    v1 = xyz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _xyz = Vector3.Lerp(v2, v3, curPercent);
                    xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                    break;
                case Dir.Up:
                    //3-0
                    v2 = _x_y_z;
                    v1 = _x_yz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //3
                    //2-1
                    v2 = x_y_z;
                    v1 = x_yz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    x_y_z = Vector3.Lerp(v2, v3, curPercent);
                    x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //2

                    //7-4
                    v2 = _xy_z;
                    v1 = _xyz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    _xy_z = Vector3.Lerp(v2, v3, curPercent);
                    _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //7
                    //6-5
                    v2 = xy_z;
                    v1 = xyz;
                    v3 = v1 + (v2 - v1) * 2f / (1f - heightPosOffset);
                    xy_z = Vector3.Lerp(v2, v3, curPercent);
                    xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                    break;
            }
        }


        //上下两个面是必须的
        int startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        //下表面
        _tri.Add(startCount);
        _tri.Add(startCount + 1);
        _tri.Add(startCount + 2);
        _tri.Add(startCount);
        _tri.Add(startCount + 2);
        _tri.Add(startCount + 3);

        //上表面
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 5);
        _tri.Add(startCount + 4);
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 6);
        _tri.Add(startCount + 5);


        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        if (LastDir != Dir.Right && NextDir != Dir.Right)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
        }

        if (LastDir != Dir.Left && NextDir != Dir.Left)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
        }

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });

        if (LastDir != Dir.Up && NextDir != Dir.Up)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        }

        if (LastDir != Dir.Down && NextDir != Dir.Down)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        }
    }

    /// <summary>
    /// 这个是倒数第二个节点突出部分往回缩
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="LastDir"></param>
    /// <param name="NextDir"></param>
    /// <param name="ShortDir"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="isUD"></param>
    /// <param name="isLR"></param>
    private void AddAnimBottomBeforeStraightMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir LastDir,
        Dir NextDir, Dir ShortDir,
        Vector3 center, float size, Color color, float littleSize, bool isUD, bool isLR)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((isLR) ? littleSize : 1f);
        float XSize = size / 2 * ((isUD) ? littleSize : 1f);
        Vector3 _x_y_z = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //0
        Vector3 x_y_z = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //1
        Vector3 x_yz = center + new Vector3(XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //2
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize); //3
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //4
        Vector3 xy_z = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, -ZSize); //5
        Vector3 xyz = center + new Vector3(XSize, size / 2 * littleSize - heightPosOffset, ZSize); //6
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize); //7


        //对四个方向进行缩短
        Vector3 v1, v2, v3;

        float NormalSize = size / 2 * littleSize;
        switch (ShortDir)
        {
            case Dir.Down:
                //0
                _x_y_z = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //0
                //1
                x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //1


                //4
                _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //4

                //5
                xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //5

                break;
            case Dir.Left:
                //0
                _x_y_z = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //0
                //3
                _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //3

                //7
                _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //7
                //4
                _xy_z = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //4
                break;
            case Dir.Right:
                //1
                x_y_z = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, -NormalSize); //1
                //2
                x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //2

                //5
                xy_z = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, -NormalSize); //5
                //6
                xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                break;
            case Dir.Up:
                //3-0
                _x_yz = center + new Vector3(-NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //3
                //2-1
                x_yz = center + new Vector3(NormalSize, -size / 2 * littleSize - heightPosOffset, NormalSize); //2

                //7-4
                _xyz = center + new Vector3(-NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //7
                //6-5
                xyz = center + new Vector3(NormalSize, size / 2 * littleSize - heightPosOffset, NormalSize); //6
                break;
        }


        //上下两个面是必须的
        int startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        //下表面
        _tri.Add(startCount);
        _tri.Add(startCount + 1);
        _tri.Add(startCount + 2);
        _tri.Add(startCount);
        _tri.Add(startCount + 2);
        _tri.Add(startCount + 3);

        //上表面
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 5);
        _tri.Add(startCount + 4);
        _tri.Add(startCount + 7);
        _tri.Add(startCount + 6);
        _tri.Add(startCount + 5);


        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });
        if (LastDir != Dir.Right && NextDir != Dir.Right)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
        }

        if (LastDir != Dir.Left && NextDir != Dir.Left)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
        }

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //0
        _ver.Add(x_y_z); //1
        _ver.Add(x_yz); //2
        _ver.Add(_x_yz); //3
        _ver.Add(_xy_z); //4
        _ver.Add(xy_z); //5
        _ver.Add(xyz); //6
        _ver.Add(_xyz); //7
        _colors.AddRange(new Color[] { color, color, color, color, color, color, color, color });

        if (LastDir != Dir.Up && NextDir != Dir.Up)
        {
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        }

        if (LastDir != Dir.Down && NextDir != Dir.Down)
        {
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        }
    }
/// <summary>
/// 处理倒数第二个是转角但不是头部的情况
/// </summary>
/// <param name="_ver"></param>
/// <param name="_tri"></param>
/// <param name="_colors"></param>
/// <param name="center"></param>
/// <param name="size"></param>
/// <param name="color"></param>
/// <param name="littleSize"></param>
/// <param name="LD_LR_RD_RU"></param>
/// <param name="UDR"></param>
/// <param name="LRR"></param>
    private void AddAnimBottomBeforeConorMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Vector3 center,
        float size,
        Color color, float littleSize, bool LD_LR_RD_RU = true, bool UDR = false, bool LRR = false)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((UDR) ? -1 : 1);
        float XSize = size / 2 * ((LRR) ? -1 : 1);
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //0
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //1
        Vector3 _x_y_z =
            center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //2
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //3
        Vector3 mx_yz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //4
        Vector3 mxyz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //5
        Vector3 mx_ymz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //6
        Vector3 mxymz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //7
        Vector3 mx_y_z =
            center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //8
        Vector3 mxy_z = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //9
        Vector3 x_yz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //10
        Vector3 xyz = center +
                      new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //11
        Vector3 x_ymz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //12
        Vector3 xymz = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //13
        Vector3 x_y_z = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //14
        Vector3 xy_z = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //15
        Vector3 v3, v2, v1;


        if (LD_LR_RD_RU)
        {
            //重新计算点8=6
            mx_y_z = mx_ymz;

            //重新计算点9=7
            mxy_z = mxymz;

            //重新计算点14=12
            x_y_z = x_ymz;

            //重新计算点15=13
            xy_z = xymz;
        }
        else
        {
            //重新计算点0=4
            _x_yz = mx_yz;

            //重新计算点1=5
            _xyz = mxyz;

            //重新计算点2=6
            _x_y_z = mx_ymz;


            //重新计算点3=7
            _xy_z = mxymz;
        }

        int startCount = _ver.Count;
        _ver.Add(_x_yz); //0
        _ver.Add(_xyz); //1
        _ver.Add(_x_y_z); //2
        _ver.Add(_xy_z); //3
        _ver.Add(mx_yz); //4
        _ver.Add(mxyz); //5
        _ver.Add(mx_ymz); //6
        _ver.Add(mxymz); //7
        _ver.Add(mx_y_z); //8
        _ver.Add(mxy_z); //9
        _ver.Add(x_yz); //10
        _ver.Add(xyz); //11
        _ver.Add(x_ymz); //12
        _ver.Add(xymz); //13
        _ver.Add(x_y_z); //14
        _ver.Add(xy_z); //15
        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color
        });
        int s = 0;
        //上下表面
        //下表面
        if (UDR ^ LRR)
        {
            s = startCount + 1;
        }
        else
        {
            s = startCount;
        }

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s);

        _tri.Add(s + 2);
        _tri.Add(s + 6);
        _tri.Add(s + 4);

        _tri.Add(s + 4);
        _tri.Add(s + 6);
        _tri.Add(s + 12);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 10);

        _tri.Add(s + 12);
        _tri.Add(s + 6);
        _tri.Add(s + 8);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 14);

        //上表面
        if (UDR ^ LRR)
        {
            s = startCount;
        }
        else
        {
            s = startCount + 1;
        }

        _tri.Add(s + 2);
        _tri.Add(s);
        _tri.Add(s + 4);

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 10);
        _tri.Add(s + 12);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 6);

        _tri.Add(s + 12);
        _tri.Add(s + 14);
        _tri.Add(s + 8);

        startCount = _ver.Count;
        _ver.Add(_x_y_z); //2
        _ver.Add(mx_ymz); //6
        _ver.Add(_xy_z); //3
        _ver.Add(mxymz); //7
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(mx_ymz); //6
        _ver.Add(mx_y_z); //8
        _ver.Add(mxymz); //7
        _ver.Add(mxy_z); //9
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        startCount = _ver.Count;
        _ver.Add(x_y_z); //14
        _ver.Add(x_ymz); //12
        _ver.Add(xy_z); //15
        _ver.Add(xymz); //13

        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);

            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_ymz); //12
        _ver.Add(x_yz); //10
        _ver.Add(xymz); //13
        _ver.Add(xyz); //11
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_yz); //10
        _ver.Add(mx_yz); //4
        _ver.Add(xyz); //11
        _ver.Add(mxyz); //5
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        startCount = _ver.Count;
        _ver.Add(mx_yz); //4
        _ver.Add(_x_yz); //0
        _ver.Add(mxyz); //5
        _ver.Add(_xyz); //1
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color, color, color, color, color, color, color, color, color
        });
    }

    /// <summary>
    /// 修改倒数第二个网格是转角型的而且是头部 使其突出的heightposOffset部分不突出 
    /// </summary>
    /// <param name="_ver"></param>
    /// <param name="_tri"></param>
    /// <param name="_colors"></param>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="color"></param>
    /// <param name="littleSize"></param>
    /// <param name="LD_LR_RD_RU"></param>
    /// <param name="UDR"></param>
    /// <param name="LRR"></param>
    private void AddAnimBottomBeforeConorHeadMesh(List<Vector3> _ver, List<int> _tri, List<Color> _colors,
        Vector3 center, float size,
        Color color, float littleSize, float curPercent, bool LD_LR_RD_RU = true, bool UDR = false, bool LRR = false)
    {
        heightPosOffset = (1f - littleSize) * size / 2;
        float ZSize = size / 2 * ((UDR) ? -1 : 1);
        float XSize = size / 2 * ((LRR) ? -1 : 1);
        Vector3 _x_yz = center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //0
        Vector3 _xyz = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //1
        Vector3 _x_y_z =
            center + new Vector3(-XSize, -size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //2
        Vector3 _xy_z = center + new Vector3(-XSize, size / 2 * littleSize - heightPosOffset, -ZSize * littleSize); //3
        Vector3 mx_yz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //4
        Vector3 mxyz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //5
        Vector3 mx_ymz = center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //6
        Vector3 mxymz = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //7
        Vector3 mx_y_z =
            center + new Vector3(-XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //8
        Vector3 mxy_z = center + new Vector3(-XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //9
        Vector3 x_yz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            ZSize * littleSize); //10
        Vector3 xyz = center +
                      new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, ZSize * littleSize); //11
        Vector3 x_ymz = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //12
        Vector3 xymz = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset,
            -ZSize * littleSize); //13
        Vector3 x_y_z = center + new Vector3(XSize * littleSize, -size / 2 * littleSize - heightPosOffset, -ZSize); //14
        Vector3 xy_z = center + new Vector3(XSize * littleSize, size / 2 * littleSize - heightPosOffset, -ZSize); //15
        Vector3 v3, v2, v1;


        if (LD_LR_RD_RU)
        {
            //变化的点
            //0-4
            v2 = _x_yz;
            v1 = mx_yz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            _x_yz = Vector3.Lerp(v1, v3, curPercent);

            //1-5
            v2 = _xyz;
            v1 = mxyz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            _xyz = Vector3.Lerp(v1, v3, curPercent);
            //2-6
            v2 = _x_y_z;
            v1 = mx_ymz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            _x_y_z = Vector3.Lerp(v1, v3, curPercent);
            //3-7
            v2 = _xy_z;
            v1 = mxymz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            _xy_z = Vector3.Lerp(v1, v3, curPercent);


            //重新计算点8=6
            mx_y_z = mx_ymz;

            //重新计算点9=7
            mxy_z = mxymz;

            //重新计算点14=12
            x_y_z = x_ymz;

            //重新计算点15=13
            xy_z = xymz;
        }
        else
        {
            //变化的点
            //8-6
            v2 = mx_y_z;
            v1 = mx_ymz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            mx_y_z = Vector3.Lerp(v1, v3, curPercent);

            //9-7
            v2 = mxy_z;
            v1 = mxymz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            mxy_z = Vector3.Lerp(v1, v3, curPercent);
            //14-12
            v2 = x_y_z;
            v1 = x_ymz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            x_y_z = Vector3.Lerp(v1, v3, curPercent);
            //15-13
            v2 = xy_z;
            v1 = xymz;
            v3 = v1 + (v2 - v1) / heightPosOffset;
            xy_z = Vector3.Lerp(v1, v3, curPercent);


            //重新计算点0=4
            _x_yz = mx_yz;

            //重新计算点1=5
            _xyz = mxyz;

            //重新计算点2=6
            _x_y_z = mx_ymz;


            //重新计算点3=7
            _xy_z = mxymz;
        }

        int startCount = _ver.Count;
        _ver.Add(_x_yz); //0
        _ver.Add(_xyz); //1
        _ver.Add(_x_y_z); //2
        _ver.Add(_xy_z); //3
        _ver.Add(mx_yz); //4
        _ver.Add(mxyz); //5
        _ver.Add(mx_ymz); //6
        _ver.Add(mxymz); //7
        _ver.Add(mx_y_z); //8
        _ver.Add(mxy_z); //9
        _ver.Add(x_yz); //10
        _ver.Add(xyz); //11
        _ver.Add(x_ymz); //12
        _ver.Add(xymz); //13
        _ver.Add(x_y_z); //14
        _ver.Add(xy_z); //15
        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color
        });
        int s = 0;
        //上下表面
        //下表面
        if (UDR ^ LRR)
        {
            s = startCount + 1;
        }
        else
        {
            s = startCount;
        }

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s);

        _tri.Add(s + 2);
        _tri.Add(s + 6);
        _tri.Add(s + 4);

        _tri.Add(s + 4);
        _tri.Add(s + 6);
        _tri.Add(s + 12);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 10);

        _tri.Add(s + 12);
        _tri.Add(s + 6);
        _tri.Add(s + 8);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 14);

        //上表面
        if (UDR ^ LRR)
        {
            s = startCount;
        }
        else
        {
            s = startCount + 1;
        }

        _tri.Add(s + 2);
        _tri.Add(s);
        _tri.Add(s + 4);

        _tri.Add(s + 2);
        _tri.Add(s + 4);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 12);
        _tri.Add(s + 6);

        _tri.Add(s + 4);
        _tri.Add(s + 10);
        _tri.Add(s + 12);

        _tri.Add(s + 12);
        _tri.Add(s + 8);
        _tri.Add(s + 6);

        _tri.Add(s + 12);
        _tri.Add(s + 14);
        _tri.Add(s + 8);

        
        
        
        if (LD_LR_RD_RU)
        {
            //增加的面
            startCount = _ver.Count;
            _ver.Add(_x_yz); //0
            _ver.Add(_x_y_z); //2
            _ver.Add(_xyz); //1
            _ver.Add(_xy_z); //3
            s = startCount;
            if (UDR ^ LRR)
            {
                _tri.Add(s + 0);
                _tri.Add(s + 1);
                _tri.Add(s + 3);

                _tri.Add(s + 0);
                _tri.Add(s + 3);
                _tri.Add(s + 2);
            }
            else
            {
                _tri.Add(s + 0);
                _tri.Add(s + 3);
                _tri.Add(s + 1);

                _tri.Add(s + 0);
                _tri.Add(s + 2);
                _tri.Add(s + 3);
            }
        }
        
        
        
        startCount = _ver.Count;
        _ver.Add(_x_y_z); //2
        _ver.Add(mx_ymz); //6
        _ver.Add(_xy_z); //3
        _ver.Add(mxymz); //7
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(mx_ymz); //6
        _ver.Add(mx_y_z); //8
        _ver.Add(mxymz); //7
        _ver.Add(mxy_z); //9
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        if (!LD_LR_RD_RU)
        {
            //增加的面
            startCount = _ver.Count;
            _ver.Add(mx_y_z); //8
            _ver.Add(x_y_z); //14
            _ver.Add(mxy_z); //9
            _ver.Add(xy_z); //15
            s = startCount;
            if (UDR ^ LRR)
            {
                _tri.Add(s + 2);
                _tri.Add(s + 0);
                _tri.Add(s + 1);

                _tri.Add(s + 2);
                _tri.Add(s + 1);
                _tri.Add(s + 3);
            }
            else
            {
                _tri.Add(s + 2);
                _tri.Add(s + 1);
                _tri.Add(s + 0);

                _tri.Add(s + 2);
                _tri.Add(s + 3);
                _tri.Add(s + 1);
            }
        }
        

        
        startCount = _ver.Count;
        _ver.Add(x_y_z); //14
        _ver.Add(x_ymz); //12
        _ver.Add(xy_z); //15
        _ver.Add(xymz); //13

        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);

            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_ymz); //12
        _ver.Add(x_yz); //10
        _ver.Add(xymz); //13
        _ver.Add(xyz); //11
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        startCount = _ver.Count;
        _ver.Add(x_yz); //10
        _ver.Add(mx_yz); //4
        _ver.Add(xyz); //11
        _ver.Add(mxyz); //5
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 0);
            _tri.Add(s + 1);
            _tri.Add(s + 3);

            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 2);
        }
        else
        {
            _tri.Add(s + 0);
            _tri.Add(s + 3);
            _tri.Add(s + 1);

            _tri.Add(s + 0);
            _tri.Add(s + 2);
            _tri.Add(s + 3);
        }

        startCount = _ver.Count;
        _ver.Add(mx_yz); //4
        _ver.Add(_x_yz); //0
        _ver.Add(mxyz); //5
        _ver.Add(_xyz); //1
        s = startCount;
        if (UDR ^ LRR)
        {
            _tri.Add(s + 2);
            _tri.Add(s + 0);
            _tri.Add(s + 1);

            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 3);
        }
        else
        {
            _tri.Add(s + 2);
            _tri.Add(s + 1);
            _tri.Add(s + 0);

            _tri.Add(s + 2);
            _tri.Add(s + 3);
            _tri.Add(s + 1);
        }

        _colors.AddRange(new Color[]
        {
            color, color, color, color, color, color, color, color, color, color, color, color, color, color, color,
            color, color, color, color, color, color, color, color, color
        });
    }

    #endregion
}