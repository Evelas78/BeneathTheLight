using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public abstract class scr_animations{
    //These two string variables are used to gather the frames from the directory theyre stored in
    protected string assetPathName;
    //Total num of frames, calculated by our regex function
    protected int numOfFrames;

    //Stores the frames to reference within their animation script
    [SerializeField] protected Sprite[] frameArray = null;

    //If the animation does not loop, or will not loop at this point, then it will go into this escape animation instead
    [SerializeField] protected int escapeAnimationNum = GLOBAL_VARS.CharacterStates.Idle;
    
    //currGameObject for target
    public scr_animations(string _gameObjectName, string _objectType)
    {   
        loadPath(_gameObjectName, _objectType);
    }
    void loadPath(string _gameObjectName, string _objectType)
    {
        //All it does is setup the basic directory that this sprite will be found in
        string objectName = _gameObjectName;
        string objectType = _objectType;
        
        assetPathName = Application.dataPath + "/Resources/Sprites/" + objectType + "/"  + objectName;
        //Debug.Log(assetPathName);
    }
    [SerializeField] private List<string> storedFilePaths;
    [SerializeField] private Regex rgxAppDatPath = new Regex("^(" + Application.dataPath + "/Resources/)");
    [SerializeField] private Regex rgxAsset = new Regex("(.asset)$");
    public void loadFrames(string animationName)
    {
        //Creates a path to count all the data paths. 
        assetPathName += ("/" + animationName + "/");
        storedFilePaths = Directory.GetFiles(@assetPathName, "*.asset").ToList<string>();

        numOfFrames = storedFilePaths.Count; //accounts for indexes
        Debug.Log(numOfFrames + " " + assetPathName);
        //Sort alphabetically/numerically, meaning each frame will be in the right order naturally, no real need to make our own sort program for this
        storedFilePaths.Sort();
        frameArray = new Sprite[numOfFrames];
        
        for (int i = 0; i < numOfFrames; i++)
        {
            //Use Reg Expression to trim off everyting unneeded for Resource.Load()
            string storedstring = rgxAppDatPath.Replace(storedFilePaths.ElementAt(i), "", 1);
            storedstring = rgxAsset.Replace(storedstring, "", 1);
            //Debug.Log(storedstring);

            //When loading prepThrow specifically, the asset wouldnt be read as a sprite for whatever reason and there wasnt any difference with other sprite loads, so we use "as" operator instead of typecasting unfortunately.
            //Might end up more prone to mistakes than typecasting but I dont believe I found a worthwhile workaround
            frameArray[i] = Resources.Load(storedstring) as Sprite;
            
            //Debug.Log(frameArray[i]);
        }           
    }

    //The logic behind each animation, this can pick and choose functions below to customize them almost modularly.
    public abstract bool animationScript(SpriteRenderer _currSpriteRender, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _entitySpeedMult);
    public Sprite returnFrame(int currFrameNum)
    {
        return frameArray[currFrameNum];
    }
    //Since different animations may need different specific values, this is will be used to initialize a templates main entity script, allowing us to access those needed values 
    public abstract void convertToSpecificScript(scr_BaseEntity_Main _currEntityScript);

    //Automatically flips sprite based on the direction the entity is looking at
    public static void autoFlip(SpriteRenderer _currSpriteRenderer, scr_BaseEntity_Main _currEntityScript)
    {
        _currSpriteRenderer.flipX = (_currEntityScript.getVelocity().x > 0) ? false : (_currEntityScript.getVelocity().x < 0) ? true : _currSpriteRenderer.flipX;
    }

    //Universal function used to progress animations and allow for loops if need be. 
    //Isn't always necessary, especially for animations that have only one frame, or has its own specific way of functioning
    //currAnim is passed to debug
    public static bool progressAnim(scr_animations currAnim, scr_animController _currAnimController, int _numOfFrames, int _escapeAnimNum, float _entitySpeedMult, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _currFrameLength = -1, int loopStartFrame = -1, int loopEndFrame = -1, bool partWillLoop = false)
    {   
        if(_currFrameLength == -1)
        {
            throw new Exception("Big currFrameLength not provided, impossible to progress animation");
        }
        currFrameProg += 1 * Time.deltaTime * _entitySpeedMult; //1 times delta time to get the amount of frames per second. 
        if(currFrameProg >= _currFrameLength)
        {
            currFrameProg = 0;
            ++currFrameNum;

            if(currFrameNum > loopEndFrame && partWillLoop) //Check if the currFrameNum has surpassed the current loop threshold, and if it does, check if it desires to loop
            {
                Debug.Log("Starting Loop");
                currFrameNum = loopStartFrame;
                return false;
            }
            else if(currFrameNum == _numOfFrames) //In the case where it doesn't want to loop (as the loopEndFrame should match the final frame, check if it has hit the true total frames to then go into its escape)
            {
                Debug.Log("Running escape anim " + currAnim + " totFrames: " + _numOfFrames + " esc num: " +  _escapeAnimNum);
                _currAnimController.spriteLoad(_escapeAnimNum);
                //New animation loaded, we finish it off by running this, meaning we gotta start running that one this frame
                return true;
            }
            //In the case where it is not the end frame, itll just progress the animation as normal, hence no else function
        }
        return false;
    }
    
    //Since this'll be used in almost every walking/running/sprinting animation, we'll use this to edit frameLength speeds
    //This is based on the speed of a character
    public static float speedUpAnim(float growingFactor, float majorFactor)
    {
        //GrowingFactor can be anything that it attempting to grow to the MajorFactor.
        return (majorFactor - growingFactor) / majorFactor; 
    }

}
