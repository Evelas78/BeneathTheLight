using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PassThroughFloor : scr_BaseEntity_Main
{
    public static IDScript passThroughFloorIDScript;

    public override void CharacterAwake()
    {
        if(passThroughFloorIDScript == null)
        {
            passThroughFloorIDScript = new IDScript();
            passThroughFloorIDScript.ObjectType = GLOBAL_VARS.ObjectType.isWall;
            passThroughFloorIDScript.damage_level = 0;
            passThroughFloorIDScript.hasEffectScript = false;
            passThroughFloorIDScript.passThroughLeft = true;
            passThroughFloorIDScript.passThroughRight = true;
            passThroughFloorIDScript.passThroughUp = true;
        }
        objectIDScript = passThroughFloorIDScript;
    }
    public override void CharacterStart() {}
    public override void CharacterFixedUpdate(float __unused__) {}
    public override void CharacterUpdate(float __unused__) {}
    public override void CharacterDeathUpdate(float __unused__) {}
}
