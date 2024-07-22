using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class scr_Anim_mainBody_Walk_Hat : scr_Anim_mainBody_template
{
    //Frame Cycle is time in seconds
    
    //In the order of the frames provided, this provides the time, in a float, of how long that frame will last 
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();

    //Just Inherit
    public scr_Anim_mainBody_Walk_Hat(GameObject _currGameObject) : base(_currGameObject) 
    {
        loadFrames("sp_mainBody_Walk_Hat");
        
        frameData.Add(0, .35f);
        frameData.Add(1, .35f);
        frameData.Add(2, .35f);
        frameData.Add(3, .35f);
        frameData.Add(4, .35f);
        frameData.Add(5, .35f);
    }
    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        //Set animation to loop
        
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().x),currEntityScript.getMaxXVelocity());

        autoFlip(_currSpriteRenderer, currEntityScript);

        return  progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength, 0, numOfFrames - 1, true);
    }
}
