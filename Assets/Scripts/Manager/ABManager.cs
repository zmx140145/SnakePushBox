using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 1.ab包相关Api
/// 2.单例模式
/// 3.委托+lambda表达式
/// 4.协程
/// 5.字典
/// </summary>
[DefaultExecutionOrder(-3)]
public class ABManager : Singleton<ABManager>
{
    //储存加载过的包
    private Dictionary<string, AssetBundle> _ABDic = new Dictionary<string, AssetBundle>();
    private AssetBundle _MainAB = null;
    private AssetBundleManifest _MainManifest = null;
    private string PathURL
    {
        get
        {
            return Application.streamingAssetsPath + "/" ;
        }
    }

    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }
    #region 同步加载

 

    public void LoadDependenceAB(string abName)
    {
        //加载主包
        if (_MainAB == null)
        {
            _MainAB=AssetBundle.LoadFromFile(PathURL+MainABName);
            _MainManifest = _MainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        //获得依赖包相关信息
        string[] strs = _MainManifest.GetAllDependencies(abName);
        //加载依赖包
        for (int i = 0; i < strs.Length; i++)
        {
            if (!_ABDic.ContainsKey(strs[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathURL + strs[i]);
                _ABDic.Add(strs[i],ab);
            }
        }
       
        if (!_ABDic.ContainsKey(abName))
        {
            //加载AB包
            _ABDic.Add(abName,AssetBundle.LoadFromFile(PathURL + abName));
        }
      
    }
    public Object LoadResource(string abName, string resName)
    {
       LoadDependenceAB(abName);
      
        //加载需要的资源
        return _ABDic[abName].LoadAsset(resName);

    }
    public  Object LoadResource(string abName, string resName,System.Type type) 
    {
        LoadDependenceAB(abName);
      
        //加载需要的资源
        return _ABDic[abName].LoadAsset(resName,type);
    

    }
    public  T LoadResource<T>(string abName, string resName) where T:Object
    {
        LoadDependenceAB(abName);
       
        //加载需要的资源
        return _ABDic[abName].LoadAsset(resName,typeof(T)) as T;
    

    }
    #endregion

    #region 异步加载

    //从AB中加载资源异步加载
    public void LoadResourceAsync(string abName, string resName, UnityAction<Object> callback)
    {
        StartCoroutine(IE_LoadResourceAsync(abName, resName, callback));
    }

    private IEnumerator IE_LoadResourceAsync(string abName, string resName, UnityAction<Object> callback)
    { 
        LoadDependenceAB(abName);
        
        //加载需要的资源
      AssetBundleRequest abr=_ABDic[abName].LoadAssetAsync(resName);
       yield return abr;
       callback?.Invoke(abr.asset);
    }
    
    public void LoadResourceAsync(string abName, string resName,Type type, UnityAction<Object> callback)
    {
        StartCoroutine(IE_LoadResourceAsync(abName, resName,type, callback));
    }

    private IEnumerator IE_LoadResourceAsync(string abName, string resName,Type type, UnityAction<Object> callback)
    { 
        LoadDependenceAB(abName);
        
        //加载需要的资源
        AssetBundleRequest abr=_ABDic[abName].LoadAssetAsync(resName,type);
        yield return abr;
        callback?.Invoke(abr.asset);
    }
    public void LoadResourceAsync<T>(string abName, string resName, UnityAction<T> callback)where T:Object
    {
        StartCoroutine(IE_LoadResourceAsync<T>(abName, resName, callback));
    }

    private IEnumerator IE_LoadResourceAsync<T>(string abName, string resName, UnityAction<T> callback)where T:Object
    { 
        LoadDependenceAB(abName);
        
        //加载需要的资源
        AssetBundleRequest abr=_ABDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        callback?.Invoke(abr.asset as T);
    }
    #endregion
    
    
    #region 卸载

    public void UnloadAB(string abName)
    {
        if (_ABDic.ContainsKey(abName))
        {
            _ABDic[abName].Unload(false);
            _ABDic.Remove(abName);
        }
    }

    public void UnloadAllAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        _ABDic.Clear();
        _MainAB = null;
        _MainManifest = null;
    }

    #endregion

}
