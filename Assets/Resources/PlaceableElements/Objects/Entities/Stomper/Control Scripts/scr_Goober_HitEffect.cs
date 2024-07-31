using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Goober_HitEffect : scr_Effect_Script
{   
    scr_BaseEntity_Main self_entityScr;
    private void Awake() 
    {
        self_entityScr = gameObject.GetComponent<scr_BaseEntity_Main>();   
    }
    public override void HitTarget(GameObject _hitBy, RaycastHit2D _currRaycastInfo, int _hitDirection)
    {
        scr_BaseEntity_Main currObjectScript = _hitBy.GetComponent<scr_BaseEntity_Main>();
        if(currObjectScript.objectIDScript.ObjectType == GLOBAL_VARS.ObjectType.isPlayer)
        {
            switch(_hitDirection)
            {
                case GLOBAL_VARS.Direction.down:
                    self_entityScr.triggerDeath();
                break;

                case GLOBAL_VARS.Direction.up:
                case GLOBAL_VARS.Direction.left:
                case GLOBAL_VARS.Direction.right:
                    Vector2 bounceDirection;             
                break;
            }
        }
        else //Will be hit by hat when hat object is made
        {

        }
    }
}
