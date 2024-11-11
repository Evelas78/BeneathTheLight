using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditorInternal;


public class scr_transitionController : MonoBehaviour
{
    private scr_gameController currGameController;
    //We get this to find the transitionScreen's animator and store this as a reference in any potential use case
    private GameObject transitionScreen;
    //Note: I don't like unity's animator controller systems as is the main reason I will move from this engine, since it somewhat takes away the control I like over animation
    //That is why I have my self designed animation system for sprites, but animation controllers for this 
    //To experiment essentially, its all about learning (at least thats the mindset I try to keep)
    private Animator transitionAnimatorController;


    //For the main game controller initial prime
    public static GLOBAL_VARS.controllerResponseSignal transitionControllerPrimedSignal;
    //To tell the main game our transition completed (for scene work)
    public static GLOBAL_VARS.transitionCompleted transitionCompletedSignal;


    //Initial instantiation of necessary info (stuff that doesnt take time to load, its a necessity to do this) Constant Time work
    public void InstantiateTransitionController(scr_gameController _currGameController)
    {
        currGameController = _currGameController;
        scr_gameController.gameInitialPrimeSignal += InitialGamePrime;

        scr_gameController.transitionStartSignal += transitionReceiveSignal;
    }   

    public void transitionControllerUpdate(float _gameSpeedMult)
    {
        transitionWorker(_gameSpeedMult);
    }
    float transitionCounter = 0;
    public void transitionWorker(float _gameSpeedMult)
    {
        if(transitionTime != -1)
        {
            //We send in the counter number to lerp it from 0 to a 100, divide by a hundred, and clamp to make sure it stays within 0-1
            float _transitionAmount = Mathf.Clamp(GLOBAL_VARS.lerpWorker(lerpType, lerpStartVal, lerpEndVal, transitionCounter, transitionTime) / 100, 0, 1);
            //This is the value of how far into the transition it is
            transitionAnimatorController.SetFloat("CompletionState", _transitionAmount);
            
            if(transitionCounter >= transitionTime)
            {
                switch(transitionType)
                {
                    case GLOBAL_VARS.transitionType.basicOpenFade:
                        transitionAnimatorController.SetBool("onOpenFade", false);
                    break;
                    case GLOBAL_VARS.transitionType.basicCloseFade:
                        transitionAnimatorController.SetBool("onCloseFade", false);
                    break;
                    default:
                        Debug.LogError("No such transition {" + transitionType + "} type exists to end");
                    break;
                }

                transitionCounter = 0;
                transitionTime = -1;
                transitionCompletedSignal?.Invoke();

                //Debug.Log("transition Counter hit transitionTime"); 
            }
            else //if not above or equal to transition time, we add to the counter
                transitionCounter += Time.deltaTime * _gameSpeedMult;
        }
    }
    [SerializeField] int transitionType, lerpType;
    [SerializeField] float lerpStartVal = 0f, lerpEndVal = 100f;
    [SerializeField] bool debugIsClose;
    [SerializeField] float transitionTime = -1;
    public void transitionReceiveSignal(int _transitionType, int _lerpType, float _transitionTime, bool _isCloseTrans)
    {
        Debug.Log("transition signal receieved - TransitionController {TransitionReceiveSignal}");
        //Trigger it so the animator Controller knows which transition is active.
        switch(_transitionType)
        {
            case GLOBAL_VARS.transitionType.basicOpenFade:
                transitionAnimatorController.SetBool("onOpenFade", true);
            break;
            case GLOBAL_VARS.transitionType.basicCloseFade:
                transitionAnimatorController.SetBool("onCloseFade", true);
            break;
            default:
                Debug.LogError("No such transition {" + _transitionType + "} type exists to prime");
            break;
        }

        //If its close, then it goes from 0 - 100, if its open, then its the opposite, so it reverses the numbers.
        if(_isCloseTrans)
        {
            lerpStartVal = 0f;
            lerpEndVal = 100f;
        }
        else
        {
            lerpStartVal = 100f;
            lerpEndVal = 0f; 
        }

        debugIsClose = _isCloseTrans;
        transitionType = _transitionType; 
        lerpType = _lerpType;
        transitionTime = _transitionTime;
    }

    //As with all other prime functions, we start Coroutines to seperate our work into pieces (not multithreaded) so we can have the game move, while the game lopads instead of freezing
    public void InitialGamePrime()
    {
        Debug.Log("Transition Controller Coroutine Start");
        StartCoroutine(InitialGameCoroutine());
    }
    IEnumerator InitialGameCoroutine()
    {
        transitionScreen = GameObject.Find("TransitionScreen");
        yield return true;

        transitionAnimatorController = transitionScreen.GetComponent<Animator>();
        yield return new WaitForSecondsRealtime(3);

        Debug.Log("Transition Controller Coroutine Finished");
        transitionControllerPrimedSignal?.Invoke(true);
        yield return true;
    }
}