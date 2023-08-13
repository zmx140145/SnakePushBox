using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMap : MapMesh ,OperaItem
{
    public Canvas _Canvas;
    public GameObject WordText;
    public GameObject FinalPosObj;
    private Text _text;
    private Typewriter _typewriter;
    public float WaitTime;
    public int DestroyKey;
    public TextAsset _TextAsset { get; set; }
    public int LastState { get; set; }
    public int CurState { get; set; }
    public int NextState { get; set; }
    public WordPlayState _PlayMode { get; set; }

    public bool IsStartSayWord=false;
    public int SayWordState;
    private void Start()
    {
        _TextAsset = GlobalWords.LoadTextAsset(GlobalWords.W_Tutorial);
        _Canvas.worldCamera=Camera.main;
        _text = WordText.GetComponent<Text>();
        _typewriter = WordText.GetComponent<Typewriter>();
        _typewriter.PrintFinish += SayWordFinish;
        if (IsStartSayWord)
        {
            CurState
                = SayWordState;
            PlayState(WordPlayState.MoveNext);

        }
       
        
    }

    public override void InitMyTutorial(int state)
    {
      Init(state,WordPlayState.MoveNext);
      if (state <= 1004)
      {
          Debug.Log("MapSay");
          PlayState(_PlayMode);
      }
    }
    //根据剧情自动识别说话内容
    private WordMessage SayAutoWord()
    {
        if (CurState == 1004)
        {
            FinalPosObj?.SetActive(true);
        }
        else
        {
            FinalPosObj?.SetActive(false);
        }
        WordMessage wordMessage=  WordManager.Instance.ReadWord(_TextAsset, CurState);
        if (wordMessage != null)
        {
            StopCoroutine("IE_SayWordFinfish");
            InSayWord = true;
            SayWord(wordMessage.Word);
        }

        return wordMessage;
    }
    //真正说话的地方
    public void SayWord(string word)
    {
        _text.text = word;
        WordText.SetActive(false);
        WordText.SetActive(true);
    }

    private void SayWordFinish()
    {
        
        StartCoroutine("IE_SayWordFinfish");
    }

    public bool InSayWord = false;
    IEnumerator IE_SayWordFinfish()
    {
  
        yield return new WaitForSeconds(WaitTime);
        WordText.SetActive(false);
     
        InSayWord = false;
    }

    public void ChangNextState(int nextState, WordPlayState nextplaymode)
    {
        LastState = CurState;
        CurState = nextState;
        _PlayMode = nextplaymode;
    }

    public WordMessage PlayState(WordPlayState playmode)
    {
        WordMessage a = SayAutoWord();
        if (a!=null)
        { 
            if (playmode == WordPlayState.MoveNext)
            {
                ChangNextState(a.ToState,a.PlayState);
            }
            
        }
      
        return a;
    }

    public void Init(int state, WordPlayState playmode)
    {
       
        CurState = state;
        this._PlayMode = playmode;
        WordManager.Instance.AddOperaItemToList(this);
    }
    private bool NeedDelete = false;
    public void CheckSelf(int state)
    {
        if (CurState >= DestroyKey)
        {
            if (GameManager.Instance._Map == this)
            {
                GameManager.Instance._Map = null;
            }
            Destroy(gameObject);
        }
    }
}
