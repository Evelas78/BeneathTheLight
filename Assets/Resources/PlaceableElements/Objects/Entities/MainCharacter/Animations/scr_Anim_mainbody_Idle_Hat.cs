using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class scr_Anim_mainBody_Idle_Hat : scr_Anim_mainBody_template
{
    public void Awake()
    {
        spriteName = "sp_mainBody_Idle_Hat";
    }

    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        //No need to progress frame obviously, always return the 0 frame.
        autoFlip(_currSpriteRenderer, currEntityScript);
        return frameArray[0];
    }
}
