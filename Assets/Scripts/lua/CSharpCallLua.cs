using System;
using System.Collections;
using System.Collections.Generic;
using XLua;
using UnityEngine;
using UnityEngine.Events;


public static class CSharpCallLua
{
    [CSharpCallLua]
    public static List<Type> csharpCallLuaList = new List<Type>(){
     typeof(UnityAction),
  typeof(UnityAction<bool>)
 };

}
