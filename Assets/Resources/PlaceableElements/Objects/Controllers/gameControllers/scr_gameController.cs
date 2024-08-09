using UnityEngine;
using UnityEngine.SceneManagement;
using System;



//What we want this to do and child components to do. 
//Send and Receive signals for game transitions
//Manage buttons and menus, allowing us to choose levels or change settings
//Store data to save
//Handle Music/Sounds
//Handle Scene Loading
public class scr_gameController : MonoBehaviour
{
    //scr_transitionController transitionController;
    scr_musicController musicController;
    scr_menuController menuController;
    GameObject entityControllerTemplate;
    public static event GLOBAL_VARS.sceneLoad sceneLoadSignal;
    public static event GLOBAL_VARS.sceneLoad sceneEndLoadSignal;
    int sceneType = GLOBAL_VARS.sceneType.menu;
    //Theoretically, this is the absolute FIRST thing to run in the game. So all set up and saving must be done either here or by a child component
    void Awake()
    {
        //First things first, the game controller must NEVER be destroyed. 
        //Therefore we
        //This only runs once thankfully (me thinks)

        //THIS ONLY RUNS ONCE SO KEEP IT IN MIND, SAME WITH ALL COMPONENTS
        DontDestroyOnLoad(gameObject);

        //transitionController = gameObject.AddComponent<scr_transitionController>();
        //transitionController.initializeDictionary();
        musicController = gameObject.AddComponent<scr_musicController>();
        menuController = gameObject.AddComponent<scr_menuController>();

        SceneManager.sceneLoaded += onSceneLoad;

    }
    void onSceneLoad(Scene _newScene, LoadSceneMode mode) //ignore loadSceneMode
    {
        if(sceneType == GLOBAL_VARS.sceneType.level)
        {
            //We create the entityController if we receive a signal which tells us the incoming scene is a level instead of a menu
            //Since this is the first thing that SHOULD run, then everything should theoretically work out I hope...
            Instantiate(entityControllerTemplate);
        }
    }

    //Transition should be the only thing that isnt a coroutine during a scene priming. 
}