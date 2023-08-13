
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum EffectType
{
    Move,
}

[DefaultExecutionOrder(-2)]
public class GameManager : Singleton<GameManager>
{
    public Transform CameraOriginalTransform;
    public GameObject EscPanel;
    public GameObject ReviewNeedActive; 
    public MapMesh _Map;
    public GameObject[] MapPrefabs;
    public GameObject[] _SnakePrefabs;
    public List<SnakeMesh> _Snake;
    public float Size = 1;
    public int CurSelectMap = 0;
    public int MapStartSnakeLength = 1;
    public float SnakeSize;
    public GameObject[] Prefabs;
    public GameObject[] Objs;
    public List<GameObject> TipsBall;
    public GameObject GameObjectBall;
    public List<int> RandomSelectMapIndex = new List<int>();
    [Header("消失的网格")] public GameObject DiappearObj;
    [Header("特效")] public GameObject[] EffPlayers;
    [Header("游戏模式")] public int _GameMode = 1000;
    [Header("道具计数")] public int HavePropCount = 5;
    public int CutCount = 0;
    public int IncreaseCount = 0;
    public int RemoveCount = 0;
    public int RestartCount = 0;
    public int ReverseCount = 0;
    public Transform PreviewPos;
    public ViewCameraController _ViewCameraController;
    public List<MapMesh> _PeviewMapList = new List<MapMesh>();
    public int CurPrivewIndex = -1;
    public int CurRandomStateInGame = 0;//标记现在的游戏地图进度 0-9
    [Header("卡牌设置")] public AnimationCurve _CardSizeCurve;
    public float CardTurnTime = 1f;
    public float CardTargetSize = 1.2f;
    public CameraShaker _CameraEff;
    private void Start()
    {
        // for (int i = 0; i < TipsBall.Count; i++)
        // {
        //   TipsBall[i]=Instantiate(GameObjectBall);
        // }
        InputManager.Instance.MoveInput += MoveInput;
        UIManager.Instance.PushPanel(UIPanelType.Start);
        
    }
    private void Update()
    {
        EscPanelUpdate();
        KeyPropUpdate();
        // UpdateInput(Dir.Normal);
        CalculateGameTime();
    }
    public void CreateEffect(Point pos, Dir dir, EffectType effectType)
    {
        Vector3 To = Vector3.forward;
        Vector3 position = pos.GetV3(Size);
        switch (dir)
        {
            case Dir.Up:
                To = new Vector3(Dir.Up.TransferDir().x, 0f,
                    Dir.Up.TransferDir().y);
                break;
            case Dir.Left:
                To = new Vector3(Dir.Left.TransferDir().x, 0f,
                    Dir.Left.TransferDir().y);
                break;
            case Dir.Right:
                To = new Vector3(Dir.Right.TransferDir().x, 0f,
                    Dir.Right.TransferDir().y);
                break;
            case Dir.Down:
                To = new Vector3(Dir.Down.TransferDir().x, 0f,
                    Dir.Down.TransferDir().y);
                break;
        }

        var obj = Instantiate(EffPlayers[(int)effectType], position + _Map.transform.position, Quaternion.identity);
        obj.transform.forward = To;
    }

    private bool isTeachedUseTool = false;

    public void GameOver()
    {
        InputActive = false;
        if (_GameMode < 2000 && !isTeachedUseTool)
        {
            
            if (_Map is TutorialMap)
            {
                var tutorial = _Map as TutorialMap;
                tutorial.CurState = 1999;
                tutorial.PlayState(tutorial._PlayMode);
            }
            
          
        }

        if (_GameMode >= GameMode.InGame)
        {
            if (_Snake.Count > 0)
            {
                if (!IsHaveProp())
                {
                    EndGame();
                }
                else
                {
                    InputActive = true;
                }
            }
            else
            {
                EndGame();
            }
          
           
        }
    }
    public void GameOver1()
    {
        InputActive = false;
        if (_GameMode < 2000 && !isTeachedUseTool)
        {
            
            if (_Map is TutorialMap)
            {
                var tutorial = _Map as TutorialMap;
                tutorial.CurState = 1999;
                tutorial.PlayState(tutorial._PlayMode);
            }
            
          
        }

        if (_GameMode >= GameMode.InGame)
        {
            
                EndGame();
                
        }
    }
    public void EndGame()
    {
        IsGameStart = false;
        StartCoroutine("IE_EndGame");
    }

    IEnumerator IE_EndGame()
    {
        UIManager.Instance.ShowCloudMask();
        yield return new WaitUntil(() => { return UIManager.Instance.IsShowCloudEffectFinish(); });
        UIManager.Instance.ExitPanel(UIPanelType.Game,false);
        UIManager.Instance.PushPanel(UIPanelType.End);
        UIManager.Instance.HideCloudMask();
    }
    public bool ShowTips()
    {
        if (IsGameStart == false)
        {
            return false;
        }

        if (_Snake.Count == 0)
        {
            return false;
        }

        bool canMove = false;
        foreach (var it in _Snake)
        {
            if (it.TryKnowMove(Dir.Down))
            {
                canMove = true;
                // TipsBall[0].SetActive(true);
                // TipsBall[0].transform.position =
                //     new Vector3(Dir.Down.TransferDir().x * Size + _Snake[0]._list.First.Value.x * Size, 2f,
                //         Dir.Down.TransferDir().y * Size + _Snake[0]._list.First.Value.y * Size);
            }
            else
            {
                // TipsBall[0].SetActive(false);
            }

            if (it.TryKnowMove(Dir.Left))
            {
                canMove = true;
                // TipsBall[1].SetActive(true);
                // TipsBall[1].transform.position =
                //     new Vector3(Dir.Left.TransferDir().x * Size + _Snake[0]._list.First.Value.x * Size, 2f,
                //         Dir.Left.TransferDir().y * Size + _Snake[0]._list.First.Value.y * Size);
            }
            else
            {
                // TipsBall[1].SetActive(false);
            }

            if (it.TryKnowMove(Dir.Up))
            {
                canMove = true;
                // TipsBall[2].SetActive(true);
                // TipsBall[2].transform.position =
                //     new Vector3(Dir.Up.TransferDir().x * Size + _Snake[0]._list.First.Value.x * Size, 2f,
                //         Dir.Up.TransferDir().y * Size + _Snake[0]._list.First.Value.y * Size);
            }
            else
            {
                // TipsBall[2].SetActive(false);
            }

            if (it.TryKnowMove(Dir.Right))
            {
                canMove = true;
                // TipsBall[3].SetActive(true);
                // TipsBall[3].transform.position =
                //     new Vector3(Dir.Right.TransferDir().x * Size + _Snake[0]._list.First.Value.x * Size, 2f,
                //         Dir.Right.TransferDir().y * Size + _Snake[0]._list.First.Value.y * Size);
            }
            else
            {
                // TipsBall[3].SetActive(false); 
            }
        }

        if (!canMove)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public float curTimeRestart = 0f;
    public float curTimeReverse = 0f;
    public float curTimeIncrease = 0f;
    public float curTimeRemove = 0f;
    public float curTimeCut = 0f;
    private bool isRestartThisTime = false;
    private bool isReverseThisTime = false;
    private bool isIncreaseThisTime = false;
    private bool isRemoveThisTime = false;
    private bool isCutThisTime = false;
    public Action OnPropUse;

    public bool IsHaveProp()
    {
        return CutCount > 0 || IncreaseCount > 0 || RestartCount > 0 || ReverseCount > 0 || RemoveCount > 0;
    }
    
    private void KeyPropUpdate()
    {
       
        #region R 重开

        if (InputManager.Instance.GetRestartKeyDown)
        {
            curTimeRestart += Time.deltaTime;
            if (!isRestartThisTime)
            {
                if (curTimeRestart > 1f)
                {
                    if (_GameMode < GameMode.GameReady)
                    {
                        //教程模式
                        isRestartThisTime = true;
                        PlayReStartProp();
                     
                    }

                    if (_GameMode >= GameMode.InGame)
                    {
                        //游戏模式
                        if (RestartCount > 0)
                        {
                            isRestartThisTime = true;
                            RestartCount--;
                            PlayReStartProp();
                        }
                    }
                }
            }
            
        }
        else
        {
            curTimeRestart = 0f;
            isRestartThisTime = false;
        }

        #endregion

        #region C 切割

        if (InputManager.Instance.GetCutKeyDown)
        {
            curTimeCut += Time.deltaTime;
            if (!isCutThisTime)
            {
                if (curTimeCut > 1f)
                {
                    if (_GameMode < GameMode.GameReady)
                    {
                        //教程模式
                        isCutThisTime = true;
                  
                        PlayCutSnakeProp();
                       
                    }

                    if (_GameMode >= GameMode.InGame)
                    {
                        //游戏模式
                        if (CutCount > 0)
                        {
                            isCutThisTime = true;
                            CutCount--;
                            PlayCutSnakeProp();
                        }
                    }
                }
            }
          
        }
        else
        {
            curTimeCut = 0f;
            isCutThisTime = false;
        }

        #endregion

        #region V 增长

        if (InputManager.Instance.GetAddKeyDown)
        {
            curTimeIncrease += Time.deltaTime;
            if (!isIncreaseThisTime)
            {
               
                if (curTimeIncrease > 1f)
                {
                    if (_GameMode < GameMode.GameReady)
                    {
                        //教程模式
                        isIncreaseThisTime = true;
                        PlayIncreaseSnakeProp();
                        
                    }

                    if (_GameMode >= GameMode.InGame)
                    {
                        //游戏模式
                        if (IncreaseCount > 0)
                        {
                            isIncreaseThisTime = true;
                            IncreaseCount--;
                            PlayIncreaseSnakeProp();
                        }
                    }
                }
            }
          
        }
        else
        {
            curTimeIncrease = 0f;
            isIncreaseThisTime = false;
        }

        #endregion


        #region F 缩短

        if (InputManager.Instance.GetRemoveKeyDown )
        {
            curTimeRemove += Time.deltaTime;
            if (!isRemoveThisTime)
            {
              
                if (curTimeRemove > 1f)
                {
                
                    if (_GameMode < GameMode.GameReady)
                    {
                        //教程模式
                        isRemoveThisTime = true;
                        PlayRemoveSnakeProp();
             
                    }

                    if (_GameMode >= GameMode.InGame)
                    {
                        //游戏模式
                        if (RemoveCount > 0)
                        {
                            isRemoveThisTime = true;
                            RemoveCount--;
                            PlayRemoveSnakeProp();
                        }
                    }
                }
            }
           
        }
        else
        {
            curTimeRemove = 0f;
            isRemoveThisTime = false;
        }

        #endregion

        #region E 反转

        if (InputManager.Instance.GetReverseKeyDown)
        {
            curTimeReverse += Time.deltaTime;
            if (!isReverseThisTime)
            {
                if (curTimeReverse > 1f)
                {
                    if (_GameMode < GameMode.GameReady)
                    {
                        //教程模式
                        isReverseThisTime = true;
                        PlayReverseSnakeProp();
                 
                    }

                    if (_GameMode >= GameMode.InGame)
                    {
                        //游戏模式
                        if (ReverseCount > 0)
                        {
                            isReverseThisTime = true;
                            ReverseCount--;
                            PlayReverseSnakeProp();
                        }
                    }
                }
            }
            else
            {
                curTimeReverse = 0f;
                isReverseThisTime = false;
            }

            }
           
         
        #endregion
    }

    public void PlayReStartProp()
    {
        InitMap(CurSelectMap);
    }

    public bool Prop_NeedIncreaseThisMove = false;

    public void PlayIncreaseSnakeProp()
    {
        Debug.Log("增加");
        Prop_NeedIncreaseThisMove = true;
    }

    public void PlayReverseSnakeProp()
    {
        Debug.Log("反转");
        List<SnakeMesh> _listNeedReverse = new List<SnakeMesh>();
        foreach (var it in _Snake)
        {
            if (it._list.Count > 1)
            {
                _listNeedReverse.Add(it);
            }
        }

        foreach (var needReverse in _listNeedReverse)
        {
            ReverseSnake(needReverse);
        }
    }

    public void ReverseSnake(SnakeMesh snake)
    {
        LinkedListNode<Point> pL = snake._list.Last, pF = snake._list.First;
        while (pL != pF)
        {
            Point temp = pF.Value;
            pF.Value = pL.Value;
            pL.Value = temp;
            if (pF.Next == pL)
            {
                break;
            }
            pL = pL.Previous;
            pF = pF.Next;
        }
    }

    public void PlayRemoveSnakeProp()
    {
        Debug.Log("移除");
        List<SnakeMesh> _listNeedCut = new List<SnakeMesh>();
        foreach (var it in _Snake)
        {
            if (it._list.Count > 1)
            {
                _listNeedCut.Add(it);
            }
        }

        foreach (var snake in _listNeedCut)
        {
            PlayHurtAnim(snake,snake._list.First.Value);
            CutSnake(snake, snake._list.First.Value);
        }
    }

    public void PlayCutSnakeProp()
    {
        Debug.Log("切割");
        List<SnakeMesh> _listNeedCut = new List<SnakeMesh>();
        foreach (var it in _Snake)
        {
            if (it._list.Count > 2)
            {
                _listNeedCut.Add(it);
            }
        }

        foreach (var snake in _listNeedCut)
        {
            int i = snake._list.Count / 2;
            LinkedListNode<Point> po = snake._list.First;
            for (int k = 0; k < i ; k++)
            {
                po = po.Next;
            }
            PlayHurtAnim(snake,po.Value);
            CutSnake(snake, po.Value);
        }
    }

    public void CutSnake(SnakeMesh snakeMesh, Point pos)
    {
        int count = 0;
        SnakeMesh s1 = Instantiate(GameManager.Instance._SnakePrefabs[0]).GetComponent<SnakeMesh>();
        SnakeMesh s2 = Instantiate(GameManager.Instance._SnakePrefabs[0]).GetComponent<SnakeMesh>();
        var it = snakeMesh._list.First;
        bool FirstFin = false;
        while (it != snakeMesh._list.Last)
        {
            if (it.Value == pos)
            {
                count = 0;
                FirstFin = true;
                it = it.Next;
                continue;
            }

            if (!FirstFin)
            {
                count++;
                s1._list.AddLast(new Point(it.Value));
                s1.CurLength = count;
            }
            else
            {
                count++;
                s2._list.AddLast(new Point(it.Value));
                s2.CurLength = count;
            }

            it = it.Next;
        }

        //处理最后一个
        if (it.Value != pos)
        {
            count++;
            s2._list.AddLast(new Point(it.Value));
            s2.CurLength = count;
        }

        //两个新的配置好 显示
        if (s1.CurLength == 0)
        {
            Destroy(s1.gameObject);
        }
        else
        {
            s1._Map = snakeMesh._Map;
            s1.Size = snakeMesh.Size;
            s1.ChangePos();
            GameManager.Instance._Snake.Add(s1);
        }

        if (s2.CurLength == 0)
        {
            Destroy(s2.gameObject);
        }
        else
        {
            s2._Map = snakeMesh._Map;
            s2.Size = snakeMesh.Size;
            s2.ChangePos();
            GameManager.Instance._Snake.Add(s2);
        }


        //GameManager里面更新
        GameManager.Instance._Snake.Remove(snakeMesh);
        Destroy(snakeMesh.gameObject);
    }


    public bool InputActive = true;
    public bool IsGameStart = false;


    public int[] abc;

    public void  EscPanelUpdate()
    {
        if (_GameMode >= GameMode.InGame && _GameMode < GameMode.EndGame)
        {
            if (InputManager.Instance.GetEscDown)
            {
                if (UIManager.Instance.GetTopPanel(UIPanelType.Game))
                {
                    if (EscPanel)
                    {
                        if (EscPanel.active)
                        {
                            EscPanel.SetActive(false);
                            InputActive = true;
                        }
                        else
                        {
                            EscPanel.SetActive(true);
                            InputActive = false;
                        }
                        
                    }
                  
                }
               
            }
           
        }
    }

    private void MoveInput(Dir dir)
    {
        UpdateInput(dir);
    }

    public Double GameMapMaxTime=Double.MaxValue;
    public bool IsCalculatrRemainGameTime=false;
    public Double CurRemainGameTime;
    private void CalculateGameTime()
    {
        if (IsCalculatrRemainGameTime)
        {
            CurRemainGameTime += Time.deltaTime;
        }
    }
    private void UpdateInput(Dir dir)
    {
        if (InputActive)
        {
            // if (Input.GetKey(KeyCode.A))
            // {
            //     dir = Dir.Left;
            // }
            //
            // if (Input.GetKey(KeyCode.D))
            // {
            //     dir = Dir.Right;
            // }
            //
            // if (Input.GetKey(KeyCode.W))
            // {
            //     dir = Dir.Up;
            // }
            //
            // if (Input.GetKey(KeyCode.S))
            // {
            //     dir = Dir.Down;
            // }

            if (dir != Dir.Normal)
            {
                if (!InDoMove && !InCheckAllItem && !InCheckAllSnake && IsGameStart)
                {
                    StartCoroutine(DoMove(dir));
                    //移动之后要根据模式不同更新
                    if (_GameMode < GameMode.GameReady)
                    {
                        //如果教程模式移动了直接进入地图的指导模式
                        if (_GameMode < GameMode.TutorialDoMove)
                        {
                            //通知所有人更新状态
                            WordManager.Instance.NoticeOperaNewState(GameMode.TutorialDoMove);
                            _GameMode = GameMode.TutorialDoMove;
                            //设置ClickCount让按键可以触发开始游戏
                            UIManager.Instance.GetTopPanel(UIPanelType.Start).GetComponent<StartPanel>()
                                ._StartGameButton.ClickPointCount = 3;
                            _Map.InitMyTutorial(_GameMode);
                        }
                    }
                    else
                    {
                    }
                }
            }
        }
    }
    public bool IsSayMoveAdd = false;
    public bool IsCutSnakeWord = false;

    public void PlayHurtAnim(SnakeMesh snake, Point CurPos)
    {
        if (!IsCutSnakeWord)
        {
            if (_GameMode < GameMode.GameReady)
            {
                if (_Map)
                {
                    IsCutSnakeWord = true;
                    if (_Map is TutorialMap)
                    {
                        var it = _Map as TutorialMap;
                        it.CurState = 1005;
                        it.PlayState(WordPlayState.MoveNext);
                    }
                   
                }
            }
        }

        var Obj = Instantiate(DiappearObj, Vector3.zero, Quaternion.identity, transform);
        DisappearMesh dm = Obj.GetComponent<DisappearMesh>();
        dm.SetHurtMesh(snake, CurPos);
        dm.PlayHurtEffect();
    }

    private bool InDoMove = false;
    private bool IsSayNoBad = false;
    public float ShakeTime,ShakeRange;
    IEnumerator DoMove(Dir dir)
    {
        InDoMove = true;
        InputActive = false;
        IsCanDoEffect = false;
        bool iscanMove = false;
        foreach (var sn in _Snake)
        {
            if (sn)
            {
              bool a= sn.TryMove(dir);
              if (a)
              {
                  iscanMove = true;
              }
            }
           
        }

        if (!iscanMove)
        {
            StartCoroutine(_CameraEff.Shake(ShakeTime,ShakeRange));
            SoundManager.Instance.PlaySound(GlobalSounds.S_Error);
        }
        //等待所有的效果执行完成 再判断是否胜利
        if (!InCheckAllSnake)
        {
            InCheckAllItem = true;
            StartCoroutine(WaitAllSnakeMove(ApplyAllItem));
        }

        yield return new WaitUntil(() => { return !InCheckAllItem && !InCheckAllSnake; });
        InDoMove = false;
        if (!IsSayNoBad && _GameMode < GameMode.GameReady)
        {
            bool a = false;
//????
//TODO:不知道为什么写死了
            foreach (var sn in _Snake)
            {
                if (sn.IsHaveSelfPos(new Point(2, 4)))
                {
                    a = true;
                }
            }

            if (a && IsSayMoveAdd)
            {
                if (_Map)
                {
                    IsSayNoBad = true;
                    if (_Map is TutorialMap)
                    {
                        var tutorialMap = _Map as TutorialMap;
                        tutorialMap.CurState = 1007;
                        tutorialMap.PlayState(WordPlayState.MoveNext);
                    }

                }
            }
        }
        

        if (ShowTips())
        {
            InputActive = true;
        }
        else
        {
            if (IsGameStart)
            {
                //这里有说话
                GameOver();
            }
            else
            {
                Debug.Log("游戏结束");
            }
        }
    }

    IEnumerator DoMove1(Dir dir)
    {
        InDoMove = true;
        InputActive = false;
        IsCanDoEffect = false;
        foreach (var sn in _Snake)
        {
            sn?.TryMove(dir);
        }

        //等待所有的效果执行完成 再判断是否胜利
        if (!InCheckAllSnake)
        {
            InCheckAllItem = true;
            StartCoroutine(WaitAllSnakeMove(ApplyAllItem));
        }

        yield return new WaitUntil(() => { return !InCheckAllItem && !InCheckAllSnake; });
        InDoMove = false;
        InputActive = true;
    }

    public void ApplyAllItem()
    {
        //先检查所以方块效果
        foreach (var its in _Map._GroundMap)
        {
            foreach (var it in its)
            {
                it?.CheckCurrentItem();
            }
        }
        //再应用物体效果
        foreach (var it in _Map._ItemGroup)
        {
            foreach (var item in it)
            {
                if (item is GroundButton)
                {
                    GroundButton gb = item as GroundButton;
                    gb.CheckObj();
                }
            }
        }

      
        Debug.Log("判断");
        StartCoroutine(WaitAllItemApplyEffect(CheckCanWin));
    }

    public void CheckCanWin()
    {
        Debug.Log("胜利");
        foreach (var sn in _Snake)
        {
            sn?.JudgeWin();
        }
    }

    public bool IsCanDoEffect = true;
    public bool IsApplyEffectFinish = false;
    public bool InCheckAllItem = false;
    public bool InCheckAllSnake = false;

    IEnumerator WaitAllSnakeMove(Action action)
    {
        bool a = true;
        InCheckAllSnake = true;
        while (a)
        {
            a = false;
            foreach (var it in _Snake)
            {
                if (it.isInMove)
                {
                    a = true;
                }
            }

            yield return null;
        }

        InCheckAllSnake = false;
        IsCanDoEffect = true;
        action?.Invoke();
    }

    IEnumerator WaitAllItemApplyEffect(Action action)
    {
        IsApplyEffectFinish = false;
        if (!_Map)
        {
            InCheckAllItem = false;
            IsApplyEffectFinish = true;
            yield break;
        }


        bool a = true;
        while (a)
        {
            a = false;
            for (int i = 0; i < _Map.GetMapWidth; i++)
            {
                for (int j = 0; j < _Map.GetMapLength; j++)
                {
                    foreach (var it in _Map.FindItem(new Point(i, j)))
                    {
                        if (it)
                        {
                            if (it.IsEffectRun)
                            {
                                a = true;
                            }
                        }
                    }
                }
            }

            yield return null;
        }

        action?.Invoke();
        IsApplyEffectFinish = true;
        InCheckAllItem = false;
    }

    public bool isPosCanPushBoxNextFrame(Dir dir, Point pos, List<SnakeMesh> _BeforeList)
    {
        if (_BeforeList == null)
        {
            return false;
        }

        foreach (var b in _Snake)
        {
            if (b.isPosHaveSelfNextFramNoCycle(dir, pos, _BeforeList))
            {
                return false;
            }
        }

        return true;
    }

    public bool isPosHaveSnakeNextFrame(Dir dir, Point pos, List<SnakeMesh> _BeforeList)
    {
        foreach (var b in _Snake)
        {
            if (_BeforeList == null)
            {
                var c = new List<SnakeMesh>() { b };
                if (b.isPosHaveSelfNextFrame(dir, pos, c))
                {
                    return true;
                }
            }
            else
            {
                if (b.isPosHaveSelfNextFrame(dir, pos, _BeforeList))
                {
                    return true;
                }
            }
        }

        return false;
    }


    public void DestroySnakes()
    {
        for (int i = 0; i < _Snake.Count; i++)
        {
            Destroy(_Snake[i]?.gameObject);
            InputActive = false;
            IsGameStart = false;
        }

        _Snake.Clear();
    }

    public bool isStartReadyGame = false;
    public void StartReadyGame()
    {
        if (!isStartReadyGame)
        {
            isStartReadyGame = true;
            GameManager.Instance.Prop_NeedIncreaseThisMove = false;
            StartCoroutine("IE_StartReadyGame");
        }
    
    }

  

    public IEnumerator IE_StartReadyGame()
    {
        UIManager.Instance.ShowCloudMask();
        yield return new WaitUntil(() => { return UIManager.Instance.IsShowCloudEffectFinish(); });
        var it = GameManager.Instance._Map;
        if (it != null)
        {
            UIManager.Instance.GetTopPanel(UIPanelType.Start).GetComponent<StartPanel>()._StartPanelMap = it;
            it.gameObject.SetActive(false);
            
            GameManager.Instance._Map = null;
            GameManager.Instance.DestroySnakes();
        }
        UIManager.Instance.PushPanel(UIPanelType.Menu);
        UIManager.Instance.HideCloudMask();
    }

    public MapMesh InitReviewMap(int index)
    {
        if (index < MapPrefabs.Length && index >= 0)
        {
            GameObject obj = Instantiate(GameManager.Instance.MapPrefabs[index], Vector3.zero,
                Quaternion.identity, PreviewPos);
            MapMesh newMap = obj.GetComponent<MapMesh>();
            newMap.ThisMapIndex = index;
            newMap.InitWorld();
            return newMap;
        }

        return null;
    }


    public void InitMap(int CurMap)
    {
        Destroy(_Map?.gameObject);
        DestroySnakes();

        if (CurMap < MapPrefabs.Length && CurMap >= 0)
        {
            CurSelectMap = CurMap;
            GameObject obj = Instantiate(MapPrefabs[CurSelectMap]);
            _Map = obj.GetComponent<MapMesh>();
            _Map.ThisMapIndex = CurMap;
            //初始化地图和蛇
            _Map.InitWorld();
            GameObject s = Instantiate(_SnakePrefabs[0]);
            _Snake = new List<SnakeMesh>() { s.GetComponent<SnakeMesh>() };
            _Snake[0]._Map = _Map;
            _Snake[0].SnakeSize = SnakeSize;
            if (_Map.MapStartCount >= 1)
            {
                MapStartSnakeLength = _Map.MapStartCount;
            }

            _Snake[0].Init(_Map.StartPoint, Size, MapStartSnakeLength);
            // GameObject s2 = Instantiate(_SnakePrefabs[0]);
            // _Snake.Add(s2.GetComponent<SnakeMesh>());
            // _Snake[1]._Map = _Map;
            // _Snake[1].SnakeSize = SnakeSize;
            // _Snake[1].Init(_Map.StartPoint+new Point(3,2),Size,MapStartSnakeLength+1);

            InputActive = true;
            IsGameStart = true;
            UIManager.Instance.HideCloudMask();
        }
    }

    public void SnakeWinMap()
    {
        StartCoroutine("IE_SnakeWinMap");
    }

    IEnumerator IE_SnakeWinMap()
    {
        int nextMapIndex = -1;
        if (_GameMode >= GameMode.InGame)
        {
            CurRandomStateInGame++;
        }
        else
        {
            nextMapIndex = _Map.NextMapIndex;
        }

        UIManager.Instance.ShowCloudMask();
        yield return new WaitUntil(() => { return UIManager.Instance.IsShowCloudEffectFinish(); });
        for (int i = 0; i < _Snake.Count; i++)
        {
            Destroy(_Snake[i].gameObject);
        }

        _Snake.Clear();
        if (_GameMode >= GameMode.InGame)
        {
            if (CurRandomStateInGame < RandomSelectMapIndex.Count)
            {
                nextMapIndex = RandomSelectMapIndex[CurRandomStateInGame];
                CurSelectMap = nextMapIndex;
                InitMap(CurSelectMap);
            }
            else
            {
                CurSelectMap = -1;
                UIManager.Instance.ExitPanel(UIPanelType.Game,false);
                UIManager.Instance.PushPanel(UIPanelType.Win);
                UIManager.Instance.HideCloudMask();
            }
        }
        else
        {
            CurSelectMap = nextMapIndex;
            InitMap(CurSelectMap);
        }
    }


    public void OnClickStartGame()
    {
        if (!InStartGame)
        {
            InStartGame = true;
            StartCoroutine("IE_StartGame");
        }

    }

    public bool InStartGame = false;
    IEnumerator IE_StartGame()
    {
  
        UIManager.Instance.ShowCloudMask();
        yield return new WaitUntil(() => { return UIManager.Instance.IsShowCloudEffectFinish(); });
        UIManager.Instance.ExitPanel(UIPanelType.Menu,false);
        UIManager.Instance.PushPanel(UIPanelType.Game);
        UIManager.Instance.HideCloudMask();
        isStartReadyGame = false;
        InStartGame = false;
    }
}