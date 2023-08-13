
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class MainViewCameraController : MonoBehaviour
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
public Transform OriganalTransform;

public bool isNeedUpdate = true;
private void Update()
   {
       if (GameManager.Instance._GameMode >= 3000)
       {
           CameraRotate();  
           CameraMouseScrollWheelFouce();
       }
       else
       {
           if (isNeedUpdate)
           {
               if (Vector3.Distance(transform.position, OriganalTransform.position) > 0.05f)
               {
                   transform.position =Vector3.Lerp(transform.position,OriganalTransform.position,Time.deltaTime);
              
               }
               else
               {
                   transform.position = OriganalTransform.position;
               }

               if (Quaternion.Angle(transform.rotation, OriganalTransform.rotation) > 0.1f)
               {
                   transform.rotation = Quaternion.Lerp(transform.rotation, OriganalTransform.rotation, Time.deltaTime);
               }
               else
               {
                   transform.rotation=OriganalTransform.rotation;
               }
           }
         
       }
         
    
   }

   public void CameraRotate()
   {
       
       if (Input.GetMouseButton(0))
       {
           mx = Input.GetAxis("Mouse X");
           my = Input.GetAxis("Mouse Y");
           
       }
       else
       {
           if (Input.GetMouseButtonUp(0))
           {
               mx = Input.GetAxis("Mouse X");
               my = Input.GetAxis("Mouse Y");
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
   
     _Camera.orthographicSize = Mathf.Clamp(_Camera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * mScrollValue,1f,MaxViewScale);
   }

   public void SetStartPos(Vector3 pos)
   {
       transform.position = pos;
   }
   public void SetStartPos(Transform tran)
   {
       transform.position = tran.position;
       transform.rotation = tran.rotation;
   }
   public void SetViewTarget(Vector3 pos,float TargerViewSize)
   {
       StartCoroutine(IE_SetViewTarget(pos,TargerViewSize));
   }

   private float t = 1f;
   public float DefalutY;
   public float DefalutX;
   IEnumerator IE_SetViewTarget(Vector3 pos,float TargetViewSize)
   {
       float sY = mYAngle;
       float sX = mXAngle;
       Vector3 start = transform.position;
       float StartViewSize = _Camera.orthographicSize;
       MaxViewScale = TargetViewSize+0.2f;
       float a = 0f;
       while (a < t)
       {
         
           var cur = a / t;
           mXAngle  = Mathf.Lerp(sX, DefalutX, cur);
            mYAngle= Mathf.Lerp(sY, DefalutY, cur);
            _Camera.orthographicSize = Mathf.Lerp(StartViewSize, MaxViewScale, cur);
           transform.position = Vector3.Lerp(start, pos, cur);
           transform.rotation=Quaternion.Euler(-mYAngle,mXAngle,0f);
           a += Time.deltaTime;
           yield return null;
       }
       mXAngle  = Mathf.Lerp(sX, DefalutX, 1);
       mYAngle= Mathf.Lerp(sY, DefalutY, 1);
       _Camera.orthographicSize = Mathf.Lerp(StartViewSize, MaxViewScale, 1);
       transform.position = Vector3.Lerp(start, pos, 1);
       transform.rotation=Quaternion.Euler(-mYAngle,mXAngle,0f);
       
      
           
   }
}
