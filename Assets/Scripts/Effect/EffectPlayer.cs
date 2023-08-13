using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
   public ParticleSystem _ParticleSystem;
   public EffectType EffectType;
   public float time;
   private void Start()
   {
      _ParticleSystem = GetComponentInChildren<ParticleSystem>();
      StartCoroutine("PlayingFinishDestroy");
      
   }

   IEnumerator PlayingFinishDestroy()
   {
      float a = 0f;
      while (a<time)
      {
         a += Time.deltaTime;
         yield return null;
      }
      Destroy(gameObject);
   }
}
