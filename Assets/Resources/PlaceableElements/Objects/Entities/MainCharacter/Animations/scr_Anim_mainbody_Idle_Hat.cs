using UnityEngine;

public class scr_Anim_mainBody_Idle_Hat : scr_Anim_mainBody_template
{
    public scr_Anim_mainBody_Idle_Hat(GameObject _currGameObject) : base(_currGameObject) 
    {
        loadFrames("sp_mainBody_Idle_Hat");
    }

    public override bool animationScript(SpriteRenderer _currSpriteRenderer, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount)
    {
        //No need to progress frame obviously, always return the 0 frame.
        autoFlip(_currSpriteRenderer, currEntityScript);
        return false;
    }
}
