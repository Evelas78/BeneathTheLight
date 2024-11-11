using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

using Vector2 = UnityEngine.Vector2;

public class scr_entityController : scr_BaseObject
{
    public scr_BaseEntity_Main currMainCharScr;
    //We use this to run every entities array
    public List<scr_BaseEntity_Main> entityArray;
    public scr_gameController gameController;
    public static event GLOBAL_VARS.camChangeSignal camChangeSignal;    
    public static event GLOBAL_VARS.controllerResponseSignal levelPrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal levelDeLoadSignal;

    //Previous speed to store and use for lerping back and forth
    [SerializeField] private float entityControllerSpeed = 1.0f, previousSpeed = 1.0f, changeToSpeed = 1.0f, lerpFactor = 0.0f;
    
    //Awake is always ALWAYS just setup. if itll affect something else in the scene, then it should not run in it.
    public void InstantiateEntityController(scr_gameController _gameController)
    {
        gameController = _gameController;

        //On level prime, this will take effect
        scr_gameController.levelLoadSignal += OnLevelLoadSignal;
        scr_gameController.levelDeLoadSignal += OnLevelDeLoadSignal;

        scr_BaseEntity_Main.entitySlowSignal += OnSlowTrigger;
        scr_BaseEntity_Main.entityActivatedSignal += OnEntityActivation;
        scr_BaseEntity_Main.entityDeactivatedSignal += OnEntityDeactivation;
    }
    public void EntityControllerStart()
    {
        if(currMainCharScr != null)
            currMainCharScr.entityStart();
        for(int i = 0; i < entityArray.Count; i++)
            entityArray[i].entityStart();
    }
    public void EntityControllerUpdate(float _gameSpeedMult)
    {
        float entitySpeed = entityControllerSpeed * _gameSpeedMult;
        if(currMainCharScr != null)
            currMainCharScr.entityUpdate(entitySpeed);
        for(int i = 0; i < entityArray.Count; i++)
            entityArray[i].entityUpdate(entitySpeed);
    }
    public void EntityControllerFixedUpdate(float _gameSpeedMult) 
    {
        //Debug.Log("Running Fixed Update EC");
        float entitySpeed = entityControllerSpeed * _gameSpeedMult; 
        //We check if our changeSpeed is different from entitySpeed then run it if so
        //previous speed will update on its own so its nothing we need to check for
        if(changeToSpeed != entitySpeed)
        {
            ChangeSpeedFixedUpdate();
        }

        if(currMainCharScr != null)
        {
            currMainCharScr.entityFixedUpdate(entitySpeed);
        }
        for(int i = 0; i < entityArray.Count; i++)
            entityArray[i].entityFixedUpdate(entitySpeed);
    
    }

    public float lerpCounter;
    public const float lerpTotal = 1.0f, lerpBase = MathF.E;
    //Smaller lerp factor means slower growth
    void OnSlowTrigger(float _changeToSpeed, float _lerpFactor)
    {   
        //Previous speed now equals are previous target speed (initially equalling one)
        previousSpeed = changeToSpeed;
        changeToSpeed = _changeToSpeed;
        //Lerp factor will always change to what we aimed for before we run our fixed update
        lerpFactor = _lerpFactor;
    }    
    //Load into the entity array to add it to the update list
    void OnEntityActivation(GameObject currGameObject, bool isMainChar, bool isNewCamTarget)
    {
        currGameObject.SetActive(true);
        //Run the start function of the newly activated object
        currGameObject.GetComponent<scr_BaseEntity_Main>().entityStart();

        Debug.Log(currGameObject + " Has activated");
        if(isMainChar)
        {
            //Debug.Log("New Mainchar scr");
            currMainCharScr = currGameObject.GetComponent<scr_BaseEntity_Main>();
        }
        else
        {

        }

        if(isNewCamTarget)
        {
            camChangeSignal?.Invoke(currGameObject, true, GLOBAL_VARS.LerpType.SFS, .25f);
        }
    }
    //Unload into the entity array to remove it from the update lists
    void OnEntityDeactivation(GameObject currGameObject, bool isMainChar, bool wasCamTarget)
    {
        currGameObject.SetActive(false);
        if(isMainChar)
        {
            //In the case this ever tries to deactivate but is no longer the mainChar
            if(currMainCharScr == currGameObject.GetComponent<scr_BaseEntity_Main>())
                currMainCharScr = null;
        }
        else
        {

        }

        if(wasCamTarget)
        {
            camChangeSignal?.Invoke(currGameObject, false, GLOBAL_VARS.LerpType.linear, 0);
            //In activation, we'll destroy camSpots on replacement if they arent entities as they will almost never need to be used again, jsut to clean space
        }
    }
    //We run this in fixed update so that there is no choppiness when it comes to movement speed changes. Thatll be more noticeable than sprite speed changes.
    void ChangeSpeedFixedUpdate()
    {
        float lerpChange = MathF.Pow(lerpBase, -lerpCounter);
        lerpCounter += lerpFactor * Time.deltaTime;
        //Lerp factor will be a percentage of 100% that you want to complete every second (APPROX since time.DeltaTime isnt consistent)

        //We hit max counter, therefore we want it to exactly be the speed we're aiming at
        //Since entity speed will equal changetospeed, itll detect that everything is good and leave it as is.
        if(lerpCounter > lerpTotal)
        {
            lerpChange = 0f;
        }

        //We want to lerp "backwards" meaning we start at the speed we currently are at beginning of the lerp (b), and get closer to our target which is our minimum (a)
        entityControllerSpeed = Mathf.Lerp(changeToSpeed, previousSpeed, lerpChange);
    }

    //Runs on levelStartPrimedSignal
    void OnLevelLoadSignal()
    {   
        Debug.Log("Entity Controller Priming Coroutine Started");
        StartCoroutine(LevelPrimingCoroutine());
    }
    //Priming Coroutine aims to ready every entity (using their awake) and make sure its in the list
    IEnumerator LevelPrimingCoroutine()
    {
        GameObject currMainChar = GameObject.Find("mainBody");
        currMainChar.SetActive(true); //make sure activated 

        currMainCharScr = currMainChar.GetComponent<scr_BaseEntity_Main>();
        currMainCharScr.entityAwake(gameController);
        camChangeSignal?.Invoke(currMainChar, true, GLOBAL_VARS.LerpType.linear, 0f);
        
        
        entityArray = UnityEngine.Object.FindObjectsByType<scr_BaseEntity_Main>(FindObjectsSortMode.None).ToList<scr_BaseEntity_Main>();
        //Debug.Log(entityArray.Count);
        yield return false;

        for(int i = 0; i < entityArray.Count; i++)
        {   
            if(entityArray[i] != currMainCharScr) //Remove main char script, it should ALWAYS run first                              //Everything else will run, regardless of activity or not
            {
                entityArray[i].entityAwake(gameController);     //Awake will always be set up no matter what
                //Debug.Log(entityArray[i]);    
            }
            yield return false;
        }

        //Now check to make sure all inactive ones wont run their starts/awakes
        int currentSpot = 0;
        for(int i = 0; i < entityArray.Count; i++)
        {   
            //If the current spot is not active or it is the character script remove it, then check the next element that flows to the spot
            //Else we move the counter. Preserves at least an linear efficient search (though a list this small will not need to be scalable higher anyway)
            if(entityArray[currentSpot].gameObject.activeInHierarchy != true || entityArray[currentSpot] == currMainCharScr)
                entityArray.RemoveAt(currentSpot);
            else
                currentSpot++;
            yield return false;
        } 

        yield return new WaitForSecondsRealtime(3);

        levelPrimedSignal?.Invoke(true);
        Debug.Log("Entity Controller finished Priming!");
        yield return true;
    }
    void OnLevelDeLoadSignal()
    {
        Debug.Log("Entity Controller Deload Coroutine Started");
        StartCoroutine(LevelDeLoadCoroutine());
    }
    IEnumerator LevelDeLoadCoroutine()
    {
        camChangeSignal?.Invoke(currMainCharScr.gameObject, false, GLOBAL_VARS.LerpType.linear, 0);
        currMainCharScr = null;
        yield return false;
        for (int i = 0; i < entityArray.Count; i++)
        {
            entityArray.RemoveAt(i);
            yield return false;
        }
        
        entityControllerSpeed = 1.0f;
        previousSpeed = 1.0f;
        changeToSpeed = 1.0f;
        lerpFactor = 0.0f;

        levelDeLoadSignal?.Invoke(true);
        yield return true;
    }
}