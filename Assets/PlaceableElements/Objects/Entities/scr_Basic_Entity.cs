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
    public UnityEngine.Vector3 changeTo = new UnityEngine.Vector3(0,0,0);

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
        Debug.Log("REGUPDATE");
        CharacterUpdate();   
    }
    void FixedUpdate() {
        Debug.Log("FIXUPDATE");
        if(gravMoveFixedUpdate != null)
            {gravMoveFixedUpdate.Invoke();}
        CharacterFixedUpdate();
        if(momentumFixedUpdate != null)
            {momentumFixedUpdate.Invoke();}
        if(raycasterFixedUpdate != null)
            {raycasterFixedUpdate.Invoke();}

        //Updates the position at the very end of the frame
        currentRigidBody.transform.position += changeTo;

    }
    public abstract void CharacterFixedUpdate();
    public abstract void CharacterUpdate();

}
