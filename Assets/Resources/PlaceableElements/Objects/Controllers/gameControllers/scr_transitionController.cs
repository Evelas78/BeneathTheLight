/*
using UnityEngine;
using System.Collections.Generic;
public class scr_transitionController : MonoBehaviour
{
    public static Dictionary<int,scr_transition> animationDictionary = new Dictionary<int, scr_transition>(); 
    protected static bool dictionaryMade = false;
    public static scr_gameController currGameController;
    public void transitionControllerAwake(scr_gameController _currGameController)
    {
        currGameController = _currGameController;
        initializeDictionary();

        scr_gameController.sceneLoadSignal += sceneLoadFunc;
    }
    public void initializeDictionary()
    {
        if(!dictionaryMade)
        {
            scr_transition newAnim = new scr_trans_basicFallBlack("Transitions", "Controller");
            animationDictionary.Add(GLOBAL_VARS.CharacterStates.Idle, newAnim);
        }
    }

    public void sceneLoadFunc(int _transType)
    {

    }
    public void transitionLoad(int _transitionType)
    {
        scr_animations newAnimation;

        try
        {
            if(animationDictionary.TryGetValue(_transitionType, out newAnimation))
            {
                //Debug.Log("Animation Loaded " + nameof(newAnimation));
                spriteLoad(newAnimation);
            }
            else
            {
                Debug.LogError("No Transition found at this key " + _transitionType);
            }
        }
        catch(KeyNotFoundException)
        {
            Debug.LogError("Key does not exist in mainBody dictionary = " + _transitionType);
        }

    }
}
*/