using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class scr_gameController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera currCam;
    public GameObject currMainChar;
    public scr_Menu currMenu = null;
    public scr_BaseEntity_Main currMainCharScr;
    public delegate void levelPrimed();
    public event levelPrimed levelPrimedSignal;
    public float gameSpeed = 1.0f;

    void Awake()
    {   
        currCam = gameObject.GetComponent<Camera>();
        onSceneLoad();
        //Use coroutines to load all of this stuff
    }

    void Start()
    {
        
    }

    void onSceneLoad()
    {
        currMainChar = GameObject.Find("mainBody");
        gameObject.AddComponent<scr_entityController>();
        levelPrimedSignal?.Invoke();
    }
    //Menu Controller
    void Update()
    {
        if(currMenu != null)
        {
            currMenu.menuUpdate();
        }
    }
    void LateUpdate()
    {
        currCam.transform.position = new UnityEngine.Vector3(currMainChar.transform.position.x, currMainChar.transform.position.y, currCam.transform.position.z);
    }

    //Means gameover/character died
    void runLoss()
    {
        //End Scene, by running transition AFTER sprite death runs. 
        //Pull up menu
    }
}
