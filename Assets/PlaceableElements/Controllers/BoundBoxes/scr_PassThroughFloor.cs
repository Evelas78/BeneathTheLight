using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PassThroughFloor : MonoBehaviour
{
    public IDScript objectIDs;
    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();
        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isFloor;
        objectIDs.versionType = GLOBAL_CONSTANTS.floorType.platform;
        objectIDs.jump_level = GLOBAL_CONSTANTS.jumpType.jumpable;
        objectIDs.damage_level = 0;
    }
}
