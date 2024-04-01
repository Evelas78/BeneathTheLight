using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEngine;

public class scr_Momentum_Component : MonoBehaviour
{
    public scr_Basic_Entity characterScript;
    
    //We clamp here
    public float max_change_x = 2.0f;
    public float max_change_y = 2.0f;

    //As this gets so small, the entirety should decay instantly.
    public float instant_decay_max_x = .0001f;
    public float instant_decay_max_y = .0001f;

    //How fast the momentum should decay
    public float decay_speed_x = .05f;

    //If touching a ground, how much slower/faster should I decay.   
    bool frictionApplies = false; 

    public void momentumFixedUpdate()
    {
        if(characterScript.changeTo.x > instant_decay_max_x)
        {
            characterScript.changeTo.x -= characterScript.changeTo.x * decay_speed_x; 
        }

    }
}
