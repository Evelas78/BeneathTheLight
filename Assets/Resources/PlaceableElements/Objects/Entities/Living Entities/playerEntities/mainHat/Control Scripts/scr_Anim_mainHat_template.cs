using System;
using UnityEngine;
//Templates are basically necessary so that they can access their specific script entity script correctly. 
public abstract class scr_Anim_mainHat_template : scr_animations
{
    public scr_mainHat_Main currEntityScript;
    public scr_Anim_mainHat_template(string _objectName, string _objectType) : base(_objectName, _objectType) {}
    public override void convertToSpecificScript(scr_BaseEntity_Main _currEntityScript)
    {
        currEntityScript = (scr_mainHat_Main)_currEntityScript;
    }
}
