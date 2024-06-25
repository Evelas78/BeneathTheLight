using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class scr_mainBody_Main : scr_Basic_Entity
{
    [SerializeField] private scr_Raycasts_Main raycasterComp = null;
    [SerializeField] private scr_Acceleration_Component accelComp = null;

    [SerializeField] private float maxProg_X, maxProg_PosY, maxProg_NegY; //Realistically, itll never touch this as the multiplier will decrease the rate of growth
    public override void CharacterAwake()
    {
        animationComp = gameObject.AddComponent<scr_animController>();
        scr_animations currIdleAnim = gameObject.AddComponent<scr_Anim_Idle_Hat>();
        animationComp.spriteLoad(currIdleAnim);
        airState = GLOBAL_CONSTANTS.AirStates.Grounded;

        maxProg_X =  (4f / Time.fixedDeltaTime);
        maxProg_PosY = (.35f / Time.fixedDeltaTime);
        maxProg_NegY = (.75f / Time.fixedDeltaTime);

        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isPlayer;

        raycasterComp = gameObject.AddComponent<scr_Raycasts_Main>();
        raycasterComp.currCharCollider = currentCollider;
        raycasterComp.depthBasedCollision = true;

        raycasterComp.boxSize_regFactor = 1f;
        raycasterComp.boxSize_arbFactor = 1/16f;


        //Origin of the boxCast will be relative to the position we give. So starting it at the Y (which is the bottom middle of the object)
        accelComp = gameObject.AddComponent<scr_Acceleration_Component>();  
        accelComp.gFrictionApplies = true;
        accelComp.gravApplies = true;
    }
    [SerializeField] private int left,right;
    [SerializeField] private int movingHorizontal = 0,movingVertical = 0;
    [SerializeField] private int prev_movingHorizontal = 0 ,prev_movingVertical = 0;

    private float maxStrength_X = .85f, minStrength_X = .25f;
    private float maxStrength_PosY = .55f, minStrength_PosY = .25f;    
    private float maxStrength_NegY = .35f, minStrength_NegY = .15f;   
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
    [SerializeField]private float currProg_X = 0f, currProg_Y = 0f;
    private float currProgSlowStart_X = .35f;
    private static float progMultiplier_XFactor = 3, progMultiplier_XBase = MathF.E, maxVelocity_X_Factor = 9f;
    private static float maxProg_PosY_conserveFactor = .65f, maxProg_PosY_threshold = .75f;
    private static float storedMaxYProg;
    [SerializeField] private bool rayCast_groundHit = false;
    public override void CharacterFixedUpdate()
    {   
        handleAirState();
        //Applies acceleration based on how long you are holding the left and right
        XFunc();

        //Applies acceleration for the Y based on how much youre holding it
        YFunc();
        //Check Y Func, but basically, your max prog stored + curr Prog changes how fast your max X Velocity is
        maxVelocity_X= Mathf.Lerp(lowMaxVelocity_X, highMaxVelocity_X, currProg_Y/storedMaxYProg);

        //Clamp velocities by max velocity
        velocity.x = Math.Clamp(velocity.x, -maxVelocity_X, maxVelocity_X);
        velocity.y = Mathf.Clamp(velocity.y, negMaxVelocity_Y, posMaxVelocity_Y);
        
        raycastDownFunction();
        determineActionState();
    }
    public void XFunc()
    { 
        //If changing direction midair, slowdown the acceleration change
        float airChange_XFactor = 1f;  
        if(airState != GLOBAL_CONSTANTS.AirStates.Grounded)
        {
            if(prev_movingHorizontal != movingHorizontal)
                airChange_XFactor = .25f;
        }

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
        MovementStrength_X *= airChange_XFactor;

        accelComp.applyAcceleration_X(movingHorizontal, MovementStrength_X, mass, ref velocity, rayCast_groundHit);
    }
    public void YFunc()
    {
        if(airState == GLOBAL_CONSTANTS.AirStates.activeAir)
        {
            movingVertical = 1;
            float movementStrength_Y_Mult = Mathf.Lerp(minStrength_PosY, maxStrength_PosY, currProg_Y/maxProg_PosY);
            float sin_factor = Mathf.PI/2 * (currProg_Y/maxProg_PosY);
            MovementStrength_Y = Mathf.Sin(sin_factor) * movementStrength_Y_Mult;
            
            currProg_Y -= 1f;
            currProg_Y = Math.Clamp(currProg_Y, 0f, maxProg_PosY);
            
            if(currProg_Y == 0)
            {
                airState = GLOBAL_CONSTANTS.AirStates.Falling;
                movingVertical = 0;
            }
        }
        else if (airState == GLOBAL_CONSTANTS.AirStates.Falling && movingVertical == -1)
        {
            //Store some of the strength from the jump to reward quick changes
            if(prev_movingVertical != movingVertical)
            {
                prev_movingVertical = movingVertical;
                if(prev_movingVertical == 1 && currProg_Y >= maxProg_PosY * maxProg_PosY_conserveFactor)
                {
                    currProg_Y = maxProg_PosY * maxProg_PosY_conserveFactor;
                    Math.Round(currProg_Y);
                }
            }

            currProg_Y += 1f;
            currProg_Y = Math.Clamp(currProg_Y, 0f, maxProg_NegY);
            MovementStrength_Y = Mathf.Lerp(minStrength_NegY, maxStrength_NegY, currProg_Y/maxProg_NegY);
        }
        else
        {  
            if(prev_movingVertical != movingVertical)
            {
                prev_movingVertical = movingVertical;
                //Dont save any progress, allowing to glide better
                if(prev_movingVertical == 1)
                {
                    currProg_Y = 0f;
                }
            }

            currProg_Y -= maxVelocity_X_Factor;
            currProg_Y = Math.Clamp(currProg_Y, 0f, maxProg_NegY);
            MovementStrength_Y = Mathf.Lerp(0f, maxStrength_NegY, currProg_Y/maxProg_NegY);
        }
        
        accelComp.applyAcceleration_Y(movingVertical, MovementStrength_Y, rayCast_groundHit, mass, ref velocity);
    }
    public void raycastDownFunction()
    {
        RaycastHit2D downRaycast = raycasterComp.downRaycast(raycast_xOffset, raycast_yOffset, ref velocity);
        if(downRaycast.collider != null)
        {
            Debug.Log("Found something " + downRaycast.collider.gameObject.name);

            GameObject hitObject = downRaycast.collider.gameObject;
            IDScript hitIDScript = hitObject.GetComponent<IDScript>();

            int targetType = hitIDScript.objectType;
            switch(targetType)
            {
                case GLOBAL_CONSTANTS.objectType.isEnemy:
                    break;
                case GLOBAL_CONSTANTS.objectType.isPlayer:
                    break;
                case GLOBAL_CONSTANTS.objectType.isObject:
                    break;
                case GLOBAL_CONSTANTS.objectType.isPlatform:
                case GLOBAL_CONSTANTS.objectType.isSolid:
                    if(airState != GLOBAL_CONSTANTS.AirStates.activeAir)
                    {
                        raycasterComp.touchObject(downRaycast, hitObject, hitIDScript, raycast_yOffset, ref velocity, gameObject, GLOBAL_CONSTANTS.Direction.down);
                        rayCast_groundHit = true;
                    }       
                    else
                        rayCast_groundHit = false;
                    break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
        else
        {
            rayCast_groundHit = false;
        }
    }
    public void raycastRightFunction()
    {
        
    }
    public void handleAirState()
    {
        if(rayCast_groundHit)
        {
            airState = GLOBAL_CONSTANTS.AirStates.Grounded;
        }
    
        //Logic for determining state whether grounded or in air/jumping
        //if youre on the ground but want to jump, then the system will prime itself to put you at max jump strength, in the right aierrstate, and set the maxStoredY
        if(airState == GLOBAL_CONSTANTS.AirStates.Grounded && movingVertical == 1)
        {
            currProg_Y = maxProg_PosY;
            airState = GLOBAL_CONSTANTS.AirStates.activeAir;
            storedMaxYProg = maxProg_PosY; 
                       
            prev_movingVertical = movingVertical;
        }
        //If actively jumping, youre no longer holding the jump button AND the currProgress on the jump has fallen less than the thresh hold we want, then set us to fall
        //The reason we have a threshold is to ensure a minimum jump to prevent a player from just staying on the ground and looking kinda goofy
        else if(airState == GLOBAL_CONSTANTS.AirStates.activeAir && movingVertical != 1 && currProg_Y < maxProg_PosY_threshold * maxProg_PosY)
        {
            currProg_Y = 0;
            airState = GLOBAL_CONSTANTS.AirStates.Falling;
            storedMaxYProg = maxProg_NegY;
        }
    }
    public void determineActionState()
    {
        if(airState == GLOBAL_CONSTANTS.AirStates.Grounded)
        {
            if(movingHorizontal != 0)
            {
                characterState = GLOBAL_CONSTANTS.CharacterStates.Walking;
            }
            else
            {
                characterState = GLOBAL_CONSTANTS.CharacterStates.Idle;
            }
        }
        else if(airState == GLOBAL_CONSTANTS.AirStates.Falling || airState == GLOBAL_CONSTANTS.AirStates.activeAir)
        {
            characterState = GLOBAL_CONSTANTS.CharacterStates.Jumping;
        }
    }

    public override void DeathUpdate()
    {
        throw new NotImplementedException();
    }

}
