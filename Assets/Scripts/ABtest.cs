using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABtest : MonoBehaviour
{
    private void Start()
    { 
        ABManager.Instance.LoadResourceAsync<GameObject>("object", "Cube", (obj) => { Instantiate(obj); });
      ABManager.Instance.LoadResourceAsync("object", "ABSphere",typeof(GameObject), (obj) =>
      {
          var it = obj as GameObject;
          Instantiate(it);
      });
      
      
    }
}
