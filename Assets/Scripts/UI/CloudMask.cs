using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using MoreMountains;
using MoreMountains.Feedbacks;

public class CloudMask : MonoBehaviour
{
    public MMF_Player[] Left;
    public MMF_Player End;

  

    public void StartEffect()
    {
        foreach (var it in Left)
        {
            it.gameObject.SetActive(true);
            it.Initialization();
            it.PlayFeedbacks();
            
        }
    }
    public void EndEffect()
    {
        End.Initialization();
      End.PlayFeedbacks();
    }
    public bool IsStartEffectFinish()
    {
        foreach (var it in Left)
        {
            if (it.IsPlaying)
            {
                return false;
            }
        }
        
        return true;
    }
    public bool IsEffectFinish()
    {
        foreach (var it in Left)
        {
            if (it.IsPlaying)
            {
                return false;
            }
        }

        if (End.IsPlaying)
        {
            return false;
        }

        return true;
    }
}
