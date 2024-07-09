using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public abstract class scr_animations : MonoBehaviour {
    //These two string variables are used to gather the frames from the directory theyre stored in
    protected String assetPathName;
    protected String spriteName;
    
    //Total num of frames, calculated by our regex function
    protected int numOfFrames;

    //In the order of the frames provided, this provides the time, in a float, of how long that frame will last 
    [SerializeField] protected Dictionary<int, float> frameData = new Dictionary<int, float>();

    //Stores the frames to reference within their animation script
    [SerializeField] protected Sprite[] frameArray = null;

    //If the animation does not loop, or will not loop at this point, then it will go into this escape animation instead
    [SerializeField] protected scr_animations escapeAnimation;

    void Start()
    {   
        loadPath();
        loadFrames(spriteName);
    }
    void loadPath()
    {
        //All it does is setup the basic directory that this sprite will be found in
        String objectName = gameObject.name;
        
        IDScript currObjIdScript = gameObject.GetComponent<IDScript>();
        String spriteType;
        switch(currObjIdScript.objectType)
        {
            case GLOBAL_CONSTANTS.objectType.isPlayer:
                spriteType = "Player";
                break;
            case GLOBAL_CONSTANTS.objectType.isEnemy:
                spriteType = "Enemy";
                break;
            default:
                spriteType = "";
                break;
        }
        
        assetPathName = Application.dataPath + "/Resources/Sprites/" + spriteType + "/"  + objectName;
    }
    [SerializeField] private List<String> storedFilePaths;
    [SerializeField] private Regex rgxAppDatPath = new Regex("^(" + Application.dataPath + "/Resources/)");
    [SerializeField] private Regex rgxAsset = new Regex("(.asset)$");
    public void loadFrames(String animationName)
    {
        //Creates a path to count all the data paths. 
        assetPathName += ("/" + animationName + "/");
        storedFilePaths = Directory.GetFiles(@assetPathName, "*.asset").ToList<String>();

        numOfFrames = storedFilePaths.Count; //accounts for indexes
        
        //Sort alphabetically/numerically, meaning each frame will be in the right order naturally, no real need to make our own sort program for this
        storedFilePaths.Sort();
        frameArray = new Sprite[numOfFrames];
        
        for (int i = 0; i < numOfFrames; i++)
        {
            //Use Reg Expression to trim off everyting unneeded for Resource.Load()
            String storedString = rgxAppDatPath.Replace(storedFilePaths.ElementAt(i), "", 1);
            storedString = rgxAsset.Replace(storedString, "", 1);
            //Debug.Log(storedString);

            frameArray[i] = (Sprite)Resources.Load(storedString);
            
            //Debug.Log(frameArray[i]);
        }           
    }

    //The logic behind each animation, this can pick and choose functions below to customize them almost modularly.
    public abstract Sprite animationScript(SpriteRenderer _currSpriteRender, scr_animController _currAnimController, ref float currFrameProg, ref int currFrameNum, ref int currLoopCount);

    //Since different animations may need different specific values, this is will be used to initialize a templates main entity script, allowing us to access those needed values 
    public abstract void convertToSpecificScript(scr_Basic_Entity _currEntityScript);

    //Automatically flips sprite based on the direction the entity is looking at
    public static void autoFlip(SpriteRenderer _currSpriteRenderer, scr_Basic_Entity _currEntityScript)
    {
        _currSpriteRenderer.flipX = (_currEntityScript.getVelocity().x > 0) ? false : (_currEntityScript.getVelocity().x < 0) ? true : _currSpriteRenderer.flipX;
    }

    //Universal function used to progress animations and allow for loops if need be. 
    //Isn't always necessary, especially for animations that have only one frame, or has its own specific way of functioning
    public void progressAnim(scr_animController _currAnimController,ref float currFrameProg, ref int currFrameNum, ref int currLoopCount, float _currFrameLength = -1, int loopStartFrame = -1, int loopEndFrame = -1, bool partWillLoop = false)
    {   
        if(_currFrameLength == -1)
        {
            throw new Exception("Big currFrameLength not provided, impossible to progress animation");
        }
        currFrameProg += 1 * Time.deltaTime; //1 times delta time to get the amount of frames per second. 
        if(currFrameProg >= _currFrameLength)
        {
            currFrameProg = 0;
            ++currFrameNum;

            if(currFrameNum > loopEndFrame && partWillLoop) //Check if the currFrameNum has surpassed the current loop threshold, and if it does, check if it desires to loop
                currFrameNum = loopStartFrame;
            else if(currFrameNum == numOfFrames) //In the case where it doesn't want to loop (as the loopEndFrame should match the final frame, check if it has hit the true total frames to then go into its escape)
                _currAnimController.spriteLoad(escapeAnimation);
            //In the case where it is not the end frame, itll just progress the animation as normal, hence no else function
        }
    }
    
    //Since this'll be used in almost every walking/running/sprinting animation, we'll use this to edit frameLength speeds
    //This is based on the speed of a character
    public static float speedUpAnim(float growingFactor, float majorFactor)
    {
        //GrowingFactor can be anything that it attempting to grow to the MajorFactor.
        return (majorFactor - growingFactor) / majorFactor; 
    }

}
