using System;
using UnityEngine;
using System.Collections.Generic;

public class scr_Anim_mainBody_Throw_Hat : scr_Anim_mainBody_template
{
    //Frame Cycle is time in seconds
    

    //In the order of the frames provided, this provides the time, in a float, of how long that frame will last 
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();

    public scr_Anim_mainBody_Throw_Hat(string _objectName, string _objectType) : base(_objectName, _objectType) 
    {
        loadFrames("sp_mainBody_Throw_Hat");
        
        frameData.Add(0, .25f);
        frameData.Add(1, .15f);
        frameData.Add(2, .15f);
        frameData.Add(3, .45f);
        frameData.Add(4, .15f);
        frameData.Add(5, .30f);
        frameData.Add(6, .1f);
        frameData.Add(7, .1f);
        frameData.Add(8, .1f);
        frameData.Add(9, .1f);
        frameData.Add(10, .1f);
    }
    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult)
    {

        float currFrameLength = frameData[currFrameNum];
        
        return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, _entitySpeedMult, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, numOfFrames - 1, false);
    }
}

