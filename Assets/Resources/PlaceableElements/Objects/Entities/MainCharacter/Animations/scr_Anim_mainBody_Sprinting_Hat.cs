using System;
using UnityEngine;

public class scr_Anim_mainBody_Sprinting_Hat : scr_Anim_mainBody_template
{
    public void Awake()
    {
        spriteName = "sp_mainBody_Sprinting_Hat";
        frameData.Add(0, .15f);
        frameData.Add(1, .125f);
        frameData.Add(2, .125f);
        frameData.Add(3, .125f);
        frameData.Add(4, .125f);
    }
    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        float currFrameLength = frameData[currFrameNum];
        //First frame does not want to speed up
        if(currFrameNum != 0)
        {

            progressAnim(_currAnimController, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 2, 4, true);
        }
        else
            progressAnim(_currAnimController, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, 1, false);

        autoFlip(_currSpriteRenderer, currEntityScript);

        return frameArray[currFrameNum];
    }
}
