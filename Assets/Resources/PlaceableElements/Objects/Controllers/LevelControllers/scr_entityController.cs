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
    public scr_levelController levelController;
    public static event GLOBAL_VARS.camChangeSignal camChangeSignal;    
    public static event GLOBAL_VARS.levelPrimer entitiesPrimedSignal;
    public static IDScript entityControllerIDScript;
    //Previous speed to store and use for lerping back and forth
    [SerializeField] private float entitySpeed = 1.0f, previousSpeed = 1.0f, changeToSpeed = 1.0f, lerpFactor = 0.0f;
    
    //Awake is always ALWAYS just setup. if itll affect something else in the scene, then it should not run in it.
    public void entityControllerAwake(scr_levelController _LevelController)
    {
        levelController = _LevelController;

        if(entityControllerIDScript == null)
        {
            entityControllerIDScript = new IDScript();
            entityControllerIDScript.ObjectType = GLOBAL_VARS.ObjectType.isController;
            entityControllerIDScript.ControllerType = GLOBAL_VARS.ControllerType.entityController;
        }
        objectIDScript = entityControllerIDScript;

        scr_levelController.levelStartPrimedSignal += onLevelTrigger;
        scr_BaseEntity_Main.entitySlowSignal += onSlowTrigger;
        scr_BaseEntity_Main.entityActivatedSignal += onEntityActivation;
        scr_BaseEntity_Main.entityDeactivatedSignal += onEntityDeactivation;
    }
    public void entityControllerStart()
    {
        if(currMainCharScr != null)
            currMainCharScr.entityStart();
        for(int i = 0; i < entityArray.Count; i++)
        {
            entityArray[i].entityStart();
        }
    }
    public void entityControllerUpdate()
    {
        if(currMainCharScr != null)
            currMainCharScr.entityUpdate(entitySpeed);
        for(int i = 0; i < entityArray.Count; i++)
            entityArray[i].entityUpdate(entitySpeed);
    }
    public void entityControllerFixedUpdate() 
    {
        //Debug.Log("Running Fixed Update EC");

        //We check if our changeSpeed is different from entitySpeed then run it if so
        //previous speed will update on its own so its nothing we need to check for
        if(changeToSpeed != entitySpeed)
        {
            changeSpeedFixedUpdate();
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
    void onSlowTrigger(float _changeToSpeed, float _lerpFactor)
    {   
        //Previous speed now equals are previous target speed (initially equalling one)
        previousSpeed = changeToSpeed;
        changeToSpeed = _changeToSpeed;
        //Lerp factor will always change to what we aimed for before we run our fixed update
        lerpFactor = _lerpFactor;
    }    
    //Load into the entity array to add it to the update list
    void onEntityActivation(GameObject currGameObject, bool isMainChar, bool isNewCamTarget)
    {
        currGameObject.SetActive(true);
        //Run the start function of the newly activated object
        currGameObject.GetComponent<scr_BaseEntity_Main>().entityStart();

        Debug.Log(currGameObject + " Has activated");
        if(isMainChar)
        {
            Debug.Log("New Mainchar scr");
            currMainCharScr = currGameObject.GetComponent<scr_BaseEntity_Main>();
        }
        else
        {

        }

        if(isNewCamTarget)
        {
            camChangeSignal?.Invoke(currGameObject, true);
        }
    }
    //Unload into the entity array to remove it from the update lists
    void onEntityDeactivation(GameObject currGameObject, bool isMainChar, bool wasCamTarget)
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
            camChangeSignal?.Invoke(currGameObject, false);
            //In activation, we'll destroy camSpots on replacement if they arent entities as they will almost never need to be used again, jsut to clean space
        }
    }
    //We run this in fixed update so that there is no choppiness when it comes to movement speed changes. Thatll be more noticeable than sprite speed changes.
    void changeSpeedFixedUpdate()
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
        entitySpeed = Mathf.Lerp(changeToSpeed, previousSpeed, lerpChange);
    }

    //Runs on levelStartPrimedSignal
    void onLevelTrigger(int __unused__)
    {   
        Debug.Log("Entity Controller Priming Coroutine Started");
        StartCoroutine(primingCoroutine());
    }
    //Priming Coroutine aims to ready every entity (using their awake) and make sure its in the list
    IEnumerator primingCoroutine()
    {
        GameObject currMainChar = GameObject.Find("mainBody");
        currMainChar.SetActive(true); //make sure activated 

        currMainCharScr = currMainChar.GetComponent<scr_BaseEntity_Main>();
        currMainCharScr.entityAwake(levelController);
        camChangeSignal?.Invoke(currMainChar, true);

        
        entityArray = UnityEngine.Object.FindObjectsOfType<scr_BaseEntity_Main>(true).ToList<scr_BaseEntity_Main>();
        Debug.Log(entityArray.Count);
        yield return false;

        for(int i = 0; i < entityArray.Count; i++)
        {   
            if(entityArray[i] == currMainCharScr) //Remove main char script, it should ALWAYS run first
                entityArray.RemoveAt(i);
            else                                  //Everything else will run, regardless of activity or not
                entityArray[i].entityAwake(levelController);     //Awake will always be set up no matter what

            yield return false;
        }

        //Now check to make sure all inactive ones wont run their starts/awakes
        for(int i = 0; i < entityArray.Count; i++)
        {   
            if(entityArray[i].gameObject.activeInHierarchy != true)
                entityArray.RemoveAt(i);
            yield return false;
        } 

        yield return new WaitForSecondsRealtime(3);

        entitiesPrimedSignal?.Invoke(entityControllerIDScript.ControllerType);
        Debug.Log("Entity Controller finished Priming!");
        yield return true;
    }
}