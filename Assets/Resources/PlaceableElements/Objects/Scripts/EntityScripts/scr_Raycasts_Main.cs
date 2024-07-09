using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class scr_Raycasts_Main : MonoBehaviour
{   
    public BoxCollider2D currCharCollider;
    public float gameObjectScale;
    public UnityEngine.Vector2 colliderOffset;
    public UnityEngine.Vector2 colliderSize;

    public UnityEngine.Vector2 vert_boxCastSize;
    public UnityEngine.Vector2 horiz_boxCastSize;

    public bool bypassFloorCollide = false;
    public bool depthBasedCollision = false; 
    //Since we use ID's, we will default to main layer for everything that is "in-game". 
    //Could use binary if necessary for robustness but for now, its alright. 
    public int targetLayer = GLOBAL_CONSTANTS.Layers.main;
    public float boxSize_regFactor, boxSize_arbFactor;
    void Start()
    {
        //we get x since every scale will ALWAYS be the same in x & y to preserve original sprite resolution.
        gameObjectScale = currCharCollider.gameObject.transform.localScale.x;
        
        colliderOffset = currCharCollider.offset * gameObjectScale;
        colliderSize = currCharCollider.size * gameObjectScale;
        //y is arbitrary since all that matters is seeing the bottom
        vert_boxCastSize = new UnityEngine.Vector2(colliderSize.x * boxSize_regFactor, colliderSize.y * boxSize_arbFactor);
        //And vice versa for X
        horiz_boxCastSize = new UnityEngine.Vector2(colliderSize.x * boxSize_arbFactor, colliderSize.y * boxSize_regFactor);
    }

    //To debug a raycast by showing its position with a gizmo, remove the UnityEngine.Vector3 from currentColliderPosition and positionVector
    //Youll also have to edit the gizmo function with its debug offset (which one it affects and how)
    public RaycastHit2D downRaycast(float _xOffset, float _yOffset, ref UnityEngine.Vector3 entityVelocity)
    {

        UnityEngine.Vector3 currentColliderPosition = 
        new UnityEngine.Vector3(currCharCollider.transform.position.x + colliderOffset.x, 
                                currCharCollider.transform.position.y + colliderOffset.y, 
                                currCharCollider.transform.position.z);

        //We change this position vector EACH TIME we switch types of casting
        UnityEngine.Vector2 positionVector = new UnityEngine.Vector2(currentColliderPosition.x + _xOffset, currentColliderPosition.y + _yOffset - colliderSize.y/2);
        
        RaycastHit2D downHit = Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            vert_boxCastSize, //Precreated size (remember origin of this is in the center of the bottom of our sprite (due to pivot points) so we adjust it in the origin position by adding half our targetted y size)
            0f, //No rotation
            UnityEngine.Vector2.up, //We use Up instead of down to avoid negating anything
            Math.Clamp(entityVelocity.y - _yOffset, float.NegativeInfinity, 0f), //Distance we send the boxcast, using the velocity (where we'll end up) & the offset we use (make up for starting higher/lowerr)
            targetLayer //Layer we are trying to get casts from
        );
        
        /* 
        // --- TESTING VARS -----
        debugVelocity = entityVelocity;
        debug_Offset = _yOffset;
        debugHitCast = downHit;
        */
        return downHit;
    }
    public RaycastHit2D upRaycast(float _xOffset, float _yOffset, ref UnityEngine.Vector3 entityVelocity)
    {
        UnityEngine.Vector3 currentColliderPosition = 
        new UnityEngine.Vector3(currCharCollider.transform.position.x + colliderOffset.x, 
                                currCharCollider.transform.position.y + colliderOffset.y, 
                                currCharCollider.transform.position.z);

        //We change this position vector EACH TIME we switch types of casting
        UnityEngine.Vector2 positionVector = new UnityEngine.Vector2(currentColliderPosition.x + _xOffset, currentColliderPosition.y - _yOffset + colliderSize.y/2);
        
        RaycastHit2D upHit = Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            vert_boxCastSize, //Precreated size (remember origin of this is in the center of the bottom of our sprite (due to pivot points) so we adjust it in the origin position by adding half our targetted y size)
            0f, //No rotation
            UnityEngine.Vector2.up, //We use Up instead of down to avoid negating anything
            Math.Clamp(entityVelocity.y + _yOffset, 0f, Mathf.Infinity), //Distance we send the boxcast, using the velocity (where we'll end up) & the offset we use (make up for starting higher/lowerr)
            targetLayer //Layer we are trying to get casts from
        );
        /* 
        // --- TESTING VARS -----
        debugVelocity = entityVelocity;
        debug_Offset = _yOffset;
        debugHitCast = upHit;
        */

        return upHit;
    }
    public RaycastHit2D rightRaycast(float _xOffset, float _yOffset, ref UnityEngine.Vector3 entityVelocity)
    {
        currentColliderPosition = 
        new UnityEngine.Vector3(currCharCollider.transform.position.x + colliderOffset.x, 
                                currCharCollider.transform.position.y + colliderOffset.y, 
                                currCharCollider.transform.position.z);

        //We change this position vector EACH TIME we switch types of casting
        positionVector = new UnityEngine.Vector2(currentColliderPosition.x - _xOffset + colliderSize.x/2, currentColliderPosition.y + _yOffset);
        
        RaycastHit2D rightHit = Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            horiz_boxCastSize, //Precreated size based on the entire y size of our entity and a small sliver of x which is arbitrary
            0f, //No rotation
            UnityEngine.Vector2.right, //We use Right as if we want to negate something, it would go in the wrong direction
            Math.Clamp(entityVelocity.x + _xOffset, 0f, Mathf.Infinity), //We add xOffset since itll always be slightly behind where we send out the check, and add velocity because we always want to check the direction we are going, clamp negative side, we'll never check on the inside like that
            targetLayer //Layer we are trying to get casts from
        );

        if(rightHit.collider != null)
        {
            Debug.Log("RightHit");
        }

        // --- TESTING VARS -----
        debugVelocity = entityVelocity;
        debug_Offset = _xOffset;
        debugHitCast = rightHit;
        
        return rightHit;
    }
    public RaycastHit2D leftRaycast(float _xOffset, float _yOffset, ref UnityEngine.Vector3 entityVelocity)
    {
        UnityEngine.Vector3 currentColliderPosition = 
        new UnityEngine.Vector3(currCharCollider.transform.position.x + colliderOffset.x, 
                                currCharCollider.transform.position.y + colliderOffset.y, 
                                currCharCollider.transform.position.z);

        //We change this position vector EACH TIME we switch types of casting                                                                                                                                                              
        UnityEngine.Vector2 positionVector = new UnityEngine.Vector2(currentColliderPosition.x + _xOffset - colliderSize.x/2, currentColliderPosition.y + _yOffset);
        
        RaycastHit2D leftHit = Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            horiz_boxCastSize, //Precreated size based on the entire y size of our entity and a small sliver of x which is arbitrary
            0f, //No rotation
            UnityEngine.Vector2.right, //We use Right as if we want to negate something, it would go in the wrong direction
            Math.Clamp(entityVelocity.x - _xOffset, Mathf.NegativeInfinity, 0f), //We subtract xOffset as itll always make up for it going to the left, clamp off the positive side since we'll never need to check within
            targetLayer //Layer we are trying to get casts from
        );
    
        /* 
        // --- TESTING VARS -----
        debugVelocity = entityVelocity;
        debug_Offset = _xOffset;
        debugHitCast = leftHit;
        */
        
        return leftHit;
    }

    //Universal script for landing on something, since all of the floors and entities can be reduced to a generalized system
    public void touchObject(RaycastHit2D _hitTarget, GameObject _hitObject, IDScript _hitTarget_ID, float _currOffset, ref UnityEngine.Vector3 entityVelocity, GameObject _caller, int _hitType, bool _ignore)
    {
        
        if(!_ignore)
        {
            float velocity_Clamp = 0;
            UnityEngine.Vector3 _currColliderPosition = currCharCollider.transform.position + new UnityEngine.Vector3(colliderOffset.x, colliderOffset.y, 0);
            switch(_hitType)
            {
                case GLOBAL_CONSTANTS.Direction.down:
                    //Get distance between original collider position and hitpoint, then add the offset
                    velocity_Clamp =  (_hitTarget.collider.transform.position.y + _hitTarget.collider.transform.localScale.y) - (_currColliderPosition.y - colliderSize.y/2);
                    velocity_Clamp = Math.Clamp(velocity_Clamp, Mathf.NegativeInfinity, 0f);
                    entityVelocity.y = velocity_Clamp;
                    
                    //Debug.Log(currCharCollider.transform.position.y + " " + _hitTarget.point.y + " " + velocity_Clamp + " " + _hitTarget.collider.transform.localScale.y);
                break;
                
                case GLOBAL_CONSTANTS.Direction.up:
                    //Get distance between original collider position and hitpoint, then add the offset
                    velocity_Clamp =  (_hitTarget.collider.transform.position.y) - (_currColliderPosition.y + colliderSize.y/2); 
                    velocity_Clamp = Math.Clamp(velocity_Clamp, 0f, Mathf.Infinity);
                    entityVelocity.y = velocity_Clamp;
                    
                    //Debug.Log(currCharCollider.transform.position.y + " " + _hitTarget.point.y + " " + velocity_Clamp + " " + _hitTarget.collider.transform.localScale.y);
                break;
                
                case GLOBAL_CONSTANTS.Direction.right:
                    //We divide the local scale by two since the orig bin is in the middle
                    velocity_Clamp = (_hitTarget.collider.transform.position.x - (_hitTarget.collider.transform.localScale.x / 2)) - (_currColliderPosition.x + colliderSize.x/2);
                    velocity_Clamp = Math.Clamp(velocity_Clamp, 0f, Mathf.Infinity);
                    entityVelocity.x = velocity_Clamp;
                    Debug.Log("ERMMMMM RIGHT???");

                break;

                case GLOBAL_CONSTANTS.Direction.left:
                    //We divide the local scale by two since the orig bin is in the middle
                    velocity_Clamp = (_hitTarget.collider.transform.position.x + (_hitTarget.collider.transform.localScale.x / 2)) - (_currColliderPosition.x - colliderSize.x/2);
                    velocity_Clamp = Math.Clamp(velocity_Clamp, Mathf.NegativeInfinity, 0f);
                    entityVelocity.x = velocity_Clamp;
                break;

                default:
                    throw new ArgumentOutOfRangeException("Int used is out of range somehow, make sure the calling object " + gameObject.name + "'s main script calls the raycast.touchObject with a valid hit type");
            }
        }
        if(_hitTarget_ID.hasEffectScript)
        {
            scr_Effect_Script currEffect = _hitObject.GetComponent<scr_Effect_Script>();
            currEffect.HitTarget(_caller, _hitTarget, _hitType);
        }
    }
    
    //These are our testinvariables
    UnityEngine.Vector3 currentColliderPosition;
    UnityEngine.Vector2 positionVector;
    UnityEngine.Vector2 debugVelocity;
    RaycastHit2D debugHitCast;
    float debug_Offset;
    private void OnDrawGizmos() {
        UnityEngine.Vector3 cubeDebugSize = new UnityEngine.Vector3(0.1f,0.1f,0.1f);
        Gizmos.DrawCube(currentColliderPosition, cubeDebugSize);

        cubeDebugSize = new UnityEngine.Vector3(0.15f,0.15f,0.15f);
        Gizmos.DrawCube(debugHitCast.point, cubeDebugSize);

        cubeDebugSize = new UnityEngine.Vector3(0.05f,0.05f,0.05f);
        UnityEngine.Vector3 edit = new UnityEngine.Vector3(positionVector.x + debugVelocity.x + debug_Offset, positionVector.y, 0f);
        
        Gizmos.DrawCube(edit, cubeDebugSize);
    }
    
}
