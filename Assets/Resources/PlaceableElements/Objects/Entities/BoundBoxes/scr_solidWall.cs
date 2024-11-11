using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class scr_solidFloor : scr_BaseEntity_Main
{
    public static IDScript solidFloorIDScript;
    public override void CharacterAwake()
    {
        isAnimated = false;
        isStationary = true;
        if(solidFloorIDScript == null)
        {
            solidFloorIDScript = new IDScript();
            solidFloorIDScript.ObjectType = GLOBAL_VARS.ObjectType.isWall;
            solidFloorIDScript.damage_level = 0;
            solidFloorIDScript.hasEffectScript = false;
        }
        objectIDScript = solidFloorIDScript;
        
    }
    public override void CharacterStart() {}
    public override void CharacterFixedUpdate(float __unused__) {}
    public override void CharacterUpdate(float __unused__) {}
    public override void CharacterDeathUpdate(float __unused__) {}
  
}
