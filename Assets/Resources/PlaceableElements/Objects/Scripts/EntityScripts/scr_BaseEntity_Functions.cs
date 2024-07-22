using System;
using UnityEngine;
public static class scr_BaseEntity_Functions
{
    //Basic jump counter to provide a movement force for either acceleration or a special movement control. 
    public static float bounce(ref float jumpCounter, float maxJumpCounter, float decaySpeed, float moveForce)
    {
        float jumpForce = 0f;
        
        jumpForce = (jumpCounter/maxJumpCounter) * moveForce;

        jumpCounter -= decaySpeed * Time.deltaTime;

        return jumpForce;
    }
}