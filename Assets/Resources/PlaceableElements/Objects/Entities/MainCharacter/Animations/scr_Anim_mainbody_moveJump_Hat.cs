using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class scr_Anim_mainBody_moveJump_Hat : scr_Anim_mainBody_template
{
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();

    public scr_Anim_mainBody_moveJump_Hat(GameObject _currGameObject) : base(_currGameObject) 
    {
        loadFrames("sp_mainBody_moveJump_Hat");        
        escapeAnimationNum = GLOBAL_CONSTANTS.CharacterStates.FreeFalling2; //regular fall state
        frameData.Add(0, .05f);
        frameData.Add(1, .05f);
    }

    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        //No need to progress frame obviously, always return the 0 frame.
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().y),currEntityScript.getMaxYVelocity());

        return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength);
    }
}
