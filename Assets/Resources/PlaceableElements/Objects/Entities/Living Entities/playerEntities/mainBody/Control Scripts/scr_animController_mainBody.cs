using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class scr_animController_mainBody : scr_animController
{   
    public static Dictionary<int,scr_animations> animationDictionary = new Dictionary<int, scr_animations>(); 
    protected static bool dictionaryMade = false;

    public override void initializeDictionary(GameObject _gameObject, scr_BaseEntity_Main _currEntityScript, string _objectType)
    {
        if(!dictionaryMade)
        {
            //LOOK INTO SCRIPTABLE OBJECTS THOUGH (nvm, this is for future reading, this is good for setting templates of stuff, but not much else. No need to replace anything)
            scr_animations newAnim = new scr_Anim_mainBody_Idle_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Idle, newAnim);
            
            
            newAnim = new scr_Anim_mainBody_Walk_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Walking, newAnim);

            newAnim = new scr_Anim_mainBody_Run_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Running, newAnim);
        
            newAnim = new scr_Anim_mainBody_Sprint_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Sprinting, newAnim);

            newAnim = new scr_Anim_mainBody_Skid_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Skidding, newAnim);



            newAnim = new scr_Anim_mainBody_Fall_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.FreeFalling1, newAnim);

            newAnim = new scr_Anim_mainBody_moveFall_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.FreeFalling2, newAnim);

            newAnim = new scr_Anim_mainBody_Jump_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Jumping1, newAnim);

            newAnim = new scr_Anim_mainBody_moveJump_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Jumping2, newAnim);



            newAnim = new scr_Anim_mainBody_prepThrow_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.prepAbility1, newAnim);

            newAnim = new scr_Anim_mainBody_slowMove_prepThrow_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.prepAbility2, newAnim);

            newAnim = new scr_Anim_mainBody_fastMove_prepThrow_Hat(_gameObject.name, _objectType);
            newAnim.convertToSpecificScript(_currEntityScript);
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.prepAbility3, newAnim);
        
        }
    }

    public override void spriteLoad(int _characterState)
    {
        scr_animations newAnimation;

        try
        {
            if(animationDictionary.TryGetValue(_characterState, out newAnimation))
            {
                //Debug.Log("Animation Loaded " + nameof(newAnimation));
                spriteLoad(newAnimation);
            }
            else
            {
                Debug.LogError("No Animation found at this key " + _characterState);
            }
        }
        catch(KeyNotFoundException)
        {
            Debug.LogError("Key does not exist in mainBody dictionary = " + _characterState);
        }

    }



}