using UnityEngine;
using System;
using Unity.VisualScripting;

public class scr_mainHat_Main : scr_BaseEntity_Main
{
    private scr_Acceleration_Component accelComp;
    private scr_Raycast_Component raycasterComp;
    private float maxVelocity_X = 1.5f;
    private float initialXStrength, currXStrength;
    private float initalYStrength, currYStrength;
    public static IDScript mainHatIDScript;

    public override void CharacterAwake()
    {
        if(mainHatIDScript != null)
        {
            mainHatIDScript = gameObject.AddComponent<IDScript>();
            mainHatIDScript.ObjectType = GLOBAL_VARS.ObjectType.isPlayer;
        }

        objectIDScript = gameObject.AddComponent<IDScript>();
        scr_mainBody_Main.throwingHat += ThrowHat;
    }
    public static string objectType = "Player";
    public void animationControllerInitializer()
    {
        animationController = new scr_animController_mainBody();
        animationController.objRenderer = gameObject.GetComponent<SpriteRenderer>();
        animationController.initializeDictionary(gameObject, this, objectType);
        animationController.spriteLoad(characterState);
    }
    public void raycasterInitializer()
    {
        raycasterComp = gameObject.AddComponent<scr_Raycast_Component>();
        raycasterComp.currCharCollider = currentCollider;
        raycasterComp.depthBasedCollision = true;
    
        //Multiple by local scale so the actual size is correct
        raycasterComp.colliderOffset = currentCollider.offset * currentCollider.gameObject.transform.localScale.x;
        raycasterComp.colliderSize = currentCollider.size * currentCollider.gameObject.transform.localScale.x;
    }
    public void accelCompIntializer()
    {
        accelComp = gameObject.AddComponent<scr_Acceleration_Component>();  
        accelComp.gFrictionApplies = true;
        accelComp.gravApplies = true;
    }
    public override void CharacterStart()
    {
        throw new NotImplementedException();
    }
    public override void CharacterUpdate(float _entityGameSpeed)
    {
        throw new NotImplementedException();
    }
    public override void CharacterFixedUpdate(float _entityGameSpeed)
    {
        throw new NotImplementedException();
    }
    public override void DeathUpdate(float _entitySpeedMult)
    {
        throw new NotImplementedException();
    }

    public void ThrowHat(Vector2 _throwVector)
    {
        float xStrength = _throwVector.x;
        float yStrength = _throwVector.y;

        activateObject(true, true);
    }
    public float GetMaxXVelocity() { return maxVelocity_X; }


}