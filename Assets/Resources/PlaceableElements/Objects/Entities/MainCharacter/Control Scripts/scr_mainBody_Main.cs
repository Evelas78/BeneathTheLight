using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class scr_mainBody_Main : scr_BaseEntity_Main
{
    [SerializeField] private scr_Raycasts_Main raycasterComp = null;
    [SerializeField] private scr_Acceleration_Component accelComp = null;
    public override void CharacterAwake()
    {
        airState = GLOBAL_CONSTANTS.AirStates.Grounded;
        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isPlayer;

        animationControllerInitializer();
        raycasterInitializer();
        accelCompIntializer();
        Debug.Log("This Runs");
    }
    public void animationControllerInitializer()
    {
        animationController = new scr_animController_mainBody();
        animationController.objRenderer = gameObject.GetComponent<SpriteRenderer>();
        animationController.initializeDictionary(gameObject, this);
        animationController.spriteLoad(characterState);
    }
    public void raycasterInitializer()
    {
        raycasterComp = gameObject.AddComponent<scr_Raycasts_Main>();
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
    
    [SerializeField] private int prev_movingHorizontal = 0 ,prev_movingVertical = 0;

    private float maxStrength_X = 0.85f, minStrength_X = 0.25f;
    private float negMaxVelocity_Y = -1f, posMaxVelocity_Y = 0.85f;
    private float raycast_yOffset = 0.1f, raycast_xOffset = 0.05f;

    public override void CharacterStart()
    {
        maxVelocity_X = lowMaxVelocity_X;
        storedMaxYProg = maxProg_PosY;
        MovementStrength_X = minStrength_X;
        MovementStrength_Y = 0f;
    }

    [SerializeField] private int left,right,abilityKey;
    public override void CharacterUpdate()
    {
        left = Input.GetKey(KeyCode.A) ? 1:0 ;
        right = Input.GetKey(KeyCode.D) ? 1:0 ;
        movingVertical = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space) ? 1:0 ;
        movingVertical = Input.GetKey(KeyCode.S) ? -1:movingVertical;
        //Concise conditionals, finally found a good use for it to replace implicit bool->int conversion from gml
        movingHorizontal =  right - left;
        //remember, right is increasing
        abilityKey = Input.GetKey(KeyCode.F) ? 1:0;
        
    }

    //First two factors effect the calculation for x speed while holding it down, final factor is how fast progress decays in a regular fall

    public float maxVelocity_X;
    //These are used in the y function to lerp between the maxVelocities it may have, using the yProgs
    private const float lowMaxVelocity_X = 0.75f, highMaxVelocity_X = 1.5f; 

    public override void CharacterFixedUpdate()
    {   
        handleAirState();
        //Applies acceleration based on how long you are holding the left and right
        XFunc();

        //Applies acceleration for the Y based on how much youre holding it
        YFunc();
        //Check Y Func, but basically, your max prog stored + curr Prog changes how fast your max X Velocity is
        //Remember, the boost of velocity when falling is perpetrated by the fact that its prog is maxprog is much lower than the jumping one and currProg is naturally 0 
        maxVelocity_X= Mathf.Lerp(lowMaxVelocity_X, highMaxVelocity_X, currProg_Y/storedMaxYProg);

        //Clamp velocities by max velocity
        velocity.x = Math.Clamp(velocity.x, -maxVelocity_X, maxVelocity_X);
        velocity.y = Mathf.Clamp(velocity.y, negMaxVelocity_Y, posMaxVelocity_Y);
        
        raycastDownFunction(0, raycast_yOffset);
        raycastRightFunction(raycast_xOffset, 0);
        raycastLeftFunction(raycast_xOffset, 0);
        raycastUpFunction(0, raycast_yOffset);

        determineActionState();
    }
    //Movement strength is determined by the currProg
    [SerializeField] private float MovementStrength_X;

    //CurrProg is used to determine how fast the main char should be going, growing the longer the buttons are held
    [SerializeField]private float currProg_X = 0.0f;
    //When the prog finally hits this percentage from its max, then it starts slowing down
    private const float currProgSlowStart_X = 0.35f;
    //Max Prog of X, the lower it is, the faster it grows
    [SerializeField] private const float maxProg_X = 4.0f;

    //These are used to determine how fast it grows when it starts slowing off
    private const float progMultiplier_XFactor = 3, progMultiplier_XBase = MathF.E;

    public void XFunc()
    { 
        //Remember, this all determines MOVEMENT STRENGTH, not anything else. 

        //Factor is naturally one so it doesn't affect anything, but if we aren't on the ground, then we want to make it so changing directions is a lot slower
        float airChange_XFactor = 1f;  
        //This small part of the code does the whole slowdown checking
        if(airState != GLOBAL_CONSTANTS.AirStates.Grounded)
        {
            if(prev_movingHorizontal != movingHorizontal)
                airChange_XFactor = .25f;
        }
        //If we aren't idle/not moving, and our currProg is not at the max or above it, AND our current movingHorizontal equals our current prevMoving Horizontal,
        //we want it to grow in prog, instead of shrink
        if(movingHorizontal != 0 && currProg_X < maxProg_X && prev_movingHorizontal == movingHorizontal)
        {       
            float _currProg_add_X = .25f;

            //If currProg exceeds the currProgSlowStart threshold, then itll slow down the growth using these factors (with a negative exponent)
            if(currProg_X/maxProg_X  >= currProgSlowStart_X)
            {
                _currProg_add_X = MathF.Pow(progMultiplier_XBase, -(currProg_X/maxProg_X) * progMultiplier_XFactor);
            }

            //How much it increases the currProg
            currProg_X += _currProg_add_X * Time.deltaTime;

            //prevent currProg from going above max
            if(currProg_X > maxProg_X)
            {
                currProg_X = maxProg_X;
            }
        }
        //In the case that our prevMovingHorizontal is in a different direction, we always want to reset our currProg. Also if we're not moving at all, obviously. 
        //Since acceleration component already has friction built in, we don't have to worry about the slowdown part
        else if(movingHorizontal == 0 || prev_movingHorizontal != movingHorizontal)
        {
            currProg_X = 0f; 
        }
        prev_movingHorizontal = movingHorizontal;

        //Movement Strength is lerped from min to max strength, using currProg & maxProg , then checked by airChangeFactor
        MovementStrength_X = Mathf.Lerp(minStrength_X, maxStrength_X, currProg_X/maxProg_X);
        MovementStrength_X *= airChange_XFactor;

        //Apply the acceleration with the raycastgroundhit to check if friction is strong or not
        accelComp.applyAcceleration_X(movingHorizontal, MovementStrength_X, mass, ref velocity, raycast_groundHit);
    }

    //Strength at which force is applied for Y acceleration, determined by prog and its specific type
    [SerializeField] private float MovementStrength_Y;

    //Similar to prog as X, raises over tiem to meet the targetted maxProgs
    [SerializeField]private float currProg_Y  = 0.0f;
    //If above this threshold while jumping then switching to immediate falling, this also determines how much is conserved when switching from active jumping to active falling, rewarding quick enough switches
    private const float maxProg_PosY_conserveFactor = 0.65f;
    //How fast prog will decay when falling (meaning slower fall, but also slower max velocity)
    //ProgChange pos is for jumping, neg is for active falling
    private const float ProgDecay_Y = 9f, ProgChange_PosY = 1.0f, ProgChange_activeNegY = 1.0f;

    //These are determined in Awake using time.deltaTime. Basically, how much the prog should grow/shrink from as it is going in that direction
    [SerializeField] private const float maxProg_PosY = 0.35f, maxProg_NegY = 0.75f;

    //Use these to lerp between to get how fast things fall, etc. Uses prog to grow to them as others do.
    private float maxStrength_PosY = 0.55f, minStrength_PosY = 0.25f;    
    private float maxStrength_NegY = 0.35f, minStrength_NegY = 0.15f;   
    private const float bufferTimeMax = .4f, bufferDecayFactor = 1.0f;
    private float bufferTime = bufferTimeMax;
    public void YFunc()
    {
        //if currrently jumping
        if(airState == GLOBAL_CONSTANTS.AirStates.activeAir)
        {
            movingVertical = 1;
            //This is how strong the movementStrength will be based on the currProg, while sin determines how it decreases. So overall it decreases in a semi exponential sin function (sorry best way to describe it whoever is reading it, im very tired as of right now)
            float movementStrength_Y_Mult = Mathf.Lerp(minStrength_PosY, maxStrength_PosY, currProg_Y/maxProg_PosY);
            float sin_factor = Mathf.PI/2 * (currProg_Y/maxProg_PosY);
            MovementStrength_Y = Mathf.Sin(sin_factor) * movementStrength_Y_Mult;
            
            //Drops curr Prog then clamps it at 0, since it starts at max
            currProg_Y -= ProgChange_PosY * Time.deltaTime;
            currProg_Y = Math.Clamp(currProg_Y, 0f, maxProg_PosY);
            
            //We go straight to falling if we were still holding down the button when its fall time. 
            if(currProg_Y == 0)
            {
                airState = GLOBAL_CONSTANTS.AirStates.Falling;
                movingVertical = 0;
            }
        }
        else if (airState == GLOBAL_CONSTANTS.AirStates.Falling && movingVertical == -1)
        {
            bufferTime -= bufferDecayFactor * Time.deltaTime;
            //If you pressed switch fast enough and currProg is still above a threshold, then your currProg can save into your fast fall, making X velocity max consistently larger.
            if(bufferTime > 0 && currProg_Y >= maxProg_PosY * maxProg_PosY_conserveFactor)
            {
                currProg_Y = maxProg_PosY * maxProg_PosY_conserveFactor;
                Math.Round(currProg_Y);
            }

            prev_movingVertical = movingVertical;
    
            //This grows into how fast you fall
            currProg_Y += ProgChange_activeNegY * Time.deltaTime;
            currProg_Y = Math.Clamp(currProg_Y, 0f, maxProg_NegY);
            MovementStrength_Y = Mathf.Lerp(minStrength_NegY, maxStrength_NegY, currProg_Y/maxProg_NegY);
        }
        else
        {  
            //Decays currProg_Y slowing down max Velocity for X and the fall speed
            currProg_Y -= ProgDecay_Y * Time.deltaTime;
            currProg_Y = Math.Clamp(currProg_Y, 0f, maxProg_NegY);
            MovementStrength_Y = 0f; //Movement strength doesn't matter since we just let natural built in gravity apply.
        }
        
        accelComp.applyAcceleration_Y(movingVertical, MovementStrength_Y, raycast_groundHit, mass, ref velocity);
    }
    [SerializeField] private bool raycast_groundHit = false, didBonk = false;
    [SerializeField] private float bonkThreshold_X = 0.5f, bonkThreshold_Y = 0.75f;
    public void raycastDownFunction(float _xOffset, float _yOffset)
    {
        RaycastHit2D downRaycast = raycasterComp.downRaycast(_xOffset, _yOffset, ref velocity);
        if(downRaycast.collider != null)
        {
            //Debug.Log("Found something (DOWN)" + downRaycast.collider.gameObject.name);

            GameObject hitObject = downRaycast.collider.gameObject;
            IDScript hitIDScript = hitObject.GetComponent<IDScript>();

            int targetType = hitIDScript.objectType;
            switch(targetType)
            {
                case GLOBAL_CONSTANTS.objectType.isEnemy:
                case GLOBAL_CONSTANTS.objectType.isObject:
                case GLOBAL_CONSTANTS.objectType.isWall:
                    bool shouldIgnore = (velocity.y >= 0) ? true : hitIDScript.passThroughDown; 
                    raycasterComp.touchObject(downRaycast, hitObject, hitIDScript, raycast_yOffset, ref velocity, gameObject, GLOBAL_CONSTANTS.Direction.down, shouldIgnore, gameObject);
                    raycast_groundHit = !shouldIgnore;
                    break;
                case GLOBAL_CONSTANTS.objectType.isPlayer: //Means you hit the hat. 
                break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
        else
        {
            raycast_groundHit = false;
        }
    }
    public void raycastUpFunction(float _xOffset, float _yOffset)
    {
        RaycastHit2D upRaycast = raycasterComp.upRaycast(_xOffset, _yOffset, ref velocity);
        if(upRaycast.collider != null)
        {
            //Debug.Log("Found something (UP)" + upRaycast.collider.gameObject.name);

            GameObject hitObject = upRaycast.collider.gameObject;
            IDScript hitIDScript = hitObject.GetComponent<IDScript>();

            int targetType = hitIDScript.objectType;
            switch(targetType)
            { 
                case GLOBAL_CONSTANTS.objectType.isPlayer:
                    break;
                case GLOBAL_CONSTANTS.objectType.isWall:
                case GLOBAL_CONSTANTS.objectType.isObject:
                case GLOBAL_CONSTANTS.objectType.isEnemy:
                    //If your jumping, you have to check if you can naturally pass through it, else if not jumping, you ignore the top since youre falling
                    bool shouldIgnore = (velocity.y <= 0) ? true : hitIDScript.passThroughUp; 

                    //If velocity is greater or equal to the threshold of its max velocity, itll trigger a bonk which will take effect next step, 
                    //since determining state doesn't do anything until after everything runs that step
                    //Leave it behind touchObject since itll clamp velocity before checking
                    if(!shouldIgnore && Math.Abs(velocity.y) >= posMaxVelocity_Y * bonkThreshold_Y)
                    {
                        didBonk = true;
                    }
                    raycasterComp.touchObject(upRaycast, hitObject, hitIDScript, raycast_yOffset, ref velocity, gameObject, GLOBAL_CONSTANTS.Direction.up, shouldIgnore, gameObject);
                    break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    public void raycastRightFunction(float _xOffset, float _yOffset)
    {
        RaycastHit2D rightRaycast = raycasterComp.rightRaycast(_xOffset, _yOffset, ref velocity);
        if(rightRaycast.collider != null)
        {
            Debug.Log("Found something (RIGHT)" + rightRaycast.collider.gameObject.name);

            GameObject hitObject = rightRaycast.collider.gameObject;
            IDScript hitIDScript = hitObject.GetComponent<IDScript>();

            int targetType = hitIDScript.objectType;
            switch(targetType)
            { 
                case GLOBAL_CONSTANTS.objectType.isPlayer:
                    break;
                case GLOBAL_CONSTANTS.objectType.isWall:
                case GLOBAL_CONSTANTS.objectType.isObject:
                case GLOBAL_CONSTANTS.objectType.isEnemy:
                    //If velocity is in the opposite direction, ignore the clamp so you can move, else check if it can pass through normally
                    bool shouldIgnore = (velocity.x <= 0) ? true : hitIDScript.passThroughRight;

                    //If velocity is greater or equal to the threshold of its max velocity, itll trigger a bonk which will take effect next step, 
                    //since determining state doesn't do anything until after everything runs that step
                    //Leave it behind touchObject since itll clamp velocity before checking
                    if(!shouldIgnore && Math.Abs(velocity.x) >= maxVelocity_X * bonkThreshold_X)
                    {
                        didBonk = true;
                    }
                    raycasterComp.touchObject(rightRaycast, hitObject, hitIDScript, raycast_yOffset, ref velocity, gameObject, GLOBAL_CONSTANTS.Direction.right, shouldIgnore, gameObject);
                    break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    public void raycastLeftFunction(float _xOffset, float _yOffset)
    {
        RaycastHit2D leftRaycast = raycasterComp.leftRaycast(_xOffset, _yOffset, ref velocity);
        if(leftRaycast.collider != null)
        {
            Debug.Log("Found something (Left)" + leftRaycast.collider.gameObject.name);

            GameObject hitObject = leftRaycast.collider.gameObject;
            IDScript hitIDScript = hitObject.GetComponent<IDScript>();

            int targetType = hitIDScript.objectType;
            switch(targetType)
            { 
                case GLOBAL_CONSTANTS.objectType.isPlayer:
                    break;
                case GLOBAL_CONSTANTS.objectType.isWall:
                case GLOBAL_CONSTANTS.objectType.isObject:
                case GLOBAL_CONSTANTS.objectType.isEnemy:
                    //If velocity is in the opposite direction, ignore the clamp so you can move, else check if it can pass through normally
                    bool shouldIgnore = (velocity.x >= 0) ? true : hitIDScript.passThroughLeft;

                    //If velocity is greater or equal to the threshold of its max velocity, itll trigger a bonk which will take effect next step, 
                    //since determining state doesn't do anything until after everything runs that step
                    //Leave it behind touchObject since itll clamp velocity before checking
                    if(!shouldIgnore && Math.Abs(velocity.x) >= maxVelocity_X * bonkThreshold_X)
                    {
                        didBonk = true;
                    }
                    raycasterComp.touchObject(leftRaycast, hitObject, hitIDScript, raycast_yOffset, ref velocity, gameObject, GLOBAL_CONSTANTS.Direction.left, hitIDScript.passThroughLeft, gameObject);
                    break;
                default:
                    Debug.LogError("Somehow you hit something that isnt a tracked type (not floor or object)!", gameObject);
                    break;
            }
        }
    }
    
    [SerializeField] bool isPreppingHat = false, releasingHat = false;
    //We multiply our throwStrength by this so our magnitude doesnt make the throw insanely strong
    public const float throwFactor = 0.25f;
    public void hatThrowLogic()
    {
        if(abilityKey == 1)
        {
            if(isPreppingHat == false)
            {
                isPreppingHat = true;
            }
        }
        else if(abilityKey == 0 && isPreppingHat == true)
        {
            isPreppingHat = false;
            releasingHat = true;
        }

        Vector2 mosPosition = Input.mousePosition;
        Vector2 charCurrentPosition = new Vector2(currentRigidBody.transform.position.x, currentRigidBody.transform.position.y);
        mosPosition = charCurrentPosition + mosPosition - new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        Vector2 normalizedAngleVec = mosPosition / mosPosition.magnitude;
        if(isPreppingHat)
        {
            //Plus drawing the arrow too
        }
        else if(releasingHat)
        {
            float throwStrength = mosPosition.magnitude * throwFactor;
            //Throw the hat at the angle, once the animation is done, we can hide our character/reset it,
            // depends on what we find to be better, then we have the hat run its script
        }
    }
    
    //Doesn't allow the character to escape a jump UNTIL it passes the progThreshold (remember, its decreasing so in reality its only 25%)
    private const float maxProg_PosY_threshold = 0.75f;
    
    //Stored maxYProg is the currProg depending on positive/negative, allowing us to calculate max velocity using lerps.
    private float storedMaxYProg;
    //Allows for better fall state switching since we will always be slightly falling due to gravity but being picked back up due to our raycasts
    private const float fallVelocityThreshold = -0.1f;
    public void handleAirState()
    {
        if(raycast_groundHit)
            airState = GLOBAL_CONSTANTS.AirStates.Grounded;
        else if(characterState != GLOBAL_CONSTANTS.AirStates.activeAir && velocity.y < fallVelocityThreshold) //if not jumping AND falling fast enough (since the game cant exactly perfectly land and not fall at the same time, due to how pixels and our grav system works)
            airState = GLOBAL_CONSTANTS.AirStates.Falling;

        //Logic for specifically jumping. Checks first if you can jump and allows a jump, then works for exiting a jump, making sure youve jumped enough before leaving it.
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
            bufferTime = bufferTimeMax;
            currProg_Y = 0;
            airState = GLOBAL_CONSTANTS.AirStates.Falling;
            storedMaxYProg = maxProg_NegY;
        }
    }

    [SerializeField] float runningStateThreshold = 0.4f, sprintingStateThreshold = 0.75f;
    [SerializeField] float bonkEndThreshold = 0.15f;
    [SerializeField] 
    public void determineActionState()
    {   //If currently bonked or has an active bonk then
        if(characterState == GLOBAL_CONSTANTS.CharacterStates.Bonk || didBonk)
        {
            //Check if not bonked yet, change it
            if(characterState != GLOBAL_CONSTANTS.CharacterStates.Bonk)
            {
                characterState = GLOBAL_CONSTANTS.CharacterStates.Bonk;
            }
            else if (Math.Abs(velocity.x) <= bonkEndThreshold * maxVelocity_X) //if already bonked, then check if the velocity is small enough compared to its curr max Velocity
            {
                characterState = GLOBAL_CONSTANTS.CharacterStates.Idle;
            }
            didBonk = false;
        }    

        if(characterState != GLOBAL_CONSTANTS.CharacterStates.Bonk)
        {
            if(airState == GLOBAL_CONSTANTS.AirStates.Grounded)
            {
                if(movingHorizontal != 0)
                {
                    if(Math.Abs(velocity.x) >= sprintingStateThreshold * maxVelocity_X)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Sprinting;
                    else if (Math.Abs(velocity.x) >= runningStateThreshold * maxVelocity_X)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Running;
                    else
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Walking;
                }
                else
                {
                    //Logic Behind This: If not moving, for sure always put it in idle,
                    //else check if its either already skidding so it can go til a stop, 
                    //or initially check to see if you moved fast enough to start skidding. 
                    //If it doesnt fill any of those conditions then default to idle. 
                    //Since this only triggers when you ARENT moving, then this will always work itself back to 0
                    if(velocity.x == 0 && characterState != GLOBAL_CONSTANTS.CharacterStates.Idle)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Idle;
                    else if (characterState == GLOBAL_CONSTANTS.CharacterStates.Skidding || Math.Abs(velocity.x) >= runningStateThreshold * maxVelocity_X)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Skidding;
                    else if (characterState != GLOBAL_CONSTANTS.CharacterStates.Idle)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Idle;
                }
            }
            else
            {   
                if(airState == GLOBAL_CONSTANTS.AirStates.activeAir)
                    if(Math.Abs(velocity.x) >= runningStateThreshold * maxVelocity_X)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Jumping2;
                    else
                        characterState = GLOBAL_CONSTANTS.CharacterStates.Jumping1;
                else if(characterState != GLOBAL_CONSTANTS.CharacterStates.FreeFalling1 || characterState != GLOBAL_CONSTANTS.CharacterStates.FreeFalling2)
                {
                    if(Math.Abs(velocity.x) >= runningStateThreshold * maxVelocity_X)
                        characterState = GLOBAL_CONSTANTS.CharacterStates.FreeFalling2; //Moving fast enough
                    else 
                        characterState = GLOBAL_CONSTANTS.CharacterStates.FreeFalling1;
                }
            }

        }
    }
    public override void DeathUpdate()
    {
        throw new NotImplementedException();
    }

    public float getMaxXVelocity() {return maxVelocity_X;}
    public float getMaxYVelocity() {return posMaxVelocity_Y;}

}
