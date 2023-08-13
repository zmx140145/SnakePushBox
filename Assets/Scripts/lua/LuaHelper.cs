using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;


//AB包打包不能识别 .lua 需要把lua为txt

public class LuaHelper : Singleton<LuaHelper>
{
    private LuaEnv _LuaEnv;

    public LuaTable Global
    {
        get
        {
            return _LuaEnv.Global;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
       Init();
      
       LuaHelper.Instance.DoLuaFile("Main");
       
    }

    public delegate void aaa(int key);

    public event aaa event_aaa;
    public void Init()
    {
        if (_LuaEnv != null)
        {
            return;
        }

        _LuaEnv = new LuaEnv();
        // _LuaEnv.AddLoader(MyCustomLoaderAB);
        _LuaEnv.AddLoader(MyCustomLoader);

    }
    
/// <summary>
/// 执行lua语言
/// </summary>
/// <param name="str"></param>
    public void DoString(string str)
    {
        if (_LuaEnv == null)
        {
            Debug.LogError("解析器为Null");
            return;
        }
        _LuaEnv.DoString(str);
    }
public void DoLuaFile(string str)
{
    string a = String.Format("require('{0}')", str);
    if (_LuaEnv == null)
    {
        Debug.LogError("解析器为Null");
        return;
    }
    _LuaEnv.DoString(a);
}
/// <summary>
/// 释放垃圾
/// </summary>
    public void Tick()
    {
        if (_LuaEnv == null)
        {
            Debug.LogError("解析器为Null");
            return;
        }
        _LuaEnv.Tick();
    }
/// <summary>
/// 销毁解析器
/// </summary>
    public void Dispose()
    {
        if (_LuaEnv == null)
        {
            Debug.LogError("解析器为Null");
            return;
        }
        _LuaEnv.Dispose();
        _LuaEnv = null;
    }
    
    //函数自动执行
    private byte[] MyCustomLoader(ref string filePath)
    {
        string path = Application.dataPath + "/Lua/" + filePath + ".lua";
        if (File.Exists(path))
        {
        return  File.ReadAllBytes(path);
        }
        Debug.LogError($"重定向失败:文件名为{filePath}");
        return null;
    }
    //函数自动执行
    private byte[] MyCustomLoaderAB(ref string filePath)
    {
        string FileName = filePath + ".lua";
       TextAsset ta= ABManager.Instance.LoadResource("lua", FileName,typeof(TextAsset)) as TextAsset;
       if (ta != null)
       {
           return ta.bytes;
       }
       Debug.LogError($"重定向失败:文件名为{filePath}");
        return null;
    }
}
