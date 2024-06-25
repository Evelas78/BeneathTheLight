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


    //These states are pretty integral to our systems
    //Air state is the state whether youre grounded, actively doing something in the air (therefore not bounded by platforms) such as jumping, or falling regularly (in which no passing by platforms).
    [SerializeField]protected int airState;
    //Character state is the MAIN state engine of the game, this'll determine if you're currently in a major state, like walking, hurt, jumpign etc. This doesn't determine animation, essentially just logic, but can be used to determine what animation youre doing
    [SerializeField]protected int characterState = GLOBAL_CONSTANTS.CharacterStates.Walking;
    //Animation state is used to determine current animation that should be playing, meaning its a lot more states possible than characterState


    [SerializeField] protected float mass = 10.0f; 
    [SerializeField] protected UnityEngine.Vector3 velocity = new UnityEngine.Vector3(0,0,0);
    [SerializeField] protected int MaxHealth, Health;
    [SerializeField] protected bool isDead;
    [SerializeField] protected scr_animController animationComp = null;
    
    //Used for when you get hit by something an effect needs to play, 
    //is a list due to the fact multiple effects can happen at once
    //lets say when you hit an enemy and also a boost pad or something
    //First effect grabbed will take priority and we can set up logic within each effect script for an escape if need be
    public List<scr_Effect_Script> spEffectList = new List<scr_Effect_Script>();
    public scr_Effect_Script dmgEffect = null;

    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();

        GameObject gameController = GameObject.Find("gameController");
        gameControllerScript = gameController.GetComponent<scr_gameController>();

        currentEntityGameObject = gameObject; 
        currentRigidBody = currentEntityGameObject.GetComponent<Rigidbody2D>();
        currentCollider = currentEntityGameObject.GetComponent<BoxCollider2D>();

        CharacterAwake();
    }
    void Start()
    {
        CharacterStart();
    }
    void Update()
    {
        CharacterUpdate();   
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
    public abstract void CharacterStart();
    public abstract void CharacterAwake();
    public abstract void CharacterFixedUpdate();
    public abstract void CharacterUpdate();
    public abstract void DeathUpdate();
    //For effect scripts to trigger 
    public void triggerDeath() {isDead = true;}
    
    /*
    private void OnDrawGizmos() {
        UnityEngine.Vector3 cubeDebugSize = new UnityEngine.Vector3(0.15f,0.15f,0.15f);
        UnityEngine.Vector3 edit = currentRigidBody.transform.position;
        
        Gizmos.DrawCube(edit, cubeDebugSize);
    }
    */
}
