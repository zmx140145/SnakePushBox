using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface OperaItem
{
    public TextAsset _TextAsset { get; set; }
    public int LastState { get; set; }
    public int CurState { get; set; }
    public int NextState { get; set; }
    public WordPlayState _PlayMode { get; set; }
    public void ChangNextState(int nextState,WordPlayState nextplaymode);
    public WordMessage PlayState(WordPlayState playmode);
    public void Init(int state,WordPlayState playmode);

    /// <summary>
    /// 通过通知触发  检查自己是否需要变更状态
    /// </summary>
    /// <param name="state"></param>
    public void CheckSelf(int state);

}

public enum WordPlayState
{
    Repeated,MoveNext
}
public class WordMessage
{
    public string Word;
    public int CurState;
    public int ToState;
    public WordPlayState PlayState;
}
public class WordManager : Singleton<WordManager>
{
    public TextAsset DataFile;
    public Dictionary<TextAsset, OperaItem> _OperaItemsDic = new Dictionary<TextAsset, OperaItem>();
    private void Start()
    {
        DataFile = GlobalWords.LoadTextAsset(GlobalWords.W_Global);
    }

    public void AddOperaItemToList(OperaItem _item)
    {
        _OperaItemsDic[_item._TextAsset] = _item;
    }
    public void RemoveOperaItemFromList(OperaItem _item)
    {
        if (_OperaItemsDic.ContainsKey(_item._TextAsset))
        {
            _OperaItemsDic.Remove(_item._TextAsset);
        }
    }

    public void NoticeOperaNewState(int State)
    {
        GameManager.Instance._GameMode = State;
        foreach (var it in _OperaItemsDic)
        {
            it.Value.CurState = State;
            it.Value.CheckSelf(State);
        }
    }
    public WordMessage ReadWord(TextAsset Words, int state)
    {
        string[] rows = Words.text.Split('\n');
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].Length > 0)
            {
                if (rows[i][0] == '$')
                {
                    //正在读取
                    string[] coll = rows[i].Split(',');
                    if (coll.Length > 1)
                    {
                        if (int.TryParse(coll[1],out var cur))
                        {
                            if (cur == state)
                            {
                                int count = coll.Length;
                                if (count >= 3)
                                {
                                    string w = "";
                                    if (int.TryParse(coll[2], out var next))
                                    {
                                        for (int j = 3; j < count-1; j++)
                                        {
                                            w += coll[j];
                                        }

                                        if (int.TryParse(coll[count - 1], out var playstate))
                                        {
                                            return new WordMessage() { CurState = state, ToState = next, Word = w,PlayState = (WordPlayState)playstate};
                                        }
                                        else
                                        {
                                            return new WordMessage() { CurState = state, ToState = next, Word = w,PlayState = (WordPlayState)WordPlayState.MoveNext};
                                        }
                                      
                                    }
                                  
                                    
                                }
                                
                               
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("文档错误 $后没有数据");
                    }
                }
            }
        }

        return null;
    }
}
