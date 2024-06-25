using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class scr_Anim_Idle_Hat : scr_animations
{
    public void Awake()
    {
        spriteName = "sp_mainBody_Idle_Hat";
        isCompleteLoop = true;
    }

    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_Basic_Entity _currEntityScript, scr_animController _currAnimController)
    {
        progressAnim(_currAnimController, 0, 0, numOfFrames - 1, isCompleteLoop);
        autoFlip(_currSpriteRenderer, _currEntityScript);
        return frameArray[0];
    }
}
