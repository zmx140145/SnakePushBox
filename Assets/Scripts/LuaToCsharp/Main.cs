 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Main : MonoBehaviour
{
 private void Start()
 {
  Debug.Log("Main Mono脚本");


 }
 public int DoFun(int a,int b)
 {
  return a+b;
 }
 public int DoFun(int a)
 {
  return a;
 }
 public float DoFun(float a)
 {
  return a;
 }
}
