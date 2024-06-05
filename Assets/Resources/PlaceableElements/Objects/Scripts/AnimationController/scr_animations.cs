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
    public abstract Sprite animationScript();
}
