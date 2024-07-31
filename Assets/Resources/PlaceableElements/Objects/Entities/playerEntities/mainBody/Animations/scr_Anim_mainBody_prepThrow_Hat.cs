using System;
using UnityEngine;
using System.Collections.Generic;

public class scr_Anim_mainBody_prepThrow_Hat : scr_Anim_mainBody_template
{
    public scr_Anim_mainBody_prepThrow_Hat(string _objectName, string _objectType) : base(_objectName, _objectType) 
    {
        loadFrames("sp_mainBody_prepThrow_Hat");
    }

    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult)
    {
        //No need to progress frame obviously, always return the 0 frame.
        autoFlip(_currSpriteRenderer, currEntityScript);
        return false;
    }
}

