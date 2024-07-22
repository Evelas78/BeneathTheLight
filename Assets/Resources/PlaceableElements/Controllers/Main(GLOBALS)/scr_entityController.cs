using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
public class scr_entityController : MonoBehaviour {
    public bool characterUpdateStart = true;
    //Since character Update wants to go first each time, we have it seperated to ensure it
    public scr_BaseEntity_Main currMainCharScr;
    //We use this to run every entities array
    public List<scr_BaseEntity_Main> entityArray;
    public scr_gameController gameController;
    void Awake()
    {
        this.enabled = false;
        gameController = gameObject.GetComponent<scr_gameController>();
        gameController.levelPrimedSignal += onTrigger;
    }
    void onTrigger()
    {
        this.enabled = true;
        GameObject currMainChar = GameObject.Find("mainBody");
        currMainCharScr = currMainChar.GetComponent<scr_BaseEntity_Main>();
        entityArray = UnityEngine.Object.FindObjectsOfType<scr_BaseEntity_Main>().ToList<scr_BaseEntity_Main>();
        
        for(int i = 0; i < entityArray.Count; i++)
        {
            if(entityArray[i] == currMainCharScr || entityArray[i].gameObject.activeInHierarchy == true)
            {
                entityArray.RemoveAt(i);
            }
        }

        currMainCharScr.entityAwake();
    }
    void Start()
    {
        currMainCharScr.entityStart();
    }
    void Update()
    {
        if(characterUpdateStart)
        {
            currMainCharScr.entityUpdate();
            
            for(int i = 0; i < entityArray.Count; i++)
            {
                entityArray[i].entityUpdate();
            }
        }
    }
    void FixedUpdate() {
        currMainCharScr.entityFixedUpdate();
    }
}