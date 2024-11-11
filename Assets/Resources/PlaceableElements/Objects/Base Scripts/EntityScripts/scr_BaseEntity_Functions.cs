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

    public static scr_Raycast_Component raycasterInitializer(GameObject gameObject, BoxCollider2D currentCollider)
    {
        scr_Raycast_Component raycasterComponent = gameObject.AddComponent<scr_Raycast_Component>();
        raycasterComponent.currCharCollider = currentCollider;
        raycasterComponent.depthBasedCollision = true;
    
        //Multiple by local scale so the actual size is correct
        raycasterComponent.colliderOffset = currentCollider.offset * currentCollider.gameObject.transform.localScale.x;
        raycasterComponent.colliderSize = currentCollider.size * currentCollider.gameObject.transform.localScale.x;
        raycasterComponent.InstantiateRaycastComp();

        return raycasterComponent;
    }

    public static scr_animController animationControllerInitializer(GameObject gameObject, scr_BaseEntity_Main mainScript, String objectType, int initialState)
    {
        scr_animController animationController = new scr_animController_mainBody();
        animationController.objRenderer = gameObject.GetComponent<SpriteRenderer>();

        animationController.initializeDictionary(gameObject, mainScript, objectType);
        animationController.spriteLoad(initialState);

        return animationController;
    }
}