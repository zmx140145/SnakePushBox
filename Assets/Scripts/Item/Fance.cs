

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Fance : Item,CanIn
{

  
 
 
    public Dir[] AcceptDir;
  
    private bool isCancel = false;
    
    
    public override void Init()
    {
      
    }
 
    public override bool CanMoveIn(object[] args)
    {
        
            Dir dir = (Dir)args[0];
            
            return AcceptDir.Contains(dir);
        
            
    }

    public override bool CanDestory(int type)
    {
        return false;
    }

    public override MoveEffecState GetMoveEffect(object[] args)
    {
       
            Dir FromDir = (Dir)args[1];
            foreach (var it in AcceptDir)
            {
                if (it == FromDir)
                {
                    return MoveEffecState.NoEffect;
                }
            }
            return MoveEffecState.MoveFail; 
       
    
    }


    public void RoleMoveIn(SnakeMesh _role)
    {
      
    }

    public void RoleMoveOut(SnakeMesh _role)
    { 
        
      
    }
}

