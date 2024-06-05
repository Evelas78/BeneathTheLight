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

    public UnityEngine.Vector2 down_boxCastSize;
    public bool bypassFloorCollide = false;
    public bool depthBasedCollision = false; 
    //Since we use ID's, we will default to main layer for everything that is "in-game". 
    //Could use binary if necessary for robustness but for now, its alright. 
    public int targetLayer = GLOBAL_CONSTANTS.layers.main;
    public float boxSize_X_factor, boxSize_Y_factor;
    void Start()
    {
        //we get x since every scale will ALWAYS be the same in x & y to preserve original sprite resolution.
        gameObjectScale = currCharCollider.gameObject.transform.localScale.x;
        
        colliderOffset = currCharCollider.offset * gameObjectScale;
        colliderSize = currCharCollider.size * gameObjectScale;
        down_boxCastSize = new UnityEngine.Vector2(colliderSize.x * boxSize_X_factor, colliderSize.y * boxSize_Y_factor);
    }

    // Update is called once per frame
    UnityEngine.Vector3 currentColliderPosition;
    UnityEngine.Vector3 positionVector;
    UnityEngine.Vector3 debugVelocity;
    float debug_yOffset;
    public RaycastHit2D downRaycast(float _xOffset, float _yOffset, ref UnityEngine.Vector3 entityVelocity)
    {
        debugVelocity = entityVelocity;
        debug_yOffset = _yOffset;
        currentColliderPosition = 
        new UnityEngine.Vector3(currCharCollider.transform.position.x + colliderOffset.x, 
                                currCharCollider.transform.position.y + colliderOffset.y, 
                                currCharCollider.transform.position.z);

        //We change this position vector EACH TIME we switch types of casting
        positionVector = new UnityEngine.Vector2(currentColliderPosition.x +_xOffset, currentColliderPosition.y + _yOffset - colliderOffset.y);
        
        RaycastHit2D DownHit = Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            down_boxCastSize, //Precreated size (remember origin of this is in the center of the bottom of our sprite (due to pivot points) so we adjust it in the origin position by adding half our targetted y size)
            0f, //No rotation
            UnityEngine.Vector2.up, //We use Up instead of down to avoid negating anything
            Math.Clamp(entityVelocity.y - _yOffset, float.NegativeInfinity, 0f), //Distance we send the boxcast, using the velocity (where we'll end up) & the offset we use (make up for starting higher/lowerr)
            targetLayer //Layer we are trying to get casts from
        );
        Debug.Log(DownHit.collider);
        
        return DownHit;
    }
    public void landOnEnemy(RaycastHit2D _hitTarget, IDScript _hitTarget_ID, ref int _action)
    {


        
    }

    public void landOnGround(RaycastHit2D hitTarget, IDScript _hitTarget_ID, float yOffset, ref int Health, ref UnityEngine.Vector3 entityVelocity)
    {
        float velocity_Y_Clamp = currCharCollider.transform.position.y - hitTarget.point.y + yOffset;
        entityVelocity.y = -velocity_Y_Clamp;
    
        if(_hitTarget_ID.hasEffectScript)
        {
            //Create an abstract script with a function called "affectFunc"
        }
    }
    private void OnDrawGizmos() {
        UnityEngine.Vector3 cubeDebugSize = new UnityEngine.Vector3(0.1f,0.1f,0.1f);
        Gizmos.DrawCube(currentColliderPosition, cubeDebugSize);

        cubeDebugSize = new UnityEngine.Vector3(0.075f,0.075f,0.075f);
        Gizmos.DrawCube(positionVector, cubeDebugSize);

        cubeDebugSize = new UnityEngine.Vector3(0.05f,0.05f,0.05f);
        UnityEngine.Vector3 edit = new UnityEngine.Vector3(positionVector.x + debugVelocity.x, positionVector.y + debugVelocity.y + debug_yOffset, 0f);
        
        Gizmos.DrawCube(edit, cubeDebugSize);
    }
}
