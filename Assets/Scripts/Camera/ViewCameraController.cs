using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ViewCameraController : MonoBehaviour
{
   public float mXSpeed;
   public float mYSpeed;
   public float MaxYAngle;
   public float MinYAngle;
   private float mx = 0;
   private float my = 0;
   private float mXAngle = 0;
   private float mYAngle = 0;
   public Camera _Camera;
   public float mScrollValue = 1f;
public float ClamDownSpeed=1f;
public float MaxViewScale = 10f;

private void Start()
{
    _Camera = GetComponentInChildren<Camera>();
}

private void Update()
   {
       if (UIManager.Instance.InPreviewMode)
       {
           CameraRotate();  
           CameraMouseScrollWheelFouce();
       }
    
   }

   public void CameraRotate()
   {
       
       if (InputManager.Instance.IsLMouseActive)
       {
           mx = InputManager.Instance.GetMouseDelta.x;
           my = InputManager.Instance.GetMouseDelta.y;
           
       }
       else
       {
           if (InputManager.Instance.IsLMouseUp)
           {
               mx = InputManager.Instance.GetMouseDelta.x;
               my = InputManager.Instance.GetMouseDelta.y;
           }
           else
           {
               mx = Mathf.Lerp(mx, 0f,Time.deltaTime*ClamDownSpeed);
               my = Mathf.Lerp(mx, 0f,Time.deltaTime*ClamDownSpeed);
           }
       }

       mYAngle += my * mYSpeed;
       mXAngle += mx * mXSpeed;
       if (mXAngle > 180f)
       {
           mXAngle -= 360f;
       }
       mYAngle = Mathf.Clamp(mYAngle, MinYAngle, MaxYAngle);
       
       transform.rotation=Quaternion.Euler(-mYAngle,mXAngle,0f);
       
   }

   public void CameraMouseScrollWheelFouce()
   {
   
     // _Camera.orthographicSize = Mathf.Clamp(_Camera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * mScrollValue,1f,MaxViewScale);
   }

   public void SetStartPos(Vector3 pos)
   {
       transform.position = pos;
   }
   public void SetViewTarget(int LastSelect ,int CurSelect,Vector3 pos,float TargerViewSize)
   {
       GameManager.Instance._PeviewMapList[CurSelect].gameObject.SetActive(true);
       StartCoroutine(IE_SetViewTarget(LastSelect,pos,TargerViewSize));
   }

   private float t = 1f;
  
   IEnumerator IE_SetViewTarget(int LastSelect,Vector3 pos,float TargetViewSize)
   {
       float sY = mYAngle;
       float sX = mXAngle;
       Vector3 start = transform.position;
       float StartViewSize = _Camera.orthographicSize;
       MaxViewScale = TargetViewSize*1.5f;
       float a = 0f;
       while (a < t)
       {
         
           var cur = a / t;
           mXAngle  = Mathf.Lerp(sX, 0f, cur);
            mYAngle= Mathf.Lerp(sY, -60f, cur);
            _Camera.orthographicSize = Mathf.Lerp(StartViewSize, MaxViewScale, cur);
           transform.position = Vector3.Lerp(start, pos, cur);
           transform.rotation=Quaternion.Euler(-mYAngle,mXAngle,0f);
           a += Time.deltaTime;
           yield return null;
       }
       mXAngle  = Mathf.Lerp(sX, 0f, 1);
       mYAngle= Mathf.Lerp(sY, -60f, 1);
       _Camera.orthographicSize = Mathf.Lerp(StartViewSize, MaxViewScale, 1);
       transform.position = Vector3.Lerp(start, pos, 1);
       transform.rotation=Quaternion.Euler(-mYAngle,mXAngle,0f);
       
       GameManager.Instance._PeviewMapList[LastSelect].gameObject.SetActive(false);
           
   }
}
