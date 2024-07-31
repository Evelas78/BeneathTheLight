using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_solidFloor : MonoBehaviour
{
    public static IDScript objectIDs;
    public static bool createdIDScript = false;
    public void Awake()
    {
        if(!createdIDScript)
        {
            objectIDs = gameObject.AddComponent<IDScript>();
            objectIDs.ObjectType = GLOBAL_VARS.ObjectType.isWall;
            objectIDs.damage_level = 0;
            objectIDs.hasEffectScript = false;
        }
    }
  
}
