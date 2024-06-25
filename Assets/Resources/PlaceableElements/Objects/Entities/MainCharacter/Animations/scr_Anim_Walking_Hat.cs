using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class scr_Anim_Walking_Hat : scr_animations
{
    //Frame Cycle is time in seconds
    Dictionary<int, float> frameData = new Dictionary<int, float>();

    public void Awake()
    {
        spriteName = "sp_mainBody_Walking_Hat";
        frameData.Add(0, .25f);
        frameData.Add(1, .25f);
        frameData.Add(2, .25f);
        frameData.Add(3, .25f);
        isCompleteLoop = true;
    }
    public override Sprite animationScript(SpriteRenderer _currSpriteRenderer, scr_Basic_Entity _currEntityScript, scr_animController _currAnimController)
    {
        autoFlip(_currSpriteRenderer, _currEntityScript);

        float currFrameLength = frameData[currFrameNum];
        progressAnim(_currAnimController, currFrameLength, 0, numOfFrames - 1, isCompleteLoop);

        return frameArray[currFrameNum];
    }
}
