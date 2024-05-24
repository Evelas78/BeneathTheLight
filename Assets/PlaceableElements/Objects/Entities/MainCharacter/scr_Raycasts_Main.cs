using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class scr_Raycasts_Main : MonoBehaviour
{   
    public BoxCollider2D currCharCollider;
    public UnityEngine.Vector2 colliderOffset;
    public UnityEngine.Vector2 colliderSize;

    public ContactFilter2D castFilter; 
    public UnityEngine.Vector2 down_boxCastSize;
    public bool bypassFloorCollide = false;
    public bool depthBasedCollision = false; 
    //Since we use ID's, we will default to main layer for everything that is "in-game". 
    //Could use binary if necessary for robustness but for now, its alright. 
    public int targetLayer = 1 << GLOBAL_CONSTANTS.layers.main;
    public float boxSize_X_factor, boxSize_Y_factor;
    void Awake()
    {
        colliderOffset = currCharCollider.offset;
        colliderSize = currCharCollider.size;
        down_boxCastSize = new UnityEngine.Vector2(colliderSize.x * boxSize_X_factor, colliderSize.y * boxSize_Y_factor);

        //Sets up filters for the collision
        castFilter.useLayerMask = true;
        castFilter.layerMask = targetLayer;

        castFilter.useDepth = depthBasedCollision;
        castFilter.useOutsideDepth = false;
    
        castFilter.useNormalAngle = false;
        castFilter.useOutsideNormalAngle = false;
        castFilter.useTriggers = true;
    }

    // Update is called once per frame
    public void downRaycast(float _xOffset, float _yOffset, ref UnityEngine.Vector3 entityVelocity)
    {
        UnityEngine.Vector3 currentColliderPosition = 
        new UnityEngine.Vector3(currCharCollider.transform.position.x + colliderOffset.x, 
                                currCharCollider.transform.position.y + colliderOffset.y, 
                                currCharCollider.transform.position.z);

        //We change this position vector EACH TIME we switch types of casting
        UnityEngine.Vector2 positionVector = new UnityEngine.Vector2(currentColliderPosition.x +_xOffset, currentColliderPosition.y + _yOffset - colliderOffset.y);
    
        List<RaycastHit2D> DownHit = new List<RaycastHit2D>();

        Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            down_boxCastSize, //Precreated size (remember origin of this is in the center of the bottom of our sprite (due to pivot points) so we adjust it in the origin position by adding half our targetted y size)
            0f, //No rotation
            UnityEngine.Vector2.up, //Down is the direction we want it to go to check for the floor
            castFilter, //Negated since going up means positively going down and vice versa, so to fix it, we negate
            DownHit, //Target only the ground layers
            entityVelocity.y + _yOffset
        );

        RaycastHit2D _closestHit;
        _closestHit = DownHit.ElementAt<RaycastHit2D>(0);
        foreach(RaycastHit2D _hit_obj in DownHit)
        {
            if(_closestHit.distance > _hit_obj.distance)
            {
                _closestHit = _hit_obj;
            }
            else if(_closestHit.distance == _hit_obj.distance)
            {
                IDScript _currCloseHitIdScript = _closestHit.rigidbody.gameObject.GetComponent<IDScript>();
                IDScript _contenderCloseHitIDScript = _hit_obj.rigidbody.gameObject.GetComponent<IDScript>();
                if(_currCloseHitIdScript.objectType == GLOBAL_CONSTANTS.entityType.isEnemy && _contenderCloseHitIDScript.objectType != GLOBAL_CONSTANTS.entityType.isEnemy)
                {
                    //we dont change since the current closest one is an enemy
                }
                else if (_currCloseHitIdScript.objectType != GLOBAL_CONSTANTS.entityType.isEnemy && _contenderCloseHitIDScript.objectType == GLOBAL_CONSTANTS.entityType.isEnemy)
                {
                    //We change, since enemies that cant be touched wouldnt have triggered this anyway
                    _closestHit = _hit_obj;
                }
                else
                {
                    //if depth is closer then we land on that one
                    if(_closestHit.rigidbody.gameObject.transform.position.z > _hit_obj.rigidbody.gameObject.transform.position.z)
                    {
                        _closestHit = _hit_obj;
                    }

                }
            }
        }

        //Closest Object is now determined and now we act upon that object
        
    }
}