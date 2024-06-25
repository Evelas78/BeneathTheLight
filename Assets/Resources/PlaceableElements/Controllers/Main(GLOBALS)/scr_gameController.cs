using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_gameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject currCamTarget;
    public Camera currCam;
    public GameObject currGameController;
    public GameObject currMainChar;
    public scr_Menu currMenu = null;
    public scr_Basic_Entity currMainCharScr;

    void Awake()
    {
        currMainChar = GameObject.Find("mainBody");
        currMainCharScr = currMainChar.GetComponent<scr_Basic_Entity>();
        
        currCamTarget = currMainChar.gameObject;
        currCam = Camera.main;
        currCam.GetComponent<scr_Camera>().target = currCamTarget;

        currGameController = gameObject;
    }

    //Menu Controller
    void Update()
    {
        if(currMenu != null)
        {
            currMenu.menuUpdate();
        }
    }

    //Means gameover/character died
    void runLoss()
    {
        //End Scene, by running transition AFTER sprite death runs. 
        //Pull up menu
    }
}
