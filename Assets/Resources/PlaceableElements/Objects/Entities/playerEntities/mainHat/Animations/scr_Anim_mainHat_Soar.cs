using UnityEngine;
using System.Collections.Generic;
using System;

public class scr_Anim_mainHat_Soar : scr_Anim_mainHat_template
{
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();

    public scr_Anim_mainHat_Soar(string _objectName, string _objectType) : base(_objectName, _objectType)  
    {
        loadFrames("sp_mainHat_Soar");
        
        frameData.Add(0, .25f);
        frameData.Add(1, .25f);
        frameData.Add(2, .25f);
        frameData.Add(3, .25f);
    }
    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult)
    {
        //Set animation to loop
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().x),currEntityScript.GetMaxXVelocity());

        autoFlip(_currSpriteRenderer, currEntityScript);

        return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, _entitySpeedMult, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, numOfFrames - 1, true);
    }
}