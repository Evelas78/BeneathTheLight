using UnityEngine;
using System;
using Unity.VisualScripting;

public class scr_mainHat_Main : scr_BaseEntity_Main
{
    private scr_Acceleration_Component accelComp;
    private scr_Raycast_Component raycasterComp;
    private float maxVelocity_X = 1.5f;
    [SerializeField] private float initialXStrength, currXStrength;
    [SerializeField] private float initialYStrength, currYStrength;
    [SerializeField] private int xDirection, yDirection;
    [SerializeField] private float prevXVel, prevYVel;
    private float currDegAngle;
    public static IDScript mainHatIDScript;

    private float raycast_yOffset = 0.1f, raycast_xOffset = 0.1f;
    public override void CharacterAwake()
    {
        if(mainHatIDScript == null)
        {
            mainHatIDScript = new IDScript();
            mainHatIDScript.ObjectType = GLOBAL_VARS.ObjectType.isPlayer;
        }

        objectIDScript = mainHatIDScript;
        characterState = GLOBAL_VARS.CharacterStates.FreeFalling1;

        accelCompIntializer();
        raycasterInitializer();
        animationControllerInitializer();
        scr_mainBody_Main.throwingHat += ThrowHat;
        mass = 15.0f;
    }
    public static string objectType = "Player";
    
    public void animationControllerInitializer()
    {
        animationController = new scr_animController_mainHat();
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
        accelComp.gFrictionApplies = false;
        accelComp.gravApplies = true;
    }
    public override void CharacterStart() { Debug.Log("Main Hat start ran"); } //Doesnt need one as of yet
    public override void CharacterUpdate(float _entityGameSpeed)
    {
        //This is for the animation controller and thats litterally it
    }
    public override void CharacterFixedUpdate(float _entityGameSpeed)
    {
        XFunc(_entityGameSpeed);
        YFunc(_entityGameSpeed);



        //Bounce detection should go here, since we clamp velocity there
        //Since we clamp velocity, all that matters from here is the direction our hat will go in
        raycastFunctionality(_entityGameSpeed);
    }

    void raycastFunctionality(float _entityGameSpeed)
    {
        //Add all raycasts here
        raycastDownFunction(0, raycast_yOffset, _entityGameSpeed);
        raycastUpFunction(0, raycast_yOffset, _entityGameSpeed);
        raycastLeftFunction(raycast_xOffset, 0, _entityGameSpeed);
        raycastRightFunction(raycast_xOffset, 0, _entityGameSpeed);
    }

    void raycastDownFunction(float _xOffset, float _yOffset, float _entityGameSpeed)
    {
        RaycastHit2D downRaycast = raycasterComp.downRaycast(_xOffset, _yOffset, ref velocity, _entityGameSpeed);
        if(downRaycast.collider != null)
        {
            //Debug.Log(gameObject + " Found something (DOWN)" + downRaycast.collider.gameObject.name);

            GameObject hitObject = downRaycast.collider.gameObject;
            scr_BaseEntity_Main hitMainScript = hitObject.GetComponent<scr_BaseEntity_Main>();
            IDScript hitIDScript = hitMainScript.objectIDScript;

            int targetType = hitIDScript.ObjectType;
            switch(targetType)
            {
                case GLOBAL_VARS.ObjectType.isEnemy:
                break;
                case GLOBAL_VARS.ObjectType.isObject:
                break;
                case GLOBAL_VARS.ObjectType.isWall:
                    bounceHit(GLOBAL_VARS.Direction.down, downRaycast);
                    break;
                case GLOBAL_VARS.ObjectType.isPlayer: //Means you hit the hat. 
                break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    void raycastUpFunction(float _xOffset, float _yOffset, float _entityGameSpeed)
    {
        RaycastHit2D upRaycast = raycasterComp.upRaycast(_xOffset, _yOffset, ref velocity, _entityGameSpeed);
        if(upRaycast.collider != null)
        {
            //Debug.Log(gameObject + " Found something (UP)" + upRaycast.collider.gameObject.name);

            GameObject hitObject = upRaycast.collider.gameObject;
            scr_BaseEntity_Main hitMainScript = hitObject.GetComponent<scr_BaseEntity_Main>();
            IDScript hitIDScript = hitMainScript.objectIDScript;

            int targetType = hitIDScript.ObjectType;
            switch(targetType)
            {
                case GLOBAL_VARS.ObjectType.isEnemy:
                break;
                case GLOBAL_VARS.ObjectType.isObject:
                break;
                case GLOBAL_VARS.ObjectType.isWall:
                    bounceHit(GLOBAL_VARS.Direction.up, upRaycast);
                    break;
                case GLOBAL_VARS.ObjectType.isPlayer: //Means you hit the hat. 
                break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    void raycastLeftFunction(float _xOffset, float _yOffset, float _entityGameSpeed)
    {
        RaycastHit2D leftRaycast = raycasterComp.leftRaycast(_xOffset, _yOffset, ref velocity, _entityGameSpeed);
        if(leftRaycast.collider != null)
        {
            Debug.Log(gameObject + " Found something (LEFT)" + leftRaycast.collider.gameObject.name);

            GameObject hitObject = leftRaycast.collider.gameObject;
            scr_BaseEntity_Main hitMainScript = hitObject.GetComponent<scr_BaseEntity_Main>();
            IDScript hitIDScript = hitMainScript.objectIDScript;

            int targetType = hitIDScript.ObjectType;
            switch(targetType)
            {
                case GLOBAL_VARS.ObjectType.isEnemy:
                break;
                case GLOBAL_VARS.ObjectType.isObject:
                break;
                case GLOBAL_VARS.ObjectType.isWall:
                    bounceHit(GLOBAL_VARS.Direction.left, leftRaycast);
                    break;
                case GLOBAL_VARS.ObjectType.isPlayer: //Means you hit the hat. 
                break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    void raycastRightFunction(float _xOffset, float _yOffset, float _entityGameSpeed)
    {
        RaycastHit2D rightRaycast = raycasterComp.rightRaycast(_xOffset, _yOffset, ref velocity, _entityGameSpeed);
        if(rightRaycast.collider != null)
        {
            Debug.Log(gameObject + " Found something (RIGHT)" + rightRaycast.collider.gameObject.name);

            GameObject hitObject = rightRaycast.collider.gameObject;
            scr_BaseEntity_Main hitMainScript = hitObject.GetComponent<scr_BaseEntity_Main>();
            IDScript hitIDScript = hitMainScript.objectIDScript;

            int targetType = hitIDScript.ObjectType;
            switch(targetType)
            {
                case GLOBAL_VARS.ObjectType.isEnemy:
                break;
                case GLOBAL_VARS.ObjectType.isObject:
                break;
                case GLOBAL_VARS.ObjectType.isWall:
                    bounceHit(GLOBAL_VARS.Direction.right, rightRaycast);
                    break;
                case GLOBAL_VARS.ObjectType.isPlayer: //Means you hit the hat. 
                break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    void bounceHit(int _direction, RaycastHit2D _currRaycast)
    {
        GameObject hitObject = _currRaycast.rigidbody.gameObject;
        scr_BaseObject hitTargetScript = hitObject.GetComponent<scr_BaseObject>();
        IDScript hitTarget_ID = hitTargetScript.objectIDScript;
        //if our current rigid bodies position is more left than our hitpoint, meaning its gonna bounce towards our left
        bool shouldIgnore = true;
        switch(_direction)
        {
            case GLOBAL_VARS.Direction.up:
                //Since less than or equal to 0, then its going down so it shouldnt collide, else we check if the hitObject can be passThrough from below
                shouldIgnore = (velocity.y <= 0) ? true : hitTarget_ID.passThroughUp; 
                yDirection = -1;               
                break;
            case GLOBAL_VARS.Direction.down:
                shouldIgnore = (velocity.y >= 0) ? true : hitTarget_ID.passThroughDown; 
                yDirection = 1;
                break;
            case GLOBAL_VARS.Direction.left:
                //Going right
                shouldIgnore = (velocity.x >= 0) ? true : hitTarget_ID.passThroughLeft; 
                xDirection = 1;
                if(!shouldIgnore)
                    wallTouch = true;
                break;
            case GLOBAL_VARS.Direction.right:
                shouldIgnore = (velocity.x <= 0) ? true : hitTarget_ID.passThroughRight; 
                xDirection = -1;
                if(!shouldIgnore)
                    wallTouch = true;
                break;
            default:
                Debug.LogError("Direction doesn't exist in bounceHit for mainHat script");
                break;
        }
        prevYVel = velocity.y;
        prevXVel = velocity.x;
        raycasterComp.touchObject(_currRaycast, hitObject, hitTarget_ID, ref velocity, gameObject, _direction, shouldIgnore);
    }
    const float regularDecrease_X = 1.0f, wallDecrease = 2.5f;
    bool wallTouch = false;
    void XFunc(float _entityGameSpeed)
    {
        if(wallTouch)
        {
            currXStrength -= wallDecrease;
            currXStrength = Mathf.Clamp(currXStrength, 0, Mathf.Infinity);
            wallTouch = false;
        }

        accelComp.applyAcceleration_X(xDirection, _entityGameSpeed, currXStrength, mass, ref velocity, false /*G touch doesnt matter*/);

        //Reverses direction of prevXVel after hitting the wall
        if(wallTouch)
            velocity.x = -prevXVel;

        currXStrength -= regularDecrease_X * Time.deltaTime * _entityGameSpeed;
        currXStrength = Mathf.Clamp(currXStrength, 0, Mathf.Infinity);
    }
    //Bounce strength threshold must be greater than decay factor
    //Bounce Strength Threshold is the minimum remaining strength to bounce
    //ceilignDecrease is how much it decays every roof/floor hit
    const float regularDecrease_Y = 1.0f, ceilingDecrease = 5.5f, fallIncrease = 0.25f;
    bool floorTouch = false;
    void YFunc(float _entityGameSpeed)
    {
        //Figure this out when i wake up.
        if(floorTouch)
        {
            currYStrength -= ceilingDecrease;
            currYStrength = Mathf.Clamp(currYStrength, 0, Mathf.Infinity);
        }
        //Apply acceleration here
        accelComp.applyAcceleration_Y(yDirection, _entityGameSpeed, currYStrength, floorTouch, mass, ref velocity, false);
        
        if(floorTouch)
        {
            velocity.y = -prevYVel;
            floorTouch = false;
        }
        //Check if greater than 0, if so, then we want to decrease strength as its fighting gravity.
        if(velocity.y >= 0)
            currYStrength -= regularDecrease_Y * Time.deltaTime * _entityGameSpeed;
        else
            currYStrength += fallIncrease * Time.deltaTime * _entityGameSpeed;

        currYStrength = Mathf.Clamp(currYStrength, 0, Mathf.Infinity);
        //Same thing above, also setup a decay function
    }
    public override void CharacterDeathUpdate(float _entitySpeedMult) { Debug.LogError("The hat threw a CharacterDeathUpdate. This litterally cant happen");}

    public void ThrowHat(Vector2 _throwVector, Vector2 _startPos)
    {
        activateObject(true, true);

        currentRigidBody.position = _startPos;
        
        initialXStrength = Math.Abs(_throwVector.x);
        initialYStrength = Math.Abs(_throwVector.y);

        currXStrength = initialXStrength;
        currYStrength = initialYStrength;

        xDirection = Math.Sign(_throwVector.x);
        yDirection = Math.Sign(_throwVector.y);

        characterState = GLOBAL_VARS.CharacterStates.FreeFalling1;
        Debug.Log("Throw Hat Ran");
    }
    public float GetMaxXVelocity() { return maxVelocity_X; }


}