using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class scr_Basic_Entity : MonoBehaviour
{
    protected IDScript objectIDs;
    public GameObject currentEntityGameObject; 
    public Rigidbody2D currentRigidBody;
    public BoxCollider2D currentCollider;
    public scr_gameController gameControllerScript;
    
    //To pass to our animation controller & generalize logic for movement
    protected UnityEngine.Vector2 movementDirectionsVec;
    [SerializeField] protected int movingHorizontal = 0,movingVertical = 0;
    [SerializeField] protected scr_animController animationController = null;

    //These states are pretty integral to our systems
    //Air state is the state whether youre grounded, actively doing something in the air (therefore not bounded by platforms) such as jumping, or falling regularly (in which no passing by platforms).
    [SerializeField]protected int airState;
    //Character state is the MAIN state engine of the game, this'll determine if you're currently in a major state, like walking, hurt, jumpign etc. This doesn't determine animation, essentially just logic, but can be used to determine what animation youre doing
    [SerializeField]protected int characterState = GLOBAL_CONSTANTS.CharacterStates.Idle, prevState;

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
    //First effect grabbed will take priority and we can set up logic within each effect script for an escape if need be
    public List<scr_Effect_Script> spEffectList = new List<scr_Effect_Script>();
    public scr_Effect_Script dmgEffect = null;

    //Create all necessary components and prime our movementDirectionsVec to prevent it from being made each time we want to change it.
    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();

        GameObject gameController = GameObject.Find("gameController");
        gameControllerScript = gameController.GetComponent<scr_gameController>();

        currentEntityGameObject = gameObject; 
        currentRigidBody = currentEntityGameObject.GetComponent<Rigidbody2D>();
        currentCollider = currentEntityGameObject.GetComponent<BoxCollider2D>();

        movementDirectionsVec = new UnityEngine.Vector2(movingHorizontal,movingVertical);
        CharacterAwake();
    }
    void Start()
    {
        CharacterStart();
    }
    void Update()
    {
        //Basically we just wanna update the movement directions for sprite passing, thats about it.
        movementDirectionsVec.x = movingHorizontal;
        movementDirectionsVec.y = movingVertical;

        CharacterUpdate();   
        if(characterState != prevState)
        {
            Debug.Log("RUNNING CHARACTERSTATE PASS THROUGH");
            animationController.SpriteController(characterState);
            prevState = characterState;
        }
    }
    void FixedUpdate() {
        if(characterState != GLOBAL_CONSTANTS.CharacterStates.Dead)
            CharacterFixedUpdate();
        else
            DeathUpdate();
        //Updates the position at the very end of the frame
        currentRigidBody.transform.position += velocity;
    }

    public UnityEngine.Vector3 getVelocity() { return velocity; }
    public int getCharacterState() { return characterState; }
    public int getAirState() { return airState; }
    public UnityEngine.Vector2 getMovementDirection() { return movementDirectionsVec; }
    public abstract void CharacterStart();
    public abstract void CharacterAwake();
    public abstract void CharacterFixedUpdate();
    public abstract void CharacterUpdate();

    //On death, thisll run 
    public abstract void DeathUpdate();
    public void triggerDeath() {isDead = true;}


}
