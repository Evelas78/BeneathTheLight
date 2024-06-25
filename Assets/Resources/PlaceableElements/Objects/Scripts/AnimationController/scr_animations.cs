using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public abstract class scr_animations : MonoBehaviour {
    protected String assetPathName;
    protected String spriteName;
    protected int numOfFrames;
    public int currFrameMax;
    [SerializeField] protected float currFrameProg = 0;
    [SerializeField] protected int currFrameNum = 0;
    [SerializeField] protected Boolean isCompleteLoop = false;
    [SerializeField] protected scr_animations escapeAnimation;
    [SerializeField] protected Sprite[] frameArray = null;
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
            Debug.Log(storedString);

            frameArray[i] = (Sprite)Resources.Load(storedString);
            
            Debug.Log(frameArray[i]);
        }           
    }
    //Flush function so animations ALWAYS start at the beginning frame
    public void Flush()
    {
        currFrameNum = 0;
        currFrameProg = 0f;
    }
    public abstract Sprite animationScript(SpriteRenderer _currSpriteRender, scr_Basic_Entity _currEntityScript, scr_animController _currAnimController);
    public static void autoFlip(SpriteRenderer _currSpriteRenderer, scr_Basic_Entity _currEntityScript)
    {
        _currSpriteRenderer.flipX = (_currEntityScript.getVelocity().x > 0) ? false : (_currEntityScript.getVelocity().x < 0) ? true : _currSpriteRenderer.flipX;
    }
    public void progressAnim(scr_animController _currAnimController, float _currFrameLength, int loopStartFrame, int loopEndFrame, bool partWillLoop)
    {        
        currFrameProg += 1 * Time.deltaTime;
        if(currFrameProg >= _currFrameLength)
        {
            currFrameProg = 0;
            
            if(++currFrameNum > loopEndFrame && partWillLoop)
                currFrameNum = loopStartFrame;
            else
                _currAnimController.spriteLoad(escapeAnimation);
        }
    }
}
