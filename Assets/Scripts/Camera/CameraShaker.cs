using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField]private MMF_Player _player;
    [SerializeField]private AnimationCurve _curveX;
    [SerializeField]private AnimationCurve _curveY;
    private Vector3 OriganalPos;

    private void Start()
    {
        OriganalPos = transform.localPosition;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        transform.parent.GetComponent<MainViewCameraController>().isNeedUpdate = false;
        bool isCall = false;
        Vector3 originalPos = OriganalPos;
        float elapsed = 0f;
        if (!isCall&&_player)
        {
            isCall = true;
            _player.PlayFeedbacks();
        }
        while (elapsed < duration)
        {
            if (elapsed > duration / 2f)
            {
              
            }
            var per = elapsed / duration;
            float x = _curveX.Evaluate(per) * magnitude;
            float y = -1f* _curveY.Evaluate(per) * magnitude;
            transform.localPosition =originalPos+new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        transform.parent.GetComponent<MainViewCameraController>().isNeedUpdate = true;
    }
    
}
