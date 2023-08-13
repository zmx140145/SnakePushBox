using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalWords
{
    public const string W_Tutorial = "Words/Tutorial";
    public const string W_Global = "Words/Global";

    public static TextAsset LoadTextAsset(string name)
    {
        return  (TextAsset)Resources.Load(name);
    }
  
}
