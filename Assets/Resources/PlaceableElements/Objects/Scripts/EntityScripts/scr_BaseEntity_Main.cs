using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;

using Vector2 = UnityEngine.Vector2;


//We keep it monobehavior but DONT use any of the built in functions, only because we want to have the component ability
public abstract class scr_BaseEntity_Main : scr_BaseObject
{
    public GameObject currentEntityGameObject; 
    public Rigidbody2D currentRigidBody;
    public BoxCollider2D currentCollider;
    public scr_levelController levelController;
    public bool isAnimated = true, isStationary = false;
    
    //To pass to our animation controller & generalize logic for movement

    [SerializeField] protected int movingHorizontal = 0,movingVertical = 0;
    [SerializeField] protected scr_animController animationController = null;

    //These states are pretty integral to our systems
    //Air state is the state whether youre grounded, actively doing something in the air (therefore not bounded by platforms) such as jumping, or falling regularly (in which no passing by platforms).
    [SerializeField]protected int airState;
    //Character state is the MAIN state engine of the game, this'll determine if you're currently in a major state, like walking, hurt, jumpign etc. This doesn't determine animation, essentially just logic, but can be used to determine what animation youre doing
    [SerializeField]protected int characterState = GLOBAL_VARS.CharacterStates.Idle, prevState;

    //Mass effects movement speeds and useful as a non magic number to tweak speeds of different things quickly. 
    //Lower generally means higher speeds as dictated by newton himself, trust I know him personally hes cool
    [SerializeField] protected float mass = 10.0f; 

    //Use velocity to determine how we move every step. 
    //Suggest using deltaTime to temper whole numbers based on how far you wanna move a second/small period to work per update
    //While not perfectly consistent, itll get the job done more than well enough
    [SerializeField] protected UnityEngine.Vector3 velocity = new UnityEngine.Vector3(0,0,0);
    
    //Basic game stats, necessary for running different things, such as character states and in general, variety
    [SerializeField] protected int MaxHealth, Health;
    [SerializeField] protected bool isDead;

    //Used for when you get hit by something an effect needs to play, 
    //is a list due to the fact multiple effects can happen at once
    //lets say when you hit an enemy and also a boost pad or something
    //First effect grabbed will take priority and we can set up logic within each effect script 
    //for an escape if need be
    public List<scr_Effect_Script> spEffectList = new List<scr_Effect_Script>();
    public scr_Effect_Script dmgEffect = null;
    
    public static event GLOBAL_VARS.slowGameSignal entitySlowSignal;
    public static event GLOBAL_VARS.entityActiveChangeSignal entityActivatedSignal;    
    public static event GLOBAL_VARS.entityActiveChangeSignal entityDeactivatedSignal;    

    //Will always be setup, if it affects the scene, do NOT include it here. Only exceptions are signal 
    //pulling/activating in certain cases. 
    //That and priming other objects such as arrow segments
    public void entityAwake(scr_levelController _levelController)
    {
        GameObject gameController = GameObject.Find("gameController");
        levelController = gameController.GetComponent<scr_levelController>();

        currentEntityGameObject = gameObject; 
        
        currentCollider = currentEntityGameObject.GetComponent<BoxCollider2D>();

        CharacterAwake();
        
        if(!isStationary)
            currentRigidBody = currentEntityGameObject.GetComponent<Rigidbody2D>();
    }

    //This is for AFTER level is primed
    public void entityStart()
    {
        CharacterStart();
    }
    public void entityUpdate(float _entitySpeedMult)
    {

        CharacterUpdate(_entitySpeedMult);   
        if(isAnimated)
        {
            if(characterState != prevState)
            {
                //Debug.Log("Character State Change");
                animationController.spriteLoad(characterState);
                prevState = characterState;
            }

            animationController.SpriteController(_entitySpeedMult);
        }
    }
    public void entityFixedUpdate(float _entitySpeedMult) {
        if(characterState != GLOBAL_VARS.CharacterStates.Dead)
        {
            CharacterFixedUpdate(_entitySpeedMult);
        }
        else
            CharacterDeathUpdate(_entitySpeedMult);
       
        //Updates the position at the very end of the frame
        if(!isStationary)
            currentRigidBody.transform.position += velocity * _entitySpeedMult;
    }

    public UnityEngine.Vector3 getVelocity() { return velocity; }
    public int getCharacterState() { return characterState; }
    public int getAirState() { return airState; }
    public abstract void CharacterStart();
    public abstract void CharacterAwake();
    public abstract void CharacterFixedUpdate(float _entitySpeedMult);
    public abstract void CharacterUpdate(float _entitySpeedMult);

    //On death, thisll run 
    public abstract void CharacterDeathUpdate(float _entitySpeedMult);
    public void triggerDeath() {isDead = true;}

    protected void triggerEntitySlowEvent(float newSpeedPercent, float lerpFactor)
    {
        entitySlowSignal?.Invoke(newSpeedPercent, lerpFactor);
    }
    protected void activateObject(bool isMainChar, bool isNewCamTarget)
    {
        //gameObject.SetActive(true);
        entityActivatedSignal?.Invoke(gameObject, isMainChar, isNewCamTarget);
    }
    protected void deactivateObject(bool wasMainChar, bool wasCamTarget)
    {
        //gameObject.SetActive(false);
        entityDeactivatedSignal?.Invoke(gameObject, wasMainChar, wasCamTarget);
    }


}
