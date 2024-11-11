using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using System.Collections;
using TMPro.Examples;



//What we want this to do and child components to do. 
//Send and Receive signals for game transitions
//Manage buttons and menus, allowing us to choose levels or change settings
//Store data to save
//Handle Music/Sounds
//Handle Scene Loading
public class scr_gameController : MonoBehaviour
{
    //scr_transitionController transitionController;
    public scr_soundController soundController;
    public scr_menuController menuController;
    public scr_cameraController camController;
    public scr_entityController entityController;
    public scr_transitionController transitionController;
    public Animator transAnimator;
    public static event GLOBAL_VARS.gcBeginSignal gameInitialPrimeSignal;

    public static event GLOBAL_VARS.gcBeginSignal levelStartSignal;
    public static event GLOBAL_VARS.gcBeginSignal menuStartSignal;

    public static event GLOBAL_VARS.levelPause pauseSignal;
    public static event GLOBAL_VARS.gcTransitionSignals transitionStartSignal;

    public static event GLOBAL_VARS.sceneLoad menuLoadSignal;
    public static event GLOBAL_VARS.sceneLoad menuDeLoadSignal;
    public static event GLOBAL_VARS.sceneLoad levelLoadSignal;
    public static event GLOBAL_VARS.sceneLoad levelDeLoadSignal;

    public int gameState;
    public int currSceneType;
    private float gameSpeedMultiplier = 1.0f;
    public GameObject itemPool;
    //Theoretically, this is the absolute FIRST thing to run in the game. So all set up and saving must be done either here or by a child component
    void Awake()
    {
        //First things first, the game controller must NEVER be destroyed. 
        DontDestroyOnLoad(gameObject); //This only runs once

        //**************************************************************

        soundController = gameObject.AddComponent<scr_soundController>();
        soundController.InstantiateMusicController(this);

        initialTotalPrimers++;
        scr_soundController.initialGamePrimedSignal += InitialGamePrimeChecking;
        //**************************************************************

        menuController = gameObject.AddComponent<scr_menuController>();
        menuController.InstantiateMenuController(this);
        scr_menuController.transitionSceneConfirmationSignal += PrimingTransitions;

        initialTotalPrimers++;
        scr_menuController.initialGamePrimedSignal += InitialGamePrimeChecking; //Made
        menuTotalPrimers++;
        scr_menuController.menuPrimedSignal += MenuPrimeChecking;
        levelTotalPrimers++;
        scr_menuController.levelPrimedSignal += LevelPrimeChecking;

        //**************************************************************

        transitionController = gameObject.AddComponent<scr_transitionController>();
        transitionController.InstantiateTransitionController(this);
        scr_transitionController.transitionCompletedSignal += receiveTransitionCompletedSignal;

        initialTotalPrimers++;
        scr_transitionController.transitionControllerPrimedSignal += InitialGamePrimeChecking;


        //**************************************************************

        camController = gameObject.AddComponent<scr_cameraController>();
        camController.InstantiateCameraController(this);

        initialTotalPrimers++;
        scr_cameraController.initialGamePrimedSignal += InitialGamePrimeChecking;
        
        //Create cam spots, set position to main character
        levelTotalPrimers++;
        scr_cameraController.levelPrimedSignal += LevelPrimeChecking;
        //Delete all camspots, set position to origin position
        levelTotalResetters++;
        scr_cameraController.levelResetSignal += LevelResetChecking;


        //**************************************************************

        //we also want to setup the entity controller
        entityController = gameObject.AddComponent<scr_entityController>();
        entityController.InstantiateEntityController(this);

        levelTotalPrimers++;
        scr_entityController.levelPrimedSignal += LevelPrimeChecking;
        levelTotalResetters++;
        scr_entityController.levelDeLoadSignal += LevelResetChecking;
        

        Debug.Log(
            "IPrimes=" + initialTotalPrimers + "\n" +
            "LPrimes=" + levelTotalPrimers + "\n" +
            "MPrimes=" + menuTotalPrimers + "\n" +
            "MResetters=" + menuTotalResetters + "\n" +
            "LResetters=" + levelTotalResetters + "\n"
        );

        //**************************************************************
        //**************************************************************
        //**************************************************************

        gameState = GLOBAL_VARS.GameStates.gamePreparing;
        gameInitialPrimeSignal?.Invoke();
    }
    bool debug_entityStartRan = false;
    bool triggeredStartUp = false, startUpEnded = false;
    void Update()
    {
        if(gameState != GLOBAL_VARS.GameStates.gamePreparing)
        {
            menuController.menuControllerUpdate(gameSpeedMultiplier);
        }
        transitionController.transitionControllerUpdate(gameSpeedMultiplier);

        if(gameState == GLOBAL_VARS.GameStates.levelState)
        {
            if(!debug_entityStartRan)
            {
                entityController.EntityControllerStart();
                debug_entityStartRan = true;
            }
            Debug.Log("Running Entity Controller Update");
            entityController.EntityControllerUpdate(gameSpeedMultiplier);
        }
        changeSceneWorker();
    }
    void FixedUpdate()
    {
        if(gameState == GLOBAL_VARS.GameStates.levelState)
        {
            Debug.Log("We are running Entity Controller FUpdate");
            entityController.EntityControllerFixedUpdate(gameSpeedMultiplier);
        }
    }

    void LateUpdate()
    {
        camController.CameraControllerLateUpdate();
    }

    [SerializeField] private int startTransType, endTransType;
    [SerializeField] private int startLerpType, endLerpType;
    [SerializeField] private float startTransTime, endTransTime;
    [SerializeField] private string targetRoom;
    [SerializeField] private int targetRoomType = GLOBAL_VARS.SceneLoadType.unset;
    void PrimingTransitions(int _startTransType, int _endTransType, int _startLerpType, int _endLerpType, float _startTransTime, float _endTransTime, string _targetRoom, bool _isLevel)
    {
        if(_isLevel)
            targetRoomType = GLOBAL_VARS.SceneLoadType.level;
        else
            targetRoomType = GLOBAL_VARS.SceneLoadType.menu;

        startTransType = _startTransType;
        endTransType = _endTransType;
        
        startLerpType = _startLerpType;
        endLerpType = _endTransType;

        startTransTime = _startTransTime;
        endTransTime = _endTransTime;

        targetRoom = _targetRoom;
    }

    int transitionsCompleted = 0, transitionsStarted = 0, loadSignalsSent = 0;
    bool sceneChangedStarted = false, sceneChangedFinished = false;
    void changeSceneWorker()
    {
        if(targetRoomType == GLOBAL_VARS.SceneLoadType.menu)
        {
            if(gameState == GLOBAL_VARS.GameStates.menuState)
            {
                deloadMenuScene(startTransType, startLerpType, startTransTime);
            }
            else if(gameState == GLOBAL_VARS.GameStates.levelState)
            {
                deloadLevelScene(startTransType, startLerpType, startTransTime);
            }
            else if(gameState == GLOBAL_VARS.GameStates.sceneLeaving || gameState == GLOBAL_VARS.GameStates.sceneEntering) //we working on this logic now
            { 
                if(!sceneChangedStarted)
                {
                    //What this means is we are actually moving scenes in this case
                    StartCoroutine(loadSceneAsyncCoroutine());
                    sceneChangedStarted = true;
                }
                if(sceneChangedFinished)
                {
                    loadMenuScene(endTransType, endLerpType, endTransTime);
                }
            }
        }
        else if(targetRoomType == GLOBAL_VARS.SceneLoadType.level)
        {
            if(gameState == GLOBAL_VARS.GameStates.menuState)
            {
                deloadMenuScene(startTransType, startLerpType, startTransTime);
            }
            else if(gameState == GLOBAL_VARS.GameStates.levelState)
            {
                deloadLevelScene(startTransType, startLerpType, startTransTime);
            }
            else if(gameState == GLOBAL_VARS.GameStates.sceneLeaving || gameState == GLOBAL_VARS.GameStates.sceneEntering)
            { 
                if(!sceneChangedStarted)
                {
                    //What this means is we are actually moving scenes in this case
                    StartCoroutine(loadSceneAsyncCoroutine());
                    sceneChangedStarted = true;
                }
                if(sceneChangedFinished)
                {
                    loadLevelScene(endTransType, endLerpType, endTransTime);
                }
            }
        }
    }

    IEnumerator loadSceneAsyncCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetRoom);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    
        sceneChangedFinished = true;
        //Debug.Log("Scene has Changed");
    }
    void deloadMenuScene(int _transitionType, int _lerpType, float _transitionTime)
    {
        //First we need to finish the transition SO
        if(transitionsStarted == 0)
        {
            Debug.Log("Transition start signal sent in DeloadMenuScene");
            transitionStartSignal?.Invoke(_transitionType, _lerpType, _transitionTime, true);
            transitionsStarted++;
        }
        if(transitionsCompleted == 1 && loadSignalsSent == 0)
        {
            Debug.Log("Starting Menu Deload Functions");
            loadSignalsSent++;
            menuDeLoadSignal?.Invoke();
            MenuResetChecking(false); //We use this to check if there is even any in the first place, so itll auto run if we see there isnt
        }
    }

    void deloadLevelScene(int _transitionType, int _lerpType, float _transitionTime)
    {
        if(transitionsStarted == 0)
        {
            Debug.Log("Transition start signal sent in DeloadLevelScene");
            transitionStartSignal?.Invoke(_transitionType, _lerpType, _transitionTime, true);
            transitionsStarted++;
        }
        if(transitionsCompleted == 1 && loadSignalsSent == 0)
        {
            Debug.Log("Starting Level Deload Functions");
            loadSignalsSent++;
            levelDeLoadSignal?.Invoke();
            LevelResetChecking(false); //We use this to check if there is even any in the first place, so itll auto run if we see there isnt
        }
    }

    void loadMenuScene(int _transitionType, int _lerpType, float _transitionTime)
    {
        if(loadSignalsSent == 1)
        {
            Debug.Log("Starting Menu Load functions");
            menuLoadSignal?.Invoke();
            loadSignalsSent++;
            MenuPrimeChecking(false); //We use this to check if there is even any in the first place, so itll auto run if we see there isnt
        }
        if(gameState == GLOBAL_VARS.GameStates.sceneEntering) //This gains scene entering once all controllers are finished priming themselves.
        {
            //End phase, we clean everything up, trigger the final transition, and then once that happens, we sent out the level Start signal. 
            if(transitionsStarted == 1)
            {            
                Debug.Log("Transition start signal sent in LoadMenuScene");
                transitionsStarted++;
                transitionStartSignal?.Invoke(_transitionType, _lerpType, _transitionTime, false);
            }
            if(transitionsCompleted == 2)
            {
                //deload all data, change state to the real gameState, then itll work in the fixed update on its own
                transitionsStarted = 0;
                transitionsCompleted = 0;
                loadSignalsSent = 0;
                startTransType = -1;
                endTransType = -1;
                startTransTime = -1;
                endTransTime = -1;
                targetRoom = null;

                sceneChangedStarted = false;
                sceneChangedFinished = false;

                startUpEnded = false;
                triggeredStartUp = false;

                gameState = GLOBAL_VARS.GameStates.menuState;
                targetRoomType = GLOBAL_VARS.SceneLoadType.unset;
            }
        }

    }

    void loadLevelScene(int _transitionType, int _lerpType, float _transitionTime)
    {
        if(loadSignalsSent == 1)
        {
            Debug.Log("Starting Level Load functions");
            levelLoadSignal?.Invoke();
            loadSignalsSent++;
            LevelPrimeChecking(false); //We use this to check if there is even any in the first place, so itll auto run if we see there isnt
        }
        if(gameState == GLOBAL_VARS.GameStates.sceneEntering)
        {
            //End phase, we clean everything up, trigger the final transition, and then once that happens, we sent out the level Start signal. 
            if(transitionsStarted == 1)
            {
                Debug.Log("Transition start signal sent in LoadLevelScene");
                transitionsStarted++;
                transitionStartSignal?.Invoke(_transitionType, _lerpType, _transitionTime, false);
            }
            if(transitionsCompleted == 2)
            {
                //deload all data, change state to the real gameState, then itll work in the fixed update on its own
                transitionsStarted = 0;
                transitionsCompleted = 0;
                loadSignalsSent = 0;
                startTransType = -1;
                endTransType = -1;
                startTransTime = -1;
                endTransTime = -1;
                targetRoom = null;

                sceneChangedStarted = false;
                sceneChangedFinished = false;

                startUpEnded = false;
                triggeredStartUp = false;

                gameState = GLOBAL_VARS.GameStates.levelState;
                targetRoomType = GLOBAL_VARS.SceneLoadType.unset;
            }
        }

    }

    void receiveTransitionCompletedSignal()
    {
        // all we want to do is just add one, so it knows.
        transitionsCompleted++;
        Debug.Log("Received transition Signal completed (Gamecontroller)");
    }

    //Primer Checkers Section
    [SerializeField] int initialPrimersFinished = 0, initialTotalPrimers = 0;
    void InitialGamePrimeChecking(bool _successful)
    {
        if(_successful)
            initialPrimersFinished++;
        else {} //save this as an abort option, allows us to put false to run it withougt adding to the primer check
        if(initialPrimersFinished == initialTotalPrimers)
        {
            initialPrimersFinished = 0;
            gameState = GLOBAL_VARS.GameStates.menuState;
        }
    }

    [SerializeField] int menuPrimersFinished = 0, menuTotalPrimers = 0;
    void MenuPrimeChecking(bool _successful)
    {
        if(_successful)
            menuPrimersFinished++;
        else   {} //save this as an abort option
        if(menuPrimersFinished == menuTotalPrimers)
        {
            menuPrimersFinished = 0;
            gameState = GLOBAL_VARS.GameStates.sceneEntering;
        }
    }

    [SerializeField] int menuResettersFinished = 0, menuTotalResetters = 0;
    void MenuResetChecking(bool _successful)
    {
        if(_successful)
            menuResettersFinished++;
        else {} //save this as an abort option
        if(menuResettersFinished == menuTotalResetters)
        {
            menuResettersFinished = 0;
            gameState = GLOBAL_VARS.GameStates.sceneLeaving;
        }
    }

    [SerializeField] int levelPrimersFinished = 0, levelTotalPrimers = 0;
    void LevelPrimeChecking(bool _successful)
    {
        if(_successful)
            levelPrimersFinished++;
        else {} //save this as an abort option
        if(levelPrimersFinished == levelTotalPrimers)
        {
            levelPrimersFinished = 0;
            gameState = GLOBAL_VARS.GameStates.sceneEntering;
        }
    }

    [SerializeField] int levelResettersFinished = 0, levelTotalResetters = 0;
    void LevelResetChecking(bool _successful)
    {
        if(_successful)
            levelResettersFinished++;
        else {} //save this as an abort option
        if(levelResettersFinished == levelTotalResetters)
        {
            levelResettersFinished = 0;
            gameState = GLOBAL_VARS.GameStates.sceneLeaving;
        }
    }
}