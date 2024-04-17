using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class scr_MainChar_Main : scr_Basic_Entity
{
    void Awake()
    {
        //Test
        objectIDs = gameObject.AddComponent<IDScript>();
        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isEntity;

        currentEntityGameObject = gameObject; 
        currentRigidBody = currentEntityGameObject.GetComponent<Rigidbody2D>();
        currentCollider = currentEntityGameObject.GetComponent<BoxCollider2D>();

        MovementStrength = 0.1f;
        //Test

        gravMoveComp = gameObject.AddComponent<scr_Gravity_Component>();
            gravMoveComp.characterScript = this;

        /*
        momentumComp = gameObject.AddComponent<scr_Momentum_Component>();
            momentumComp.characterScript = this;
        */
        

        raycasterComp = gameObject.AddComponent<scr_Raycasts_Main>();
            raycasterComp.characterScript = this;
            
            raycasterComp.colliderXScale = currentCollider.bounds.size.x;
            raycasterComp.colliderYScale = currentCollider.bounds.size.y;

            raycasterComp.depthBasedCollision = true;
            raycasterComp.down_boxCastSize = new UnityEngine.Vector2(raycasterComp.colliderXScale / 2f, raycasterComp.colliderYScale / 16f);

            //Origin of the boxCast will be relative to the position we give. So starting it at the Y (which is the bottom middle of the object)
            raycasterComp.footOffset = raycasterComp.down_boxCastSize.y / 2f;

            raycasterComp.groundLayer = GLOBAL_CONSTANTS.objectType.isFloor << 7; 

    }
    int left,right,down,up;
    int movingHorizontal,movingVertical;
    public override void CharacterUpdate()
    {
        Debug.Log("CUPDATE");
        left = Input.GetKey(KeyCode.A) ? 1:0 ;
        right = Input.GetKey(KeyCode.D) ? 1:0 ;
        up = Input.GetKey(KeyCode.W) ? 1:0 ;
        down = Input.GetKey(KeyCode.S) ? 1:0 ;
        //Concise conditionals, finally found a good use for it to replace implicit boolean->int conversion from gml
        movingHorizontal =  right - left;
        //remember, right is increasing
        movingVertical = up - down;
        //I dont think I need a note for this one but im working at 3 am and I need to remember this stuff lol
    }

    public override void CharacterFixedUpdate()
    {
        Debug.Log(movingHorizontal + " " + movingVertical);
        velocity.x += movingHorizontal * MovementStrength;
        velocity.y += movingVertical * MovementStrength;
    }
}
