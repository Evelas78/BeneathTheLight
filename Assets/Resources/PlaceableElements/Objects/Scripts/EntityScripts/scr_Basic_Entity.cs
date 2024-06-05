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
    public int airState;
    public int actionState;
    [SerializeField] protected float mass = 10.0f; 
    [SerializeField] protected UnityEngine.Vector3 velocity = new UnityEngine.Vector3(0,0,0);
    [SerializeField] protected int MaxHealth, Health;

    [SerializeField] protected scr_animController animationComp = null;
    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();

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
        CharacterFixedUpdate();

        //Updates the position at the very end of the frame
        currentRigidBody.transform.position += velocity;
    }

    public UnityEngine.Vector3 getVelocity()
    {
        return velocity;
    }
    public abstract void CharacterStart();
    public abstract void CharacterAwake();
    public abstract void CharacterFixedUpdate();
    public abstract void CharacterUpdate();

}
