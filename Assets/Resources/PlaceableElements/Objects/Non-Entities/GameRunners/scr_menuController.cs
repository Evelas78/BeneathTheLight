using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;


public class scr_menuController : MonoBehaviour
{
    public scr_gameController currGameController;
    public static event GLOBAL_VARS.controllerResponseSignal initialGamePrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal menuPrimedSignal;
    public static event GLOBAL_VARS.controllerResponseSignal levelPrimedSignal;
    public static event GLOBAL_VARS.buttonTransitionSignal transitionSceneConfirmationSignal;
    public List<scr_ClickButton> buttonList = new List<scr_ClickButton>();

    //The Positions for both menus, in relative to the pivot button.
    public Vector2 mainMenuPosition = new Vector2(0, 4000), pauseMenuPosition = new Vector2(0, -4000);
    //This is the button we can use to navigate from the mainMenuPosition to pauseMenuPosition;
    public scr_ClickButton pivotButton;
    public bool isAcceptingButtonSignals = false;
    //Essentially an awake
    public void InstantiateMenuController(scr_gameController _currGameController)
    {
        currGameController = _currGameController;

        scr_gameController.gameInitialPrimeSignal += OnInitialPrime;
        scr_gameController.levelLoadSignal += onLevelLoadScene;
        scr_gameController.menuLoadSignal += onMenuLoadScene;

        scr_ClickButton.buttonMovementSignal += traverseCanvasReceiver;
        scr_ClickButton.buttonTransitionQuerySignal += transitionSceneReceiver;
        initialGamePrimedSignal?.Invoke(true);
    }
    public void menuControllerUpdate(float _gameSpeedMultiplier)
    {
        traverseCanvas(_gameSpeedMultiplier);
    }

    //Goal is basically to grab all the buttons in the game, then load it in. So when we move, since canvas is permanently saved, we can save all the buttons by moving them super lower and deactivating then reactivating then and repositioning on scene reload
    public void OnInitialPrime()
    {
        Debug.Log("Menu Controller Priming");
        StartCoroutine(InitialPrimeCoroutine());
    }
    IEnumerator InitialPrimeCoroutine()
    {
        //Gets a list of buttons to use to find their gameObjects
        buttonList = GameObject.FindObjectsByType<scr_ClickButton>(FindObjectsSortMode.None).ToList<scr_ClickButton>();
        yield return false;

        int _counter = 0;
        foreach(scr_ClickButton buttonScript in buttonList)
        {
            buttonScript.InstantiateButton();
            yield return false;
            _counter++;
        }       
        yield return true;

        GameObject o_pivotButton = GameObject.Find("Pivot Button");
        yield return new WaitForSecondsRealtime(3);

        pivotButton = o_pivotButton.GetComponent<scr_ClickButton>();
        yield return true;

        Debug.Log("MenuController Initial Priming Complete");
        initialGamePrimedSignal?.Invoke(true);
        isAcceptingButtonSignals = true;
        isAcceptingTraversing = true;
        yield return true;
    }


    //How all of our buttons will move, in relative to the locations. While we could do it based on the pivot button, it makes it easier to track in the game itself.
    //It does have the disadvantage of not being able to set exact position immediately, however.
    public Vector2 targetMoveVec;

    //When they arent equal, traversing will happen. 
    public float targetTime = 0, currCounter = 0;
    //Decouple from accepting buttonSignals since itll be cool if you can click buttons as it moving to load in. 
    public bool isAcceptingTraversing = false;
    public int lerpType;
    public void traverseCanvasReceiver(Vector2 _targetMoveVec, float _targetTime, int _lerpType)
    {
        if(isAcceptingTraversing)
        {
            targetMoveVec = _targetMoveVec;
            targetTime = _targetTime;
            isAcceptingTraversing = false;
            lerpType = _lerpType;
            /*
            //Basically, a test that would allow us to stack traverses so skilled people will move around the menu quickly
            //We would have to change how traverseAccepting works and make sure the Lerp position is done correctly, not leaping since direction change is messing it up.
            //Also positional changes would have to be made so it actually goes to a location not just a distance to a move.
            foreach(scr_ClickButton buttonScript in buttonList)
                    buttonScript.originalAnchorPosition = buttonScript.currTransform.anchoredPosition;
            */
        }
    }
    public void traverseCanvas(float _gameSpeedMultiplier)
    {
        if(targetTime != currCounter)
        {
            currCounter += Time.deltaTime * _gameSpeedMultiplier;
            
            float xChange = GLOBAL_VARS.lerpWorker(lerpType, 0, targetMoveVec.x, currCounter, targetTime), yChange = GLOBAL_VARS.lerpWorker(lerpType, 0, targetMoveVec.y, currCounter, targetTime); 
            moveButtons(xChange, yChange);
            
            //Reset basically, allowing for traversing again, and reseting the timers. 
            if(currCounter/targetTime >= 1.0)
            {
                isAcceptingTraversing = true;
                targetTime = 0;
                currCounter = 0; 
                foreach(scr_ClickButton buttonScript in buttonList)
                {
                    buttonScript.originalAnchorPosition = buttonScript.currTransform.anchoredPosition;
                }
            }
        }
    }
    public void moveButtons(float _newXMove, float _newYMove)
    {
        foreach(scr_ClickButton buttonScript in buttonList)
        {
            buttonScript.currTransform.anchoredPosition = new Vector2(buttonScript.originalAnchorPosition.x + _newXMove, buttonScript.originalAnchorPosition.y + _newYMove);
        }
    }
    public void transitionSceneReceiver(int _startTransType, int _endTransType, int _startLerpType, int _endLerpType, float _startTransTime, float _endTransTime, string _levelName, bool _isLevel)
    {
        if(isAcceptingButtonSignals)
        {
            isAcceptingButtonSignals = false;
            isAcceptingTraversing = false;
            Debug.Log("Transition Scene Signal Received from Button");

            //Create an event that sends to the gameController that yes, there is actually gonna be a scene transition
            transitionSceneConfirmationSignal?.Invoke(_startTransType, _endTransType, _startLerpType, _endLerpType, _startTransTime, _endTransTime, _levelName, _isLevel);
        }
    }
    
    public void resetTargetMoveToMainMenuPos()
    {
        targetMoveVec = mainMenuPosition - pivotButton.originalAnchorPosition;
    }
    public void resetTargetMoveToPauseMenuPos()
    {
        targetMoveVec = pauseMenuPosition - pivotButton.originalAnchorPosition;
    }

    //Basically a full reset. We move our buttons to the main menu position before the transition starts to show the screen
    public void onMenuLoadScene()
    {
        Debug.Log("Menu New Scene Primer Running");
        isAcceptingButtonSignals = false;
        isAcceptingTraversing = false;   
        StartCoroutine(MenuPrimingCoroutine());
    }
    IEnumerator MenuPrimingCoroutine()
    {
        resetTargetMoveToMainMenuPos();
        yield return true;
        moveButtons(targetMoveVec.x, targetMoveVec.y);
        yield return true;


        targetTime = 0f;
        lerpType = 0;
        currCounter = 0f;
        isAcceptingButtonSignals = true;
        isAcceptingTraversing = true;
        yield return true;
        
        yield return new WaitForSecondsRealtime(3);
        Debug.Log("Menu Controller Menu Primer done running");
        menuPrimedSignal?.Invoke(true);
    }

    //Level Load Menu. Basically we set position to pause menu, then let it start listening for the esc button.
    public void onLevelLoadScene()
    {
        Debug.Log("Level New Scene Primer Running");
        isAcceptingButtonSignals = false;
        isAcceptingTraversing = false;   
        StartCoroutine(LevelPrimingCoroutine());
    }
    IEnumerator LevelPrimingCoroutine()
    {
        resetTargetMoveToPauseMenuPos();
        yield return true;
        moveButtons(targetMoveVec.x, targetMoveVec.y);
        yield return true;


        targetTime = 0f;
        lerpType = 0;
        currCounter = 0f;

        yield return true;

        yield return new WaitForSecondsRealtime(3);
        Debug.Log("Menu Controller Level Primer done running");
        levelPrimedSignal?.Invoke(true);
    }

}