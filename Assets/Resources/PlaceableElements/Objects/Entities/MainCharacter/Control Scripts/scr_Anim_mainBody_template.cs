using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Templates are basically necessary so that they can access their specific script entity script correctly. 
public abstract class scr_Anim_mainBody_template : scr_animations
{
    
    public scr_mainBody_Main currEntityScript;
    public scr_Anim_mainBody_template(GameObject _currGameObject) : base(_currGameObject) {}
    public override void convertToSpecificScript(scr_BaseEntity_Main _currEntityScript)
    {
        currEntityScript = (scr_mainBody_Main)_currEntityScript;
    }
}
