/*
using UnityEngine;
using System.Collections.Generic;
public class scr_trans_basicFallBlack : scr_transition
{
    [SerializeField] public static Dictionary<int, float> frameData = new Dictionary<int, float>();
    public scr_trans_basicFallBlack(string _folderName, string _type) : base(_folderName, _type)
    {
        loadFrames("sp_transBlackRaise");
        frameData.Add(0, 25f);

    }
    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult)
    {
        //No need to progress frame obviously, always return the 0 frame.
        float currFrameLength = frameData[currFrameNum];
        currFrameLength *= speedUpAnim(Math.Abs(currEntityScript.getVelocity().y),currEntityScript.getMaxYVelocity());

        return progressAnim(this, _currAnimController, numOfFrames, escapeAnimationNum, _entitySpeedMult, ref currFrameProg, ref currFrameNum, ref currLoopCount, currFrameLength);
    }
}
*/