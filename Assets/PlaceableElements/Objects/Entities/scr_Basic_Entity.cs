using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;

public abstract class scr_Basic_Entity : MonoBehaviour
{
    protected IDScript objectIDs;
    protected scr_Gravity_Component gravMoveComp = null;
    protected scr_Raycasts_Main raycasterComp = null;
    protected scr_Momentum_Component momentumComp = null;

    Action gravMoveFixedUpdate = null;
    Action raycasterFixedUpdate = null;
    Action momentumFixedUpdate = null; 

    public GameObject currentEntityGameObject; 
    public Rigidbody2D currentRigidBody;
    public BoxCollider2D currentCollider;
    public int characterState = GLOBAL_CONSTANTS.CharacterStates.Running;
    public float MovementStrength;

    private float mass = 10.0f; 
    public UnityEngine.Vector3 acceleration = new UnityEngine.Vector3(0,0,0);
    public UnityEngine.Vector3 velocity = new UnityEngine.Vector3(0,0,0);

    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();
        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isEntity;

        currentEntityGameObject = gameObject; 
        currentRigidBody = currentEntityGameObject.GetComponent<Rigidbody2D>();
        currentCollider = currentEntityGameObject.GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        if(gravMoveComp != null)
        {
            gravMoveFixedUpdate = gravMoveComp.GravityUpdate;
        }

        if(raycasterComp != null)
        {
            raycasterFixedUpdate = raycasterComp.RaycastFixedUpdate; 
        }
        
        if(momentumComp != null)
        {
            momentumFixedUpdate = momentumComp.momentumFixedUpdate;
        }
        if(raycasterComp != null)
        {
            raycasterFixedUpdate = raycasterComp.RaycastFixedUpdate;
        }
    }
    void Update()
    {
        CharacterUpdate();   
    }
    void FixedUpdate() {

        if(gravMoveFixedUpdate != null)
            {gravMoveFixedUpdate.Invoke();
                    Debug.Log("GravUpdate");}
        CharacterFixedUpdate();
        if(momentumFixedUpdate != null)
            {momentumFixedUpdate.Invoke();}
        if(raycasterFixedUpdate != null)
            {raycasterFixedUpdate.Invoke();}

        //Updates the position at the very end of the frame
        currentRigidBody.transform.position += velocity;

    }
    public abstract void CharacterFixedUpdate();
    public abstract void CharacterUpdate();

}
