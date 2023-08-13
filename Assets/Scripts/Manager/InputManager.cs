using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : Singleton<InputManager>
{
    private PlayerInput _playerInput;
    public Action<Dir> MoveInput;
 

    public bool GetCutKeyDown
    {
        get
        {
            return _playerInput.Player.Cut.WasPressedThisFrame();
        }
    }
    public bool GetAddKeyDown
    {
        get
        {
            return _playerInput.Player.Add.WasPressedThisFrame();
        }
    }
    public bool GetRemoveKeyDown
    {
        get
        {
            return _playerInput.Player.Remove.WasPressedThisFrame();
        }
    }
    public bool GetReverseKeyDown
    {
        get
        {
            return _playerInput.Player.Reverse.WasPressedThisFrame();
        }
    }
    public bool GetRestartKeyDown
    {
        get
        {
            return _playerInput.Player.Restart.WasPressedThisFrame();
        }
    }
    public bool GetEscDown
    {
        get
        {
            return _playerInput.Player.Esc.WasPressedThisFrame();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _playerInput = new PlayerInput();
        _playerInput.Enable();
       
    }

    private void Start()
    {
        _playerInput.Player.TouchPress.started += ctx=> OnFingerDown(ctx);
        _playerInput.Player.TouchPress.canceled += ctx=> OnFingerUp(ctx);
    }

    private void Update()
    {
       CheckKeyBoardInputDir();


    }
    public Vector2 v2;
    public bool isv2;
    public void CheckKeyBoardInputDir()
    {
        v2 = _playerInput.Player.Move.ReadValue<Vector2>();
        isv2 = _playerInput.Player.Move.WasPressedThisFrame();
        if (isv2)
        {
        
            if (v2 == Vector2.up)
            {
                MoveInput?.Invoke(Dir.Up);
            }
            if (v2 == Vector2.down)
            {
                MoveInput?.Invoke(Dir.Down);
            }
            if (v2 == Vector2.left)
            {
                MoveInput?.Invoke(Dir.Left);
            }
            if (v2 == Vector2.right)
            {
                MoveInput?.Invoke(Dir.Right); 
            }
        }
    
    }


    
    public double startTime;
   
    public Vector2 TouchDelta;
    public Vector2 TotalDelta;
    public double TouchTime;
    public bool isFingerActive = false;
    
   

    public bool IsLMouseActive
    {
        get
        {
           return _playerInput.Player.MouseLeftDown.IsPressed();
        }
    }
    public bool IsLMouseUp
    {
        get
        {
            return _playerInput.Player.MouseLeftDown.WasReleasedThisFrame();
        }
    }
    public Vector2 GetMouseDelta
    {
        get
        {
            return _playerInput.Player.MouseDelta.ReadValue<Vector2>();
        }
    }

    private Vector2 TouchDownPos;
    private void OnFingerDown(InputAction.CallbackContext callbackContext)
    {
       
        isFingerActive = true;
        TouchDownPos = _playerInput.Player.TouchPosition.ReadValue<Vector2>();
        TouchTime = callbackContext.time;
    }
    private void OnFingerUp(InputAction.CallbackContext callbackContext)
    {
     
        TotalDelta = _playerInput.Player.TouchPosition.ReadValue<Vector2>() - TouchDownPos;
            TouchDownPos = Vector2.zero;
            TouchTime = callbackContext.time-TouchTime;
            isFingerActive = false;
            if (TouchTime > 0.1f&&TotalDelta.sqrMagnitude>10f)
            {
                while (TotalDelta != Vector2.zero)
                {
                   float a= Vector2.Dot(Vector2.right, TotalDelta.normalized);
                   if (a > 0.8f)
                   {
                    MoveInput?.Invoke(Dir.Right);   
                    break;
                   }
                   a= Vector2.Dot(Vector2.up, TotalDelta.normalized);
                   if (a > 0.8f)
                   {
                       MoveInput?.Invoke(Dir.Up);  
                       break;
                   }
                   a= Vector2.Dot(Vector2.left, TotalDelta.normalized);
                   if (a > 0.8f)
                   {
                       MoveInput?.Invoke(Dir.Left);  
                       break;
                   }
                   a= Vector2.Dot(Vector2.down, TotalDelta.normalized);
                   if (a > 0.8f)
                   {
                       MoveInput?.Invoke(Dir.Down);  
                       break;
                   }
                   TotalDelta=Vector2.zero;
                   break;
                }
            }
        
    }
}
