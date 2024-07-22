using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Goober_HitEffect : scr_Effect_Script
{   
    scr_BaseEntity_Main self_entityScr;
    GameObject mainChar;    

    private void Awake() 
    {
        self_entityScr = gameObject.GetComponent<scr_BaseEntity_Main>();   
        mainChar = self_entityScr.gameControllerScript.currMainChar;
    }
    public override void HitTarget(GameObject _hitBy, RaycastHit2D _currRaycastInfo, int _hitDirection)
    {
        if(_hitBy == mainChar)
        {
            switch(_hitDirection)
            {
                case GLOBAL_CONSTANTS.Direction.down:
                    self_entityScr.triggerDeath();
                break;

                case GLOBAL_CONSTANTS.Direction.up:
                case GLOBAL_CONSTANTS.Direction.left:
                case GLOBAL_CONSTANTS.Direction.right:
                    Vector2 bounceDirection;             
                break;
            }
        }
        else //Will be hit by hat when hat object is made
        {

        }
    }
}
