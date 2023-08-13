using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WordAssistant : MonoBehaviour,OperaItem
{
    public GameObject WordText;
    public GameObject StartSayObject;
    private Text _text;
    private Typewriter _typewriter;
    public float WaitTime;
    public int CurStateShow;
    public int DestroyKey;
    private void Start()
    {
        _text = WordText.GetComponent<Text>();
        _typewriter = WordText.GetComponent<Typewriter>();
        _typewriter.PrintFinish += SayWordFinish;
         Init(1000,WordPlayState.MoveNext);
         GameManager.Instance._GameMode = 1000;
         // gameObject.SetActive(false);
    }


    private void Update()
    {
        CurStateShow = CurState;
    }

    public WordMessage SayAutoWord()
    {
      WordMessage wordMessage=  WordManager.Instance.ReadWord(_TextAsset, CurState);
      if (wordMessage != null)
      {
          StopCoroutine("IE_SayWordFinfish");
          InSayWord = true;
          SayWord(wordMessage.Word);
      }

      return wordMessage;
    }
    public void SayWord(string word)
    {
        _text.text = word;
        WordText.SetActive(false);
        StartSayObject.SetActive(true);
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
        StartSayObject.SetActive(false);
        InSayWord = false;
        if (NeedDelete)
        {
            Destroy(gameObject);
        }
    }

    public TextAsset _TextAsset { get; set; }
    public int LastState { get; set; }
    public int CurState { get; set; }
    public int NextState { get; set; }
    public WordPlayState _PlayMode { get; set; }

    public void ChangNextState(int nextState,WordPlayState nextplaymode)
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
            else
            {
                NextState = a.ToState;
            }
            
        }
      
        return a;
    }
    



    public void Init(int state,WordPlayState playMode)
    {
        _TextAsset = GlobalWords.LoadTextAsset(GlobalWords.W_Tutorial);
        _PlayMode = playMode;
        CurState = state;
        WordManager.Instance.AddOperaItemToList(this);
    }

    private bool NeedDelete = false;
    public void CheckSelf(int state)
    {
        if (CurState >= DestroyKey)
        {
            if (InSayWord)
            {
                NeedDelete = true;
            }
            else
            {
                //说明不需要前置导航动作了
                UIManager.Instance.GetTopPanel(UIPanelType.Start).GetComponent<StartPanel>()._StartGameButton
                    .ClickPointCount = 3;
                Destroy(gameObject);
                
            }
           
           
        }
    }
}
