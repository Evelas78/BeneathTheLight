using System;
using UnityEngine;

public class scr_Anim_mainBody_Running_Hat : scr_Anim_mainBody_template
{
    //Frame Cycle is time in seconds
    
    //Tomorrow, find a way to cast our passed in currEntityScript (also find a way to pass it in) to have it saved as our specific mainBodyScript. Try to have a system that can generalize it within the animations script.

    public void Awake()
    {
        spriteName = "sp_mainBody_Running_Hat";
        frameData.Add(0, .25f);
        frameData.Add(1, .25f);
        frameData.Add(2, .25f);
        frameData.Add(3, .25f);
    }
    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        //Set animation to loop
        
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().x),currEntityScript.getMaxXVelocity());

        progressAnim(_currAnimController, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, numOfFrames - 1, true);

        autoFlip(_currSpriteRenderer, currEntityScript);

        return frameArray[currFrameNum];
    }
}

