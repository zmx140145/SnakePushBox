using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;

[DefaultExecutionOrder(-3)]
public class UIManager : Singleton<UIManager>
{
    // public UIPanel[] _panel;
    // private Stack<UIPanel> PopOutPanel;
    // private Stack<UIPanel> NormalPanel;
    // private Stack<UIPanel> MiddlePanel;
    // private Dictionary<UIPanelType, UIPanel> _InScenePanels=new Dictionary<UIPanelType, UIPanel>();
    public UIPanel[] UIPanels;
    public Dictionary<UIPanelType, GameObject> UIPanelDict = new Dictionary<UIPanelType, GameObject>();
    public Dictionary<UIPanelType, UIPanel> UIPanelInScene = new Dictionary<UIPanelType, UIPanel>();
    private Stack<UIPanel> UiStack = new Stack<UIPanel>();

    public int[] UIPanelInStackCount; //用枚举类型转int作为下标 记录每个类型的激活数量
    public Transform UiFather;
    public CloudMask _CloudMask;
    public bool InPreviewMode = false;

    private void Start()
    {
        OnInit();
    }

    public void OnInit()
    {
        //从GameFace中获得所有的UI面板并存入字典
        foreach (UIPanel ui in UIPanels)
        {
            UIPanelDict.Add(ui.GetUIPanelType, ui.gameObject);
        }

        UIPanelInStackCount = new int[(int)UIPanelType.End + 1];
    }

    /// <summary>
    /// 激活UI  
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns></returns>
    public bool PushPanel(UIPanelType panelType, bool remainActive = false) //激活paneltype,是否保持原来ui激活
    {
        if (UIPanelInScene.TryGetValue(panelType, out UIPanel NeedPushUI))
        {
            //记录已生成面板的字典里找到了对应的面板 直接提取出来操作
            if (UiStack.Count > 0 && !remainActive)
            {
                //如果stack上有UI说明场景里有正在激活的ui那么就要先停止它
                UIPanel NeedPauseUI = UiStack.Peek();
                NeedPauseUI.OnPause();
            }

            //把需要加入的UI加到stack 并激活它
            UiStack.Push(NeedPushUI);
            //因为肯定能加到所以直接计数
            UIPanelInStackCount[(int)NeedPushUI.GetUIPanelType]++;
            NeedPushUI.OnEnter();
            OrderPanel();
            return true;
        }
        else
        {
            //如果字典你没找到，说明需要去生成一个
            if (UiStack.Count > 0 && !remainActive)
            {
                //如果stack上有UI说明场景里有正在激活的ui那么就要先停止它
                UIPanel NeedPauseUI = UiStack.Peek();
                NeedPauseUI.OnPause();
            }

            if (PushPanelNoOnScene(panelType) != null)
            {
                OrderPanel();
                return true;
            }
            else
            {
                Debug.Log("没有查询到这个类型的UI面板，请确认添加！");
                return false;
            }
        }
    }


    /// <summary>
    ///  把不存在的ui放到界面里
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns></returns>
    private UIPanel PushPanelNoOnScene(UIPanelType panelType)
    {
        if (UIPanelDict.TryGetValue(panelType, out GameObject obj))
        {
            //生成Panel到场景
            GameObject gameObject = GameObject.Instantiate(obj, UiFather);
            //要用新生成出来的GameObject来得到NeedPushUI 不然得到的UIpanel并不是场景中的
            UIPanel NeedPushUI = gameObject.GetComponent<UIPanel>();
            UIPanelInStackCount[(int)NeedPushUI.GetUIPanelType]++;
            UiStack.Push(NeedPushUI);
            //让panel激活
            NeedPushUI.OnEnter();
            //把Panel注册到字典里
            UIPanelInScene.Add(panelType, NeedPushUI);

            return NeedPushUI;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 退出UI
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns></returns>
    public bool ExitPanel(UIPanelType panelType,bool isNeedRecovery=true)
    {
        if (UiStack.Count > 0 && UiStack.Peek().GetUIPanelType == panelType)
        {
            ThrowPanelOut(isNeedRecovery);
            OrderPanel();
            
            return true;
        }
        else
        {
            //删除那些需要删除但是存在栈底的
            if (UIPanelInStackCount[(int)panelType] > 0)
            {
                Debug.Log("删除中间");
                RemovePanelFromStack(panelType);
                UIPanelInScene.Remove(panelType);
                OrderPanel();
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 退出并恢复上一层
    /// </summary>
    private void ThrowPanelOut(bool isNeedRecovery)
    {
        if (UiStack.Count > 0)
        {
            UIPanel NeedThrow = UiStack.Pop();
            --UIPanelInStackCount[(int)NeedThrow.GetUIPanelType];
            //弹出panel不一定是删除也可能是停止 要进行判断
            if (UIPanelInStackCount[(int)NeedThrow.GetUIPanelType] > 0)
            {
               
                    NeedThrow.OnPause();
                
              
            }
            else
            {
                UIPanelInScene.Remove(NeedThrow.GetUIPanelType);
                NeedThrow.OnExit();
            }

            //因为弹出了一次面板 所以要重新判断是不是大于0
            if (UiStack.Count > 0)
            {
                UIPanel NeedActive = UiStack.Peek();
                if (!NeedActive.isActive)
                {
                    if (isNeedRecovery)
                    {
                        NeedActive.OnRecovery();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 找到stack需要删除的激活Panel并移除
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private void RemovePanelFromStack(UIPanelType type)
    {
        Stack<UIPanel> stack = new Stack<UIPanel>();

        while (UiStack.Count > 0)
        {
            UIPanel ui = UiStack.Pop();
            if (ui.GetUIPanelType == type)
            {
                UIPanelInStackCount[(int)ui.GetUIPanelType]--;
                if (UIPanelInStackCount[(int)ui.GetUIPanelType] > 0)
                {
                    ui.OnPause();
                }
                else
                {
                    ui.OnExit();
                }

                break;
            }

            stack.Push(ui);
        }

        while (stack.Count > 0)
        {
            UiStack.Push(stack.Pop());
        }

        stack.Clear();
    }


    public GameObject GetTopPanel(UIPanelType type)
    {
        if (UiStack.Peek().GetUIPanelType == type)
        {
            return UiStack.Peek().gameObject;
        }
        else
        {
            return null;
        }
    }

    public GameObject GetActivePanel(UIPanelType type)
    {
        IEnumerable<UIPanel> query =
            from n in UiStack
            where (n.GetUIPanelType == type) && n.isActive
            select n;
        if (query.ToList().Count > 0)
        {
            if (UIPanelInScene.TryGetValue(query.ToList().First().GetUIPanelType, out var dit))
            {
                return dit.gameObject;
            }
        }

        return null;
    }


    public void OrderPanel()
    {
        int[] flag = new int[(int)UIPanelType.End + 1];
        foreach (var it in UiStack)
        {
            if (it.isActive)
            {
                Debug.LogWarning(it.GetUIPanelType);
                if (flag[(int)it.GetUIPanelType] < 1)
                {
                    it.gameObject.transform.SetAsFirstSibling();
                    flag[(int)it.GetUIPanelType]++;
                }
            }
        }
    }

    public void ShowCloudMask(bool BlockUI = true)
    {

        if (!inShowIE)
        {
            if (!BlockUI)
            {
                Vector3 v3 = _CloudMask.gameObject.transform.localPosition;
                v3.z = 1080f;
                _CloudMask.gameObject.transform.localPosition = v3;
            }
            else
            {
                Vector3 v3 = _CloudMask.gameObject.transform.localPosition;
                v3.z = 80f;
                _CloudMask.gameObject.transform.localPosition = v3;
            }

            StartCoroutine("IE_ShowCloudMask");
        }
    }

    private  bool inShowIE = false;

    IEnumerator IE_ShowCloudMask()
    {
        inShowIE = true;
        yield return new WaitUntil(() =>
        {
            return _CloudMask.IsEffectFinish();
        });
        _CloudMask.StartEffect();
        yield return new WaitUntil(() =>
        {
            return _CloudMask.IsEffectFinish();
        });
        inShowIE = false;
    }

    
    public  bool IsShowCloudEffectFinish()
    {
        return !inShowIE;
    }

    public void HideCloudMask()
    {
        if (!inHideIE)
        {
            StartCoroutine("IE_HideCloudMask");
        }

    }

    private bool inHideIE = false;

    IEnumerator IE_HideCloudMask()
    {
        inHideIE = true;
        yield return new WaitUntil(() =>
        {
            return _CloudMask.IsEffectFinish();
        });

        _CloudMask.EndEffect();
        yield return new WaitUntil(() =>
        {
            return _CloudMask.IsEffectFinish();
        });
        inHideIE = false;
    }

 
  
}