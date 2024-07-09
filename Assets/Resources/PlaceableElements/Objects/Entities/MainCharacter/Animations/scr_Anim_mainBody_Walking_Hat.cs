using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class scr_Anim_mainBody_Walking_Hat : scr_Anim_mainBody_template
{
    //Frame Cycle is time in seconds
    
    //Tomorrow, find a way to cast our passed in currEntityScript (also find a way to pass it in) to have it saved as our specific mainBodyScript. Try to have a system that can generalize it within the animations script.

    public void Awake()
    {
        spriteName = "sp_mainBody_Walking_Hat";
        frameData.Add(0, .35f);
        frameData.Add(1, .35f);
        frameData.Add(2, .35f);
        frameData.Add(3, .35f);
        frameData.Add(4, .35f);
        frameData.Add(5, .35f);
    }
    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        //Set animation to loop
        
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().x),currEntityScript.getMaxXVelocity());

        progressAnim(_currAnimController, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, numOfFrames - 1, true);

        autoFlip(_currSpriteRenderer, currEntityScript);
        Debug.Log("Curr Frame Num = " + currFrameNum);
        Debug.Log("Curr Frame Prog = " + currFrameProg);
        Debug.Log("Curr Frame Length = " + currFrameLength);
        return frameArray[currFrameNum];
    }
}
