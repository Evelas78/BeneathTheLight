using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class scr_MainChar_Main : scr_Basic_Entity
{
    [SerializeField] private scr_Raycasts_Main raycasterComp = null;
    [SerializeField] private scr_Acceleration_Component accelComp = null;

    private float maxProg_X, maxProg_PosY, maxProg_NegY; //Realistically, itll never touch this as the multiplier will decrease the rate of growth
    public override void CharacterAwake()
    {
        maxProg_X =  (4f * Time.fixedDeltaTime);
        maxProg_PosY = (2f * Time.fixedDeltaTime);
        maxProg_NegY = (1f * Time.fixedDeltaTime);

        objectIDs.versionType = GLOBAL_CONSTANTS.entityType.isPlayer;

        raycasterComp = gameObject.AddComponent<scr_Raycasts_Main>();
        raycasterComp.currCharCollider = currentCollider;
        raycasterComp.depthBasedCollision = true;

        raycasterComp.boxSize_X_factor = 1f;
        raycasterComp.boxSize_Y_factor = 1/16f;

        //Origin of the boxCast will be relative to the position we give. So starting it at the Y (which is the bottom middle of the object)
        accelComp = gameObject.AddComponent<scr_Acceleration_Component>();  
        accelComp.gFrictionApplies = true;
        accelComp.gravApplies = true;
    }
    [SerializeField] private int left,right;
    [SerializeField] private int movingHorizontal = 0,movingVertical = 0;
    [SerializeField] private int prev_movingHorizontal = 0 ,prev_movingVertical = 0;

    private float maxStrength_X = 1.5f, minStrength_X = .75f;
    private float maxStrength_PosY = 2f, minStrength_PosY = 1f;    
    private float maxStrength_NegY = 1.5f, minStrength_NegY = .75f;   
    [SerializeField] private float MovementStrength_X,  MovementStrength_Y;
    
    private float maxVelocity_X;
    private float lowMaxVelocity_X = .75f, highMaxVelocity_X = 1.5f; 
    private float negMaxVelocity_Y = -1f, posMaxVelocity_Y = .85f;
    private float raycast_yOffset = .1f, raycast_xOffset = 0f;

    public override void CharacterStart()
    {
        maxVelocity_X = lowMaxVelocity_X;
        MovementStrength_X = minStrength_X;
        MovementStrength_Y = 0f;
    }
    public override void CharacterUpdate()
    {
        left = Input.GetKey(KeyCode.A) ? 1:0 ;
        right = Input.GetKey(KeyCode.D) ? 1:0 ;
        movingVertical = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) ? 1:0 ;
        movingVertical = Input.GetKey(KeyCode.S) ? -1:movingVertical;
        //Concise conditionals, finally found a good use for it to replace implicit boolean->int conversion from gml
        movingHorizontal =  right - left;
        //remember, right is increasing
    }
    private float currProg_X = 0f, currProg_Y = 0f;
    private float currProgSlowStart_X = .35f;
    private static float progMultiplier_XFactor = 3f, progMultiplier_XBase = MathF.E, maxVelocity_X_Factor = 3/4f;
    [SerializeField] private bool rayCast_groundHit = false;
    public override void CharacterFixedUpdate()
    {   
        if(movingHorizontal != 0 && currProg_X < maxProg_X && prev_movingHorizontal == movingHorizontal)
        {   
            float _currProg_add_X = 1f;
            if(currProg_X/maxProg_X  >= currProgSlowStart_X)
            {
                _currProg_add_X = MathF.Pow(progMultiplier_XBase, -(currProg_X/maxProg_X) * progMultiplier_XFactor);
            }
            //Debug.Log(_currProg_add_X + " " + maxProg_X);

            currProg_X += _currProg_add_X;

            //prevent currProg from going above max
            if(currProg_X > maxProg_X)
            {
                currProg_X = maxProg_X;
            }
        }
        else if(movingHorizontal == 0 || prev_movingHorizontal != movingHorizontal)
        {
            currProg_X = 0f; 
        }
        prev_movingHorizontal = movingHorizontal;

        MovementStrength_X = Mathf.Lerp(minStrength_X, maxStrength_X, currProg_X/maxProg_X);
        
        

        accelComp.applyAcceleration_X(movingHorizontal, MovementStrength_X, mass, ref velocity, rayCast_groundHit);

        
        //Logic for determing if you can still jump
        if(rayCast_groundHit)
        {
            prev_movingVertical = movingVertical;
            currProg_Y = maxProg_PosY;
        }
        else
        {
            if(movingVertical != 1)
            {
                prev_movingVertical = movingVertical;
            }
        }

        //Determining strength of jump/fall based on how long you held it + determining max velocity
        float _currMaxProg = 0f;
        if(movingVertical == 1 && prev_movingVertical == 1)
        {
            MovementStrength_Y = Mathf.Lerp(minStrength_PosY, maxStrength_PosY, currProg_Y/maxProg_PosY);
            currProg_Y -= 1f;

            if(currProg_Y/maxProg_PosY <= 0)
            {
                prev_movingVertical = 0;
            }

            _currMaxProg = maxProg_PosY;
        }
        else if (!rayCast_groundHit && movingVertical == -1)
        {
            if(prev_movingVertical != -1)
            {
                currProg_Y = 0f;
            }

            currProg_Y += 1f;
            MovementStrength_Y = Mathf.Lerp(minStrength_NegY, maxStrength_NegY, currProg_Y/maxProg_NegY);
            _currMaxProg = maxProg_NegY;
        }
        else
        {
            currProg_Y = 0f;
            MovementStrength_Y = 0; 
            if(_currMaxProg >= lowMaxVelocity_X)
            {
                _currMaxProg -= Time.fixedDeltaTime * maxVelocity_X_Factor;
            }
        }

        //Determined by speed of your curr Y
        maxVelocity_X= Mathf.Lerp(lowMaxVelocity_X, highMaxVelocity_X, currProg_Y);
        accelComp.applyAcceleration_Y(movingVertical, MovementStrength_Y, rayCast_groundHit, mass, ref velocity);

        //Clamp velocities by max velocity
        velocity.x = Math.Clamp(velocity.x, -maxVelocity_X, maxVelocity_X);
        velocity.y = Mathf.Clamp(velocity.y, negMaxVelocity_Y, posMaxVelocity_Y);

        RaycastHit2D downRaycast = raycasterComp.downRaycast(raycast_xOffset, raycast_yOffset, ref velocity);
        if(downRaycast.collider != null)
        {
            Debug.Log("Found something");
            rayCast_groundHit = true;

            IDScript hitIDScript = downRaycast.collider.gameObject.GetComponent<IDScript>();
            if(hitIDScript.objectType == GLOBAL_CONSTANTS.objectType.isFloor)
            {
                float distance = currentCollider.transform.position.y - downRaycast.point.y;
                velocity.y = distance;
            }
            else if(hitIDScript.objectType == GLOBAL_CONSTANTS.objectType.isEntity)
            {

            }
        }
        else
        {
            rayCast_groundHit = false;
        }
    }

    void OnDrawGizmos()
    {
        UnityEngine.Vector3 cubeDebugSize = new UnityEngine.Vector3(0.1f,0.1f,0.1f);
        UnityEngine.Vector3 offsetSSS = new UnityEngine.Vector3(currentCollider.offset.x, 0f, 0f);
        Gizmos.DrawCube(currentCollider.transform.position + offsetSSS, cubeDebugSize);
    }
}
