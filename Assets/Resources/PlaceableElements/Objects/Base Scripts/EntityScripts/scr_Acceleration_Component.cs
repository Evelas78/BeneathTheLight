using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class scr_Acceleration_Component : MonoBehaviour
{
    //How fast the momentum should decay
    public float friction_strength_x = .2f;
    public float ground_friction_strength_x = .225f;
    public float grav_strength = .02f;
    public UnityEngine.Vector3 entityAcceleration = new UnityEngine.Vector3(0,0,0); 

    //If touching a ground, how much slower/faster should I decay.   
    public bool gFrictionApplies = false; 
    public bool gravApplies = false;

    public void applyAcceleration_X(float _controlDirection, float _entitySpeedMult, float _moveStrength, float _mass, 
    ref UnityEngine.Vector3 entityVelocity, bool _gTouch)
    { 
        float velocity_direc = Math.Sign(entityVelocity.x);
        
        float frictionForce = friction_strength_x * -velocity_direc;
        
        if(gFrictionApplies && _gTouch)
        {
            frictionForce = ground_friction_strength_x * -velocity_direc; 
        }

        float _currForce = _controlDirection * _moveStrength + frictionForce;

        if(_controlDirection == 0)
        {
            if(velocity_direc > 0  && (entityVelocity.x +  _currForce) < 0)
            {
                entityVelocity.x = 0;
                _currForce = 0;
            }
            else if(velocity_direc < 0 && (entityVelocity.x + _currForce) > 0)
            {
                entityVelocity.x = 0;
                _currForce = 0;
            }

        }

        entityAcceleration.x = _currForce / _mass;
        entityVelocity.x += entityAcceleration.x * _entitySpeedMult;

        //Debug.Log("F "  + _currForce + " E_X_ACC" + entityAcceleration.x + " DIR" + _controlDirection);
    }
    public void applyAcceleration_Y(float _controlDirection, float _entitySpeedMult, float _moveStrength, bool _touchingGround, float _mass, 
    ref UnityEngine.Vector3 entityVelocity, bool _ignore)
    {
        float _currForce = _controlDirection * _moveStrength;

        if(gravApplies)
        {
            if((!_ignore && !_touchingGround) || _ignore)
            {
                _currForce += -grav_strength * _mass;
            }
            else if(!_ignore)
            {   
                _currForce = 0;
                entityAcceleration.y = 0f;
                entityVelocity.y = 0f; 
            }
        }

        entityAcceleration.y = _currForce / _mass;
        entityVelocity.y += entityAcceleration.y * _entitySpeedMult;
    }
}
