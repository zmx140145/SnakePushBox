using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Vector3 StartPos;
    public Vector3 EndPos;
    public float movetime = 1f;
    [HideInInspector] public AnimationCurve _moveCurve;
    public Text _Text;
    public Image _Image;
    public Text _CountText;
    public Text _IllustrateText;

    private void Start()
    {
        originalSize = transform.localScale;
        InputManager.Instance.MoveInput += HandleCard;
    }

    private void Update()
    {
        if (GameManager.Instance._GameMode >= GameMode.InGame)
        {
            UpdateCountext();
        }
    }

    public void InitCard(AnimationCurve _curve, Vector3 startPos, Vector3 TargetPos, string text, string illustrateText,
        int Count, Sprite sprite)
    {
        _moveCurve = _curve;
        StartPos = startPos;
        EndPos = TargetPos;
        transform.position = startPos;
        _Image.sprite = sprite;
        _Text.text = text;
        switch (text)
        {
            case "C":
                GameManager.Instance.CutCount = Count;
                break;
            case "V":
                GameManager.Instance.IncreaseCount = Count;
                break;
            case "F":
                GameManager.Instance.RemoveCount = Count;
                break;
            case "R":
                GameManager.Instance.RestartCount = Count;
                break;
            case "E":
                GameManager.Instance.ReverseCount = Count;
                break;
        }

        if (Count < 0)
        {
            _CountText.text = "";
        }
        else
        {
            _CountText.text = Count.ToString();
        }

        _IllustrateText.text = illustrateText;
        StartCoroutine("IE_MoveToPosWhenInit");
    }

    public void SetCount(int Count)
    {
        if (Count < 0)
        {
            _CountText.text = "";
        }
        else
        {
            _CountText.text = Count.ToString();
        }
    }

    IEnumerator IE_MoveToPosWhenInit()
    {
        yield return new WaitForSeconds(0.45f);
        yield return new WaitUntil(() => { return UIManager.Instance.IsShowCloudEffectFinish(); });
        float a = 0f;
        while (a < movetime)
        {
            var cur = _moveCurve.Evaluate(a / movetime);
            transform.position = Vector3.Lerp(StartPos, EndPos, cur);
            yield return new WaitForFixedUpdate();
            a += Time.fixedDeltaTime;
        }

        transform.position = EndPos;
    }

    public void UpdateCountext()
    {
        switch (_Text.text)
        {
            case "C":

                _CountText.text = GameManager.Instance.CutCount.ToString();
                break;
            case "V":

                _CountText.text = GameManager.Instance.IncreaseCount.ToString();
                break;
            case "F":

                _CountText.text = GameManager.Instance.RemoveCount.ToString();
                break;
            case "R":

                _CountText.text = GameManager.Instance.RestartCount.ToString();
                break;
            case "E":

                _CountText.text = GameManager.Instance.ReverseCount.ToString();
                break;
        }
    }

    private Vector3 originalSize;


    public void OnPointerEnter(PointerEventData eventData)
    {
        StopCoroutine("TurnSmall");
        StartCoroutine("TurnBig");
    }

    IEnumerator TurnBig()
    {
        Vector3 TargetSize = originalSize * GameManager.Instance.CardTargetSize;

        Vector3 CurSize = transform.localScale;
        float a = 0f;
        while (a < GameManager.Instance.CardTurnTime)
        {
            var cur = GameManager.Instance._CardSizeCurve.Evaluate(a / GameManager.Instance.CardTurnTime);
            transform.localScale = Vector3.Lerp(CurSize, TargetSize, cur);

            a += Time.deltaTime;
            yield return null;
        }

        transform.localScale = TargetSize;
    }

    IEnumerator TurnSmall()
    {
        Vector3 TargetSize = originalSize;
        Vector3 CurSize = transform.localScale;
        float a = 0f;
        while (a < GameManager.Instance.CardTurnTime)
        {
            var cur = GameManager.Instance._CardSizeCurve.Evaluate(a / GameManager.Instance.CardTurnTime);
            transform.localScale = Vector3.Lerp(CurSize, TargetSize, cur);
            a += Time.deltaTime;
            yield return null;
        }

        transform.localScale = TargetSize;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine("TurnBig");
        StartCoroutine("TurnSmall");
#if ISTOUCHMODE
        if (GameManager.Instance._GameMode < 3000)
        {
            isNeedHandleCard = true;
        }
        else
        {
            //游戏模式
            switch (_Text.text)
            {
                case "C":
                    if (GameManager.Instance.CutCount > 0)
                    {
                        GameManager.Instance.PlayCutSnakeProp();
                        GameManager.Instance.CutCount--;
                    }

                    break;
                case "V":
                    if (GameManager.Instance.IncreaseCount > 0)
                    {
                        GameManager.Instance.PlayIncreaseSnakeProp();
                        GameManager.Instance.IncreaseCount--;
                    }

                    break;
                case "F":
                    if (GameManager.Instance.RemoveCount > 0)
                    {
                        GameManager.Instance.PlayRemoveSnakeProp();
                        GameManager.Instance.RemoveCount--;
                    }

                    break;
                case "R":
                    if (GameManager.Instance.RestartCount > 0)
                    {
                        GameManager.Instance.PlayReStartProp();
                        GameManager.Instance.RestartCount--;
                    }

                    break;
                case "E":
                    if (GameManager.Instance.ReverseCount > 0)
                    {
                        GameManager.Instance.PlayReverseSnakeProp();
                        GameManager.Instance.ReverseCount--;
                    }

                    break;
            }
        }
#endif
    }

    private void HandleCard(Dir dir)
    {
        if (isNeedHandleCard)
        {
            Debug.Log($"{dir},卡牌");
            if (dir == Dir.Down)
            {
                //左键
                if (GameManager.Instance.HavePropCount > 0)
                {
                    GameManager.Instance.HavePropCount--;
                    switch (_Text.text)
                    {
                        case "C":
                            GameManager.Instance.CutCount++;
                            _CountText.text = GameManager.Instance.CutCount.ToString();
                            break;
                        case "V":
                            GameManager.Instance.IncreaseCount++;
                            _CountText.text = GameManager.Instance.IncreaseCount.ToString();
                            break;
                        case "F":
                            GameManager.Instance.RemoveCount++;
                            _CountText.text = GameManager.Instance.RemoveCount.ToString();
                            break;
                        case "R":
                            GameManager.Instance.RestartCount++;
                            _CountText.text = GameManager.Instance.RestartCount.ToString();
                            break;
                        case "E":
                            GameManager.Instance.ReverseCount++;
                            _CountText.text = GameManager.Instance.ReverseCount.ToString();
                            break;
                    }
                }
            }

            if (dir == Dir.Up)
            {
                //右键
                int curCount = int.Parse(_CountText.text);

                if (curCount > 0)
                {
                    GameManager.Instance.HavePropCount++;
                    switch (_Text.text)
                    {
                        case "C":
                            GameManager.Instance.CutCount--;
                            _CountText.text = GameManager.Instance.CutCount.ToString();
                            break;
                        case "V":
                            GameManager.Instance.IncreaseCount--;
                            _CountText.text = GameManager.Instance.IncreaseCount.ToString();
                            break;
                        case "F":
                            GameManager.Instance.RemoveCount--;
                            _CountText.text = GameManager.Instance.RemoveCount.ToString();
                            break;
                        case "R":
                            GameManager.Instance.RestartCount--;
                            _CountText.text = GameManager.Instance.RestartCount.ToString();
                            break;
                        case "E":
                            GameManager.Instance.ReverseCount--;
                            _CountText.text = GameManager.Instance.ReverseCount.ToString();
                            break;
                    }
                }
            }


            isNeedHandleCard = false;
        }
    }

    private bool isNeedHandleCard = false;

    public void OnPointerClick(PointerEventData eventData)
    {

#if !ISTOUCHMODE
        if (GameManager.Instance._GameMode < 3000)
        {




            if (eventData.pointerId == -1)
            {
                //左键
                if (GameManager.Instance.HavePropCount > 0)
                {
                    GameManager.Instance.HavePropCount--;
                    switch (_Text.text)
                    {
                        case "C":
                            GameManager.Instance.CutCount++;
                            _CountText.text = GameManager.Instance.CutCount.ToString();
                            break;
                        case "V":
                            GameManager.Instance.IncreaseCount++;
                            _CountText.text = GameManager.Instance.IncreaseCount.ToString();
                            break;
                        case "F":
                            GameManager.Instance.RemoveCount++;
                            _CountText.text = GameManager.Instance.RemoveCount.ToString();
                            break;
                        case "R":
                            GameManager.Instance.RestartCount++;
                            _CountText.text = GameManager.Instance.RestartCount.ToString();
                            break;
                        case "E":
                            GameManager.Instance.ReverseCount++;
                            _CountText.text = GameManager.Instance.ReverseCount.ToString();
                            break;
                    }
                }
            }

            if (eventData.pointerId == -2)
            {
                //右键
                int curCount = int.Parse(_CountText.text);

                if (curCount > 0)
                {
                    GameManager.Instance.HavePropCount++;
                    switch (_Text.text)
                    {
                        case "C":
                            GameManager.Instance.CutCount--;
                            _CountText.text = GameManager.Instance.CutCount.ToString();
                            break;
                        case "V":
                            GameManager.Instance.IncreaseCount--;
                            _CountText.text = GameManager.Instance.IncreaseCount.ToString();
                            break;
                        case "F":
                            GameManager.Instance.RemoveCount--;
                            _CountText.text = GameManager.Instance.RemoveCount.ToString();
                            break;
                        case "R":
                            GameManager.Instance.RestartCount--;
                            _CountText.text = GameManager.Instance.RestartCount.ToString();
                            break;
                        case "E":
                            GameManager.Instance.ReverseCount--;
                            _CountText.text = GameManager.Instance.ReverseCount.ToString();
                            break;
                    }
                }
            }
        }
        else
        {
            //游戏模式
            if (eventData.pointerId == -1)
            {
                //左键

                switch (_Text.text)
                {
                    case "C":
                        if (GameManager.Instance.CutCount > 0)
                        {
                            GameManager.Instance.PlayCutSnakeProp();
                            GameManager.Instance.CutCount--;
                        }

                        break;
                    case "V":
                        if (GameManager.Instance.IncreaseCount > 0)
                        {
                            GameManager.Instance.PlayIncreaseSnakeProp();
                            GameManager.Instance.IncreaseCount--;
                        }

                        break;
                    case "F":
                        if (GameManager.Instance.RemoveCount > 0)
                        {
                            GameManager.Instance.PlayRemoveSnakeProp();
                            GameManager.Instance.RemoveCount--;
                        }

                        break;
                    case "R":
                        if (GameManager.Instance.RestartCount > 0)
                        {
                            GameManager.Instance.PlayReStartProp();
                            GameManager.Instance.RestartCount--;
                        }

                        break;
                    case "E":
                        if (GameManager.Instance.ReverseCount > 0)
                        {
                            GameManager.Instance.PlayReverseSnakeProp();
                            GameManager.Instance.ReverseCount--;
                        }

                        break;
                }
            }
        }
#endif

        

    }
}