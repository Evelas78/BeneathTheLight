using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

using Vector2 = UnityEngine.Vector2;

public class scr_cameraController : scr_BaseObject
{
    public scr_levelController levelController;
    public Camera currCamera;
    public GameObject currCamTarget;
    
    [SerializeField] private GameObject camSpotTemplate;
    [SerializeField] private static GameObject itemPoolObj;
    [SerializeField] private int totalCamSpotReserves = 7, camSpotIncreaseNum = 5; //This will update when needed
    [SerializeField] private List<GameObject> camSpotStorage = new List<GameObject>();
    //The cam spot the game follows, this object exists to prevent errors when destroying objects that may be the target
    public static GameObject followCamSpot;
    public Vector2 orthographicHalf = new Vector2();

    //ID SCRIPT
    public static IDScript camControllerIDScript;

    //Just tell the levelPrimer game is setup
    public static event GLOBAL_VARS.levelPrimer camControllerPrimedSignal;

    //*************************************************************************************
    //MONOBEHAVIOUR FUNCTIONS 
    public void cameraControllerAwake(scr_levelController _LevelController)
    {
        levelController = _LevelController;

        if(camControllerIDScript == null)
        {
            camControllerIDScript = new IDScript();
            camControllerIDScript.ObjectType = GLOBAL_VARS.ObjectType.isController;
            camControllerIDScript.ControllerType = GLOBAL_VARS.ControllerType.levelController;
        }

        objectIDScript = camControllerIDScript;

 
        scr_entityController.camChangeSignal += camChangeTarget;
        scr_levelController.levelStartPrimedSignal += onLevelTrigger;
    }
    //Late update so camera doesnt jitter with the character animations/movement
    public void cameraControllerLateUpdate()
    {
        if(currCamTarget != null)
            followCamSpot.transform.position = currCamTarget.transform.position;
        currCamera.transform.position = new UnityEngine.Vector3(followCamSpot.transform.position.x, followCamSpot.transform.position.y, currCamera.transform.position.z);
    }    


    
    //*************************************************************************************
    //Regular Functions
    void camChangeTarget(GameObject _objectTarget, bool _isActivation)
    {
        if(_isActivation)
            currCamTarget = _objectTarget; 
        else
        {
            if(currCamTarget == _objectTarget)
                currCamTarget = null;
        }
        //It will maintain the cam of the followSpot
    }
    void clearCamSpots()
    {
        for(int i = 0; i < totalCamSpotReserves; i++)
        {
            GameObject currCamSpot = camSpotStorage.ElementAt(i);
            camSpotStorage.RemoveAt(0);
            Destroy(currCamSpot);
        }
        Destroy(followCamSpot);
        followCamSpot = null;
    }

    //*************************************************************************************
    //PRIMING COROUTINE WORK

    void onLevelTrigger(int __unused__)
    {   
        Debug.Log("CAM Controller Priming Coroutine Started");
        StartCoroutine(primingCoroutine());
    }
    //Priming Coroutine aims to ready every entity (using their awake) and make sure its in the list
    IEnumerator primingCoroutine()
    {
        currCamera = gameObject.GetComponent<Camera>();
        yield return false;
        
        //Necessity for orthographic calculations
        float orthoHalf_Vert = currCamera.orthographicSize;
        float orthoHalf_Horiz = orthoHalf_Vert * currCamera.pixelWidth / currCamera.pixelHeight;

        orthographicHalf.y = orthoHalf_Vert;
        orthographicHalf.x = orthoHalf_Horiz;

        GameObject currentlyStoredCamSpot;
        
        //Create enough cam spots for the reserves
        for(int i = 0; i < totalCamSpotReserves; i++)
        {
            currentlyStoredCamSpot = Instantiate(camSpotTemplate, levelController.itemPool.transform);
            camSpotStorage.Add(currentlyStoredCamSpot);
            yield return false;
        }
        
        //Set spot to be the gameObject's current spot, so we drag it to wherever we want the screen to default to in any situatioon
        followCamSpot = Instantiate(camSpotTemplate, gameObject.transform);

        camControllerPrimedSignal?.Invoke(GLOBAL_VARS.ControllerType.camController);

        yield return new WaitForSecondsRealtime(3);

        Debug.Log("Cam Controller done priming");
        yield return true;
    }

    //*************************************************************************************
    //Accessors/Mutators Below
    public GameObject getCamTarget() { return currCamTarget; }

}