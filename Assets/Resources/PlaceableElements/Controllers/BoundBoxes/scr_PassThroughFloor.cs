using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_PassThroughFloor : MonoBehaviour
{
    public IDScript objectIDs;
    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();
        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isPlatformFloor;
        objectIDs.damage_level = 0;
        objectIDs.hasEffectScript = false;
    }
}
