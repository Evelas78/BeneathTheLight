using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

using Vector2 = UnityEngine.Vector2;

public class scr_cameraController : scr_BaseObject
{
    public scr_gameController gameController;
    public Camera currCamera;
    public GameObject currCamTarget;
    
    [SerializeField] private GameObject camSpotTemplate;
    [SerializeField] private static GameObject itemPoolObj;
    [SerializeField] private int totalCamSpots = 0; //This will update when needed
    [SerializeField] private List<GameObject> camSpotStorage = new List<GameObject>();
    //The cam spot the game follows, this object exists to prevent errors when destroying objects that may be the target
    public GameObject mainCamSpot;
    
    //This is the gameobject we would want to default to. If it's null, it means just maintain position for the mainCamSpot
    //Else, we change mainCamSpot to the reboundTarget in any case that we shouldn't hold positio
    public GameObject reboundTarget;
    public Vector2 orthographicHalf = new Vector2();

    //Just tell the levelPrimer game is setup
    public static event GLOBAL_VARS.controllerResponseSignal initialGamePrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal levelPrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal levelResetSignal;
    public static event GLOBAL_VARS.controllerResponseSignal menuPrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal menuResetSignal;

    //*************************************************************************************
    //MONOBEHAVIOUR FUNCTIONS 
    public void InstantiateCameraController(scr_gameController _GameController)
    {
        gameController = _GameController;

        scr_entityController.camChangeSignal += CamChangeTarget;
        scr_gameController.gameInitialPrimeSignal += onInitialGamePrime;
        scr_gameController.levelLoadSignal += onLevelLoad;
    }
    //Late update so camera doesnt jitter with the character animations/movement
    public void CameraControllerLateUpdate()
    {
        if(currCamTarget != null && onTarget)
            mainCamSpot.transform.position = currCamTarget.transform.position;
        else if(currCamTarget != null)
            camMoveTarget();
        currCamera.transform.position = new UnityEngine.Vector3(mainCamSpot.transform.position.x, mainCamSpot.transform.position.y, currCamera.transform.position.z);
    }    

    //*************************************************************************************
    //Regular Functions
    int lerpType;
    float timeToReach;
    float currLerpCounter;
    bool onTarget = true;
    void camMoveTarget()
    {
        currLerpCounter += Time.deltaTime;

        //Base Case, 0 is timeToReach, meaning we just want to instantly land on that object
        if(timeToReach == 0)
        {
            onTarget = true;
            mainCamSpot.transform.position = currCamTarget.transform.position;
        }

        switch(lerpType)
        {
            case GLOBAL_VARS.LerpType.linear:
                mainCamSpot.transform.position = new Vector3(
                GLOBAL_VARS.linearLerp(mainCamSpot.transform.position.x, currCamTarget.transform.position.x, currLerpCounter, timeToReach),
                GLOBAL_VARS.linearLerp(mainCamSpot.transform.position.y, currCamTarget.transform.position.y, currLerpCounter, timeToReach),
                GLOBAL_VARS.linearLerp(mainCamSpot.transform.position.z, currCamTarget.transform.position.z, currLerpCounter, timeToReach));
            break;
            case GLOBAL_VARS.LerpType.decay:
                mainCamSpot.transform.position = new Vector3(
                GLOBAL_VARS.decayLerp(mainCamSpot.transform.position.x, currCamTarget.transform.position.x, currLerpCounter, timeToReach),
                GLOBAL_VARS.decayLerp(mainCamSpot.transform.position.y, currCamTarget.transform.position.y, currLerpCounter, timeToReach),
                GLOBAL_VARS.decayLerp(mainCamSpot.transform.position.z, currCamTarget.transform.position.z, currLerpCounter, timeToReach));
            break;
            case GLOBAL_VARS.LerpType.exponential:
                mainCamSpot.transform.position = new Vector3(
                GLOBAL_VARS.expoLerp(mainCamSpot.transform.position.x, currCamTarget.transform.position.x, currLerpCounter, timeToReach),
                GLOBAL_VARS.expoLerp(mainCamSpot.transform.position.y, currCamTarget.transform.position.y, currLerpCounter, timeToReach),
                GLOBAL_VARS.expoLerp(mainCamSpot.transform.position.z, currCamTarget.transform.position.z, currLerpCounter, timeToReach));
            break;
            case GLOBAL_VARS.LerpType.FSF:
                mainCamSpot.transform.position = new Vector3(
                GLOBAL_VARS.FSFLerp(mainCamSpot.transform.position.x, currCamTarget.transform.position.x, currLerpCounter, timeToReach),
                GLOBAL_VARS.FSFLerp(mainCamSpot.transform.position.y, currCamTarget.transform.position.y, currLerpCounter, timeToReach),
                GLOBAL_VARS.FSFLerp(mainCamSpot.transform.position.z, currCamTarget.transform.position.z, currLerpCounter, timeToReach));
            break;
            case GLOBAL_VARS.LerpType.SFS:
                mainCamSpot.transform.position = new Vector3(
                GLOBAL_VARS.SFSLerp(mainCamSpot.transform.position.x, currCamTarget.transform.position.x, currLerpCounter, timeToReach),
                GLOBAL_VARS.SFSLerp(mainCamSpot.transform.position.y, currCamTarget.transform.position.y, currLerpCounter, timeToReach),
                GLOBAL_VARS.SFSLerp(mainCamSpot.transform.position.z, currCamTarget.transform.position.z, currLerpCounter, timeToReach));
            break;
            
        }
        if(mainCamSpot.transform.position == currCamTarget.transform.position)
        {
            onTarget = true;
        }
    }

    void CamChangeTarget(GameObject _objectTarget, bool _isActivation /*Means we are changing, not removing*/, int _lerpType, float _timeToReach = 0.5f)
    {
        
        if(_isActivation && _objectTarget != null)
        {
            currCamTarget = _objectTarget;
            lerpType = _lerpType;
            timeToReach = _timeToReach;
            onTarget = false;
            //Debug.Log("New Cam target, " + currCamTarget.name);
        }
        else
        {
            if(currCamTarget == _objectTarget)
            {
                if(reboundTarget != null)
                    CamChangeTarget(reboundTarget, true, GLOBAL_VARS.LerpType.linear, 0f);
                else
                    currCamTarget = null;
            }
        }
        //It will maintain the cam of the followSpot
    }
    void DeleteCamSpotsArray()
    {
        for(int i = 0; i < totalCamSpots; i++)
        {
            GameObject currCamSpot = camSpotStorage.ElementAt(i);
            camSpotStorage.RemoveAt(0);
            Destroy(currCamSpot);
        }
    }

    //*************************************************************************************
    //PRIMING COROUTINE WORK
    void readSceneInfo()
    {
        scr_sceneInfo currSceneInfo = GameObject.FindObjectOfType<scr_sceneInfo>();
        //Debug.Log(currSceneInfo + " : currSceneInfoScript");

        SoSceneInfo currSceneSO = currSceneInfo.sceneInfoSO;
        //Debug.Log(currSceneInfo + " : currSceneInfoScript SO");

        if(currSceneSO.sceneType == GLOBAL_VARS.SceneLoadType.unset)
            Debug.LogError("Current Scene sceneData Object has their SO unset");

        List<Vector3> currCamPointSpots = currSceneSO.camPoints;
        changeCamStorageSpotLocations(currCamPointSpots);

        reboundTarget = currSceneInfo.startingObject;
        CamChangeTarget(reboundTarget, true, GLOBAL_VARS.LerpType.linear, 0f);
    }
    void changeCamStorageSpotLocations(List<Vector3> _newCamPointSpots)
    {
        int currCamPointTotal = _newCamPointSpots.Count;

        while(totalCamSpots < currCamPointTotal)
        {
            Instantiate(camSpotTemplate);
            camSpotStorage.Add(camSpotTemplate);
            totalCamSpots++;
        }

        int i = 0;
        foreach(Vector3 currPos in _newCamPointSpots)
        {
            camSpotStorage.ElementAt(i).transform.position = currPos;
            i++;
        }
    }
    void onInitialGamePrime()
    {   
        Debug.Log("Cam Controller Priming Coroutine Started");
        StartCoroutine(initialGamePrimingCoroutine());
    }
    //Priming Coroutine aims to ready every entity (using their awake) and make sure its in the list
    IEnumerator initialGamePrimingCoroutine()
    {
        currCamera = gameObject.GetComponent<Camera>();
        yield return false;
        
        //Necessity for orthographic calculations
        float orthoHalf_Vert = currCamera.orthographicSize;
        float orthoHalf_Horiz = orthoHalf_Vert * currCamera.pixelWidth / currCamera.pixelHeight;

        orthographicHalf.y = orthoHalf_Vert;
        orthographicHalf.x = orthoHalf_Horiz;
        
        //Set spot to be the gameObject's current spot, so we drag it to wherever we want the screen to default to in any situatioon
        mainCamSpot = Instantiate(camSpotTemplate, gameObject.transform);
        //Debug.Log(mainCamSpot);

        yield return new WaitForSecondsRealtime(3);

        Debug.Log("Cam Controller done priming");
        initialGamePrimedSignal?.Invoke(true);
        yield return true;
    }

    //We'd want an object on the scene which contains a script of a list of all spots a camera needs to be on and objects will send out signals triggering the number
    //Might be time for a scriptable object, but this is not required for the demo for sure
    void onLevelLoad()
    {
        Debug.Log("Camera Controller level load coroutine ran");
        StartCoroutine(levelLoadCoroutine());
    }
    IEnumerator levelLoadCoroutine()
    {
        //Slightly less efficient, but like, its in a loading scene anyway, ill just take this hit instead of rewriting the entire system again
        readSceneInfo();
        yield return true;

        Debug.Log("Camera Controller level load coroutine finished");
        levelPrimedSignal?.Invoke(true);
        yield return true;
    }
    //*************************************************************************************
    //Accessors/Mutators Below
    public GameObject getCamTarget() { return currCamTarget; }

}