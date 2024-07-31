using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class scr_animController_mainHat : scr_animController
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
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.FreeFalling1, newAnim);
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