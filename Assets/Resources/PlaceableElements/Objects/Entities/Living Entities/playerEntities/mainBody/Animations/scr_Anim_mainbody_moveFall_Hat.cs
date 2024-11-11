using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class scr_Anim_mainBody_moveFall_Hat : scr_Anim_mainBody_template
{
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();

    public scr_Anim_mainBody_moveFall_Hat(string _objectName, string _objectType) : base(_objectName, _objectType) 
    {
        loadFrames("sp_mainBody_moveFall_Hat");        

        frameData.Add(0, .2f);
        frameData.Add(1, .2f);
        frameData.Add(2, .2f);
        frameData.Add(3, .2f);
        frameData.Add(4, .2f);
        frameData.Add(5, .2f);
    }

    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult)
    {
        //No need to progress frame obviously, always return the 0 frame.
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().y),currEntityScript.getMaxYVelocity());

        int startFrame = 0;
        int endFrame = 2;
        if(currEntityScript.getVelocity().y < 0)
        {
            startFrame = 3;
            endFrame = 5;
            if(currFrameNum < startFrame)
            {
                currFrameNum = startFrame;
            }
        }

        autoFlip(_currSpriteRenderer, currEntityScript);
        
        return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, _entitySpeedMult, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, startFrame, endFrame, true);
    }
}
