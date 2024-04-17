using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class scr_Raycasts_Main : MonoBehaviour
{   
    public scr_Basic_Entity characterScript;
    public UnityEngine.Vector2 down_boxCastSize;
    public UnityEngine.Vector3 currentColliderPosition;
    public bool depthBasedCollision = false; 
    public float colliderXScale = 0f;
    public float colliderYScale = 0f;
    public int groundLayer = 1 << 7; //7 is the main layer where tiles, chars, and touchable objects exist

    public float footOffset = 0f;

    public ContactFilter2D castFilter; 
    public bool bypassFloorCollide = false;

    // Start is called before the first frame update
    void Awake()
    {
        //Sets up filters for the collision
        castFilter.useLayerMask = true;
        castFilter.useDepth = depthBasedCollision;
        castFilter.useOutsideDepth = false;
        castFilter.useNormalAngle = false;
        castFilter.useOutsideNormalAngle = false;
        castFilter.layerMask = groundLayer;

    }

    // Update is called once per frame
    public void RaycastFixedUpdate()
    {
        //Checking floor ones first, remember, the vector direction and distance only make the created box cast travel that whole direction, allowing it to check further away
        currentColliderPosition = characterScript.currentCollider.transform.position;

        //We change this position vector EACH TIME we switch types of casting
        UnityEngine.Vector3 positionVector = new UnityEngine.Vector3(currentColliderPosition.x, currentColliderPosition.y + footOffset, currentColliderPosition.z);
        
        RaycastHit2D DownHit = 
        Physics2D.BoxCast(
            positionVector, //The origin of our collider + a small offset of half our y size to get it right above the feet
            down_boxCastSize, //Precreated size (remember origin of this is in the center of the bottom of our sprite (due to pivot points) so we adjust it in the origin position by adding half our targetted y size)
            0f, //No rotation
            UnityEngine.Vector2.up, //Down is the direction we want it to go to check for the floor
            characterScript.velocity.y - footOffset, //Negated since going up means positively going down and vice versa, so to fix it, we negate
            groundLayer //Target only the ground layers
            );


        UnityEngine.Vector2 DEBUG_P1 = new UnityEngine.Vector2(characterScript.currentCollider.transform.position.x - colliderXScale/2f, characterScript.currentCollider.transform.position.y + down_boxCastSize.y);
        UnityEngine.Vector2 DEBUG_P2 = new UnityEngine.Vector2(characterScript.currentCollider.transform.position.x + colliderXScale/2f, characterScript.currentCollider.transform.position.y + down_boxCastSize.y);
        UnityEngine.Vector2 DEBUG_P3 = new UnityEngine.Vector2(characterScript.currentCollider.transform.position.x - colliderXScale/2f, characterScript.currentCollider.transform.position.y);
        UnityEngine.Vector2 DEBUG_P4 = new UnityEngine.Vector2(characterScript.currentCollider.transform.position.x + colliderXScale/2f, characterScript.currentCollider.transform.position.y);

        
        Debug.DrawLine(DEBUG_P3, DEBUG_P4, Color.red);
        Debug.DrawLine(DEBUG_P1, DEBUG_P3, Color.red);
        Debug.DrawLine(DEBUG_P1, DEBUG_P2, Color.red);
        Debug.DrawLine(DEBUG_P2, DEBUG_P4, Color.red);


        if(DownHit.collider != null && !bypassFloorCollide)
        {
            if(DownHit.collider.gameObject.GetComponent<IDScript>().objectType == GLOBAL_CONSTANTS.objectType.isFloor)
            {
                Debug.Log("Hit");
                Debug.DrawLine(characterScript.currentCollider.transform.position, DownHit.normal, Color.green);
                HitFloor(DownHit);
            }
        }
        else
        {
            //Check if theyre just jumping or if theyre actually falling because nothing is currently below them
            if(characterScript.characterState != GLOBAL_CONSTANTS.CharacterStates.activeAir)
            {
                characterScript.characterState = GLOBAL_CONSTANTS.CharacterStates.Falling;
            }
        }
    }
    void HitFloor(RaycastHit2D _target)
    {
        if(_target.normal.y < 0)
        {
            characterScript.velocity.y = -(currentColliderPosition.y - _target.point.y) ;
            //Sets up characterState back to running, and totalJumps back to 0 if necessary

        }
    }
}
