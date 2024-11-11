using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class scr_Anim_mainBody_Sprint_Hat : scr_Anim_mainBody_template
{
    public scr_Anim_mainBody_Sprint_Hat(string _objectName, string _objectType) : base(_objectName, _objectType) 
    {
        loadFrames("sp_mainBody_Sprint_Hat");
        
        frameData.Add(0, .15f);
        frameData.Add(1, .125f);
        frameData.Add(2, .125f);
        frameData.Add(3, .125f);
        frameData.Add(4, .125f);
    }


    //In the order of the frames provided, this provides the time, in a float, of how long that frame will last 
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();
    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult)
    {
        float currFrameLength = frameData[currFrameNum];
        autoFlip(_currSpriteRenderer, currEntityScript);
        //First frame does not want to speed up
        if(currFrameNum != 0)
        {
            return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, _entitySpeedMult, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 2, 4, true);
        }
        else
            return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, _entitySpeedMult, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, 1, false);

    }
}
