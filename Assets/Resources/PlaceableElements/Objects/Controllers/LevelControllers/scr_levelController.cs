using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;

using Vector2 = UnityEngine.Vector2;

public class scr_levelController : scr_BaseObject
{
    public static IDScript levelControllerIDScript;

    public float gameSpeed = 1.0f;
    public static event GLOBAL_VARS.levelPrimer levelStartPrimedSignal;
    public static event GLOBAL_VARS.slowGameSignal gameSlowSignal;
    public GameObject itemPool;
    public int levelState;
    public scr_entityController entityController;
    public scr_cameraController cameraController;
    public int totalPrimersLeft = 0;
    public void Awake()
    {   
        entityController = gameObject.AddComponent<scr_entityController>();
        totalPrimersLeft++;
        scr_entityController.entitiesPrimedSignal += finishedPrimingChecker;

        cameraController = gameObject.AddComponent<scr_cameraController>();
        totalPrimersLeft++;
        scr_cameraController.camControllerPrimedSignal += finishedPrimingChecker;

        itemPool = GameObject.Find("itemPool");
        if(levelControllerIDScript == null)
        {
            levelControllerIDScript = new IDScript();
            levelControllerIDScript.ObjectType = GLOBAL_VARS.ObjectType.isController;
            levelControllerIDScript.ControllerType = GLOBAL_VARS.ControllerType.levelController;
        }
        objectIDScript = levelControllerIDScript;

        cameraController.cameraControllerAwake(this);
        entityController.entityControllerAwake(this);
    }

    void Start()
    {
        levelState = GLOBAL_VARS.LevelStates.levelLoading;
        levelStartPrimedSignal?.Invoke(levelControllerIDScript.ControllerType);
    }

    //Menu Controller
    void Update()
    {
        if(levelState == GLOBAL_VARS.LevelStates.levelLoading)
        {
            //Remember totalPrimersLeft has already been set in awake as more components are loaded, so this'll only run once
            if(totalPrimersLeft <= 0)
            {   
                //We want to load all of our controller starts now. 
                entityController.entityControllerStart();
                levelState = GLOBAL_VARS.LevelStates.levelFinalLoad;
            }
        }
        else if(levelState == GLOBAL_VARS.LevelStates.levelFinalLoad)
        {
            levelState = GLOBAL_VARS.LevelStates.levelRunning;
        }
        else if(levelState == GLOBAL_VARS.LevelStates.levelRunning)
        {
            entityController.entityControllerUpdate();
        }
    }
    void FixedUpdate()
    {
        if(levelState == GLOBAL_VARS.LevelStates.levelRunning)
        {
            entityController.entityControllerFixedUpdate();
        }
    }

    bool loadEntityComplete = false;
    void LateUpdate()
    {
        if(levelState == GLOBAL_VARS.LevelStates.levelRunning)
        {
            cameraController.cameraControllerLateUpdate();
        }
    }

    //Means gameover/character died
    void runLoss()
    {
        //End Scene, by running transition AFTER sprite death runs. 
        //Pull up menu
    }

    void finishedPrimingChecker(int currentPrimer)
    {
        switch(currentPrimer)
        {
            case GLOBAL_VARS.ControllerType.entityController:
            case GLOBAL_VARS.ControllerType.camController:
                totalPrimersLeft--;
                break;
            default:
                Debug.LogError("This controller was not set up in the level controllers priming checker");
                break;
        }
    }

}
