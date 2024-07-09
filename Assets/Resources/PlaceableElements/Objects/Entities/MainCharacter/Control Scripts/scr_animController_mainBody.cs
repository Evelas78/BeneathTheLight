using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class scr_animController_mainBody : scr_animController
{   

    protected bool dictionaryMade = false;
    public override void initializeDictionary()
    {
        //LOOK INTO SCRIPTABLE OBJECTS THOUGH (nvm, this is for future reading, this is good for setting templates of stuff, but not much else. No need to replace anything)
        scr_animations newAnim = gameObject.AddComponent<scr_Anim_mainBody_Idle_Hat>();
        newAnim.convertToSpecificScript(currEntityScript);
        animationDictionary.Add(GLOBAL_CONSTANTS.CharacterStates.Idle, newAnim);
        
        newAnim = gameObject.AddComponent<scr_Anim_mainBody_Walking_Hat>();
        newAnim.convertToSpecificScript(currEntityScript);
        animationDictionary.Add(GLOBAL_CONSTANTS.CharacterStates.Walking, newAnim);

        newAnim = gameObject.AddComponent<scr_Anim_mainBody_Running_Hat>();
        newAnim.convertToSpecificScript(currEntityScript);
        animationDictionary.Add(GLOBAL_CONSTANTS.CharacterStates.Running, newAnim);
    
        newAnim = gameObject.AddComponent<scr_Anim_mainBody_Sprinting_Hat>();
        newAnim.convertToSpecificScript(currEntityScript);
        animationDictionary.Add(GLOBAL_CONSTANTS.CharacterStates.Sprinting, newAnim);

        newAnim = gameObject.AddComponent<scr_Anim_mainBody_Skidding_Hat>();
        newAnim.convertToSpecificScript(currEntityScript);
        animationDictionary.Add(GLOBAL_CONSTANTS.CharacterStates.Skidding, newAnim);
    }

    public override void SpriteController(int _characterState)
    {
        scr_animations newAnimation;
        Debug.Log("Chara State: " + _characterState);
        try
        {
            if(animationDictionary.TryGetValue(_characterState, out newAnimation))
            {
                Debug.Log("Animation Loaded " + newAnimation.name);
                spriteLoad(newAnimation);
            }
            else
            {
                Debug.LogError("No Animation found at this key");
            }
        }
        catch(KeyNotFoundException)
        {
            Debug.LogError("Key does not exist in mainBody dictionary = " + _characterState);
        }
    }



}