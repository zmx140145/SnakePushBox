using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearMesh : MonoBehaviour
{
   public MeshFilter _meshFilter;
   public MeshRenderer _MeshRenderer;
   public float EffTime1;
   public float EffTime2;
 
   public AnimationCurve _Curve1;
   public AnimationCurve _Curve2;

  

   public void PlayHurtEffect()
   {
       StartCoroutine(PlayEffect());
   }

   IEnumerator PlayEffect()
   {
       if (!_meshFilter.mesh)
       {
        yield break;
       }
       float a = 0f;
       SoundManager.Instance.PlaySound(GlobalSounds.S_SnakeHurtStart);
       while (a<=EffTime1)
       {
           var cur = _Curve1.Evaluate(a / EffTime1);
           a += Time.deltaTime;
           _MeshRenderer.material.SetFloat("_CurTime",cur);
           yield return null;
       }
       _MeshRenderer.material.SetFloat("_CurTime",1f);
       a = 0f;
       bool isplay = false;
       while (a<=EffTime2)
       {
           var cur = _Curve2.Evaluate(a / EffTime2);
           a += Time.deltaTime;
           _MeshRenderer.material.SetFloat("_CurTime2",cur);
           if (a > EffTime2 - 0.1f&&!isplay)
           {
               isplay = true;
               SoundManager.Instance.PlaySound(GlobalSounds.S_SnakeHurtEnd);
           }
           yield return null;
       }
       _MeshRenderer.material.SetFloat("_CurTime2",1f);
       //播放音频
       
       Destroy(gameObject);
   }
   public void SetHurtMesh(SnakeMesh snakeMesh,Point Pos)
   {
       var _list = snakeMesh._list;
       var Size = snakeMesh.Size;
       var SnakeSize = snakeMesh.SnakeSize;
      if (_list.Count == 0)
        {
            _meshFilter.mesh = null;
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
            NextDir = snakeMesh.CalculateDirToNext(a.Value, a.Next.Value);
            center = new Vector3(a.Value.x, 0f, a.Value.y) * Size+GameManager.Instance._Map.transform.position;
            float per1 = 1f - (float)cCount / TotalCount;
            if (center != new Vector3(Pos.x, 0f, Pos.y) * Size+GameManager.Instance._Map.transform.position)
            {
                LastDir = snakeMesh.CalculateDirToNext(a.Next.Value, a.Value);
                a = a.Next;
                cCount++;
                continue;
            }

          
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

         
            LastDir = snakeMesh.CalculateDirToNext(a.Next.Value, a.Value);
            a = a.Next;
            cCount++;
        }


        //说明只有一个点
        NextDir = Dir.Normal;
        center = new Vector3(a.Value.x, 0f, a.Value.y) * Size+GameManager.Instance._Map.transform.position;
        float per2 = 1f - (float)cCount / TotalCount;
        if (center == new Vector3(Pos.x, 0f, Pos.y) * Size+GameManager.Instance._Map.transform.position)
        {
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

        }

        TempMesh.vertices = _ver.ToArray();
        TempMesh.triangles = _tri.ToArray();

        _meshFilter.mesh = TempMesh;
        _meshFilter.mesh.RecalculateNormals();
        _meshFilter.mesh.RecalculateTangents();
        _meshFilter.mesh.RecalculateBounds();
   }
      private void AddMeshConer(List<Vector3> _ver, List<int> _tri, List<Color> _colors, Dir LastDir, Dir NextDir,
        Vector3 center, float size, Color color, float littleSize, bool UDR = false, bool LRR = false)
    {
        var heightPosOffset = (1f - littleSize) * size / 2;
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
        var  heightPosOffset = (1f - littleSize) * size / 2;
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
       
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 1);
            _tri.Add(startCount + 5);
            
            
            _tri.Add(startCount);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 7);
            _tri.Add(startCount);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 4);
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

     
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 7);
            _tri.Add(startCount + 3);
            _tri.Add(startCount + 2);
            _tri.Add(startCount + 6);
            _tri.Add(startCount + 7);
        

        
            _tri.Add(startCount);
            _tri.Add(startCount + 4);
            _tri.Add(startCount + 5);
            _tri.Add(startCount);
            _tri.Add(startCount + 5);
            _tri.Add(startCount + 1);
        
    }

}
