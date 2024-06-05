using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_mainBody_acr : scr_animController
{
    //Creating an Enum to make it easier to see which animations ar ebeing used
    public enum SpriteKey
    {
        Idle = GLOBAL_CONSTANTS.ActionStates.Idle,
        Walking = GLOBAL_CONSTANTS.ActionStates.Walking,
        Jumping = GLOBAL_CONSTANTS.ActionStates.Jumping
    }
    public override void fillDictionary()
    {
        scr_animations currAnim = gameObject.AddComponent<scr_Hat_Idle>();
        animationDictionary.Add((int)SpriteKey.Idle, currAnim);
    }
    public override Sprite determineSpriteUpdate()
    {
        Sprite targetSprite = animationDictionary[(int)SpriteKey.Idle].animationScript();
        
        return targetSprite;
    }
}
