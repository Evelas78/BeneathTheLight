using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class scr_Anim_mainBody_Skidding_Hat : scr_Anim_mainBody_template
{
    public void Awake()
    {
        spriteName = "sp_mainBody_Skidding_Hat";
        frameData.Add(0, .35f);
        frameData.Add(1, .35f);
        frameData.Add(2, .35f);
        frameData.Add(3, .35f);
    }
    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().x),currEntityScript.getMaxXVelocity());

        progressAnim(_currAnimController, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, numOfFrames - 1, true);

        autoFlip(_currSpriteRenderer, currEntityScript);

        return frameArray[currFrameNum];
    }
}
