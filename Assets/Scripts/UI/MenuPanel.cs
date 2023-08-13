using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuPanel : UIPanel
{
    public GameObject MapStartPosObj;
    public ItemCard[] _ItemCards;
    public Transform LastPos, FirstPos, StartPos;
    public Text TotalCount;
    public AnimationCurve MoveAnimCurve;
    public GameObject[] WhenPreviewNeedHide;
    public ViewCameraController _PreviewCamera;
    private void Start()
    {
        GameManager.Instance._GameMode = GameMode.GameReady;
      GameManager.Instance.InStartGame = false;
        DisplayCard();
        GenerateRandomMap();
        _PreviewCamera = GameManager.Instance._ViewCameraController;
        InputManager.Instance.MoveInput += TouchInput;
        
    }

    private void Update()
    {
        TotalCount.text = GameManager.Instance.HavePropCount.ToString();
    }

    public void GenerateRandomMap()
    {
   
        var randomMapIndexs = GameManager.Instance.RandomSelectMapIndex;
        randomMapIndexs.Clear();
        List<int> RandomList = new List<int>();
        for (int i = 3; i < GameManager.Instance.MapPrefabs.Length-1; i++)
        {
            RandomList.Add(i);
        }

        for (int i = 0; i < 9; i++)
        {
            int r = Random.Range(i, RandomList.Count);
            var temp = RandomList[i];
            RandomList[i] = RandomList[r];
            RandomList[r] = temp;
            randomMapIndexs.Add(RandomList[i]);
        }
        randomMapIndexs.Add(GameManager.Instance.MapPrefabs.Length-1);
        StartCoroutine(InstantiateReviewMap());
    }

    public bool IsInitMapObj = false;
    public float Offest = 30f;

    public void StartReview()
    {
        if(IsInitMapObj)
        StartCoroutine(IE_StartReview());
    }

    public void Exit()
    {
        if (GameManager.Instance._PeviewMapList.Count > 0)
        {
            foreach (var map in GameManager.Instance._PeviewMapList)
            {
                if(map)
                    Destroy(map.gameObject);
            }
        }
        GameManager.Instance._PeviewMapList.Clear();
        UIManager.Instance.ExitPanel(UIPanelType.Menu);
    }

    public bool IsCurTouchInput(Dir dir)
    {

        if (curTouchInput == Dir.Normal)
        {
            return false;
        }
            var temp = curTouchInput;
            if (dir == temp)
            {
                curTouchInput = Dir.Normal;
                return true;
            }

            return false;
    }

    private Dir curTouchInput;
 
    public void TouchInput(Dir dir)
    {
        curTouchInput = dir;
    }
    IEnumerator IE_StartReview()
    {
        UIManager.Instance.ShowCloudMask();
        yield return new WaitUntil(() => {return UIManager.Instance.IsShowCloudEffectFinish();});
        GameManager.Instance.ReviewNeedActive?.SetActive(true);
        foreach (var go in WhenPreviewNeedHide)
        {
         go.SetActive(false);   
        }
        foreach (var it in GameManager.Instance._PeviewMapList)
        {
            if(it)
            it.gameObject.SetActive(false);
        }
        
        UIManager.Instance.InPreviewMode = true;
        _PreviewCamera.SetStartPos(GameManager.Instance.PreviewPos.position-new Vector3(50f,0,0));
        var lastSelect = 0;
        GameManager.Instance.CurPrivewIndex = 0;
        _PreviewCamera.gameObject.SetActive(true);
        var map2 = GameManager.Instance._PeviewMapList[lastSelect];
        map2.gameObject.SetActive(true);
        //根据屏幕分辨率来调整
        float ViewSize =Mathf.Max(map2.GetMapLength / 2f,map2.GetMapWidth/3f);
        var tempSelect = lastSelect;
        _PreviewCamera.SetViewTarget(1,lastSelect,map2.transform.position+new Vector3(map2.GetMapWidth/2f,0f,map2.GetMapLength/4f)*GameManager.Instance.Size,ViewSize);
        while (true)
        {
            if (IsCurTouchInput(Dir.Left))
            {
                if (GameManager.Instance.CurPrivewIndex >= 1)
                {
                    GameManager.Instance.CurPrivewIndex--;
                }
            }
            else
            {
                if (IsCurTouchInput(Dir.Right))
                {
                    if (GameManager.Instance.CurPrivewIndex <= GameManager.Instance._PeviewMapList.Count-2)
                    {
                        GameManager.Instance.CurPrivewIndex++;
                    }
                   
                }
            }

            if (lastSelect !=GameManager.Instance.CurPrivewIndex)
            {
                 tempSelect = lastSelect;
                lastSelect = GameManager.Instance.CurPrivewIndex;
                var map = GameManager.Instance._PeviewMapList[lastSelect];
                float ViewSize1 =Mathf.Max(map.GetMapLength / 2f,map.GetMapWidth/3f);
                _PreviewCamera.SetViewTarget(tempSelect,lastSelect,map.transform.position+new Vector3(map.GetMapWidth/2f,0f,map.GetMapLength/4f)*GameManager.Instance.Size,ViewSize1);
            }
            if (InputManager.Instance.GetEscDown||IsCurTouchInput(Dir.Up))
            {
                break;
            }
            yield return null;
        }
        UIManager.Instance.InPreviewMode = false;
        foreach (var go in WhenPreviewNeedHide)
        {
            go.SetActive(true);   
        }
        _PreviewCamera.SetViewTarget(lastSelect,lastSelect,transform.position-new Vector3(50f,0,0),1);
        UIManager.Instance.HideCloudMask();
        GameManager.Instance.ReviewNeedActive?.SetActive(false);
        yield return new WaitUntil(() => {return UIManager.Instance.IsShowCloudEffectFinish();});
        foreach (var it in GameManager.Instance._PeviewMapList)
        {
            it.gameObject.SetActive(false);
        }
        _PreviewCamera.gameObject.SetActive(false);
    }
    public IEnumerator InstantiateReviewMap()
    {
        IsInitMapObj = false;
        //先清楚先前的地图
        if (GameManager.Instance._PeviewMapList.Count > 0)
        {
            foreach (var map in GameManager.Instance._PeviewMapList)
            {
                if(map)
                    Destroy(map.gameObject);
            }
            GameManager.Instance._PeviewMapList.Clear();
        }
        var list = GameManager.Instance.RandomSelectMapIndex;
        Vector3 MapStartPos = GameManager.Instance.PreviewPos.position;
        float lastWidth = 0f;
        if (list.Count > 0)
        {
            foreach (var index in list)
            {
                MapMesh newMap = GameManager.Instance.InitReviewMap(index);
                //添加进容器
                GameManager.Instance._PeviewMapList.Add(newMap);
                var MaxW = newMap.GetMapWidth;
                newMap.transform.position = new Vector3(MaxW * GameManager.Instance.Size+lastWidth,0f,0f)+MapStartPos;
                lastWidth+=MaxW+Offest;
                Vector3 startMapPoint = newMap.StartPoint.GetV3(GameManager.Instance.Size) + newMap.transform.position;
                Instantiate(MapStartPosObj, startMapPoint, Quaternion.identity, newMap.transform);
                
                yield return null;
            }
        }

        IsInitMapObj = true;
    }

    public void DisplayCard()
    {
        if (_ItemCards.Length == 0)
        {
            return;
        }

        float space = (LastPos.position.x - FirstPos.position.x) / (_ItemCards.Length - 1);
        for (int i = 0; i < _ItemCards.Length; i++)
        {
            string Key = "", illustrate = "";
            Sprite sprite = null;

            switch (i)
            {
                case 0:
                    sprite = Resources.Load<Sprite>("Picture/Cut");
                    Key = "C";
                    illustrate = "分裂";
                    break;
                case 1:
                    sprite = Resources.Load<Sprite>("Picture/Increase");
                    Key = "V";
                    illustrate = "伸长";
                    break;
                case 2:
                    sprite = Resources.Load<Sprite>("Picture/Remove");
                    Key = "F";
                    illustrate = "缩短";
                    break;
                case 3:
                    sprite = Resources.Load<Sprite>("Picture/Restart");
                    Key = "R";
                    illustrate = "重启";
                    break;
                case 4:
                    sprite = Resources.Load<Sprite>("Picture/Reverse");
                    Key = "E";
                    illustrate = "反转";
                    break;
            }

            _ItemCards[i].InitCard(MoveAnimCurve, StartPos.position,
                new Vector3(FirstPos.position.x + space * i, FirstPos.position.y, transform.position.z), Key,
                illustrate, 0, sprite);
        }
    }

    public void TurnToGamePanel()
    {
        GameManager.Instance.OnClickStartGame();
    }
}