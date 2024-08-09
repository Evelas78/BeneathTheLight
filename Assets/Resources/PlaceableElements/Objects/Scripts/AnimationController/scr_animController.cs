using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class scr_animController 
{   
    //We need the object renderer in order to actually reference the current objects spriteRenderer and load in the current sprite
    public SpriteRenderer objRenderer;

    [SerializeField] protected scr_animations currAnim = null;

    //Logical vars for going through the frames of the animation
    [SerializeField] protected float currFrameProg = 0;
    [SerializeField] protected int currFrameNum = 0;
    
    //As most functions will contain a loop of frames, this is here to keep track of it
    [SerializeField] protected int currLoopCount = 0;
    
    //thisll be called everytime we want to play a new animation/reset
    public void spriteLoad(scr_animations _targetAnim)
    {
        //Debug.Log("We loaded new Sprite" + _targetAnim);
        //Check if previous state is the same, else we wanna flush out the previous states data
        if(currAnim != _targetAnim && currAnim != null)
        {
            currFrameNum = 0;
            currFrameProg = 0;
            currLoopCount = 0;
        }

        currAnim = _targetAnim;
    }
    //Abstract overload to allow us to quickly load sprites if need be
    public abstract void spriteLoad(int _characterState);
    public abstract void initializeDictionary(GameObject _gameObject, scr_BaseEntity_Main _currEntityScript, string _objectType);
    public void SpriteController(float _entitySpeedMult)
    {
        bool runningFramesAnim = true; //Means this is currently running our frame functions, we set it to false if it ran the current one correctly, true if we are switching or its still going for whatever reason
        while(runningFramesAnim)
            runningFramesAnim = currAnim.animationScript(objRenderer, this, ref currFrameProg, ref currFrameNum, ref currLoopCount, _entitySpeedMult);
        Sprite currentSprite = currAnim.returnFrame(currFrameNum); 
        //If positive, flip, else if negative, flip. Otherwise, stay the same.
        objRenderer.sprite = currentSprite;
    }


}
