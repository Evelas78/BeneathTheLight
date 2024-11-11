using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scr_ClickButton : MonoBehaviour
{
    public Button currButton; 
    public RectTransform currTransform;

    //Current button type determines what the button should do
    public int buttonType;

    //This room will be transitioned too when this is click
    public string targetRoom;
    public bool isLevel;
    //This determines what transition will play.
    public int startTransitionType, endTransitionType;
    //This determines length of transition
    public int startTransitionTime, endTransitionTime;
    public int startLerpType, endLerpType;
    //How fast we should move the cam (basically time it should takes)
    public float moveTime;
    //How we should calc movement 
    public int lerpMoveType;
    //Position we want ti to to move to. Remember its in the dimensions of the resolution. 
    public Vector2 movePosition, originalAnchorPosition, originPosition;
    
    //Menu value to change, since we'll use a dictionary
    public int menuOptionChange;

    public static GLOBAL_VARS.buttonMovementSignal buttonMovementSignal;
    public static GLOBAL_VARS.buttonTransitionSignal buttonTransitionQuerySignal;
    public void InstantiateButton()
    {
        currButton = gameObject.GetComponent<Button>();
        currTransform = gameObject.GetComponent<RectTransform>();

        currButton.onClick.AddListener(() => OnClick());

        originPosition = currTransform.anchoredPosition;
        originalAnchorPosition = originPosition;
    }   

    public void OnClick()
    {
        switch(buttonType)
        {
            case GLOBAL_VARS.ButtonType.transitionScene:
                buttonTransitionQuerySignal?.Invoke(startTransitionType, endTransitionType, startLerpType, endLerpType, startTransitionTime, endTransitionTime, targetRoom, isLevel);
                break;
            case GLOBAL_VARS.ButtonType.valueChange:
                break;   
            case GLOBAL_VARS.ButtonType.moveScreen:
                buttonMovementSignal?.Invoke(movePosition, moveTime, lerpMoveType);
                break;  
            default:
                Debug.LogError("Issue with button type, current button type: " + buttonType);
            break;
        }
        //SceneManager.LoadScene("TestRoom");
    }
}
