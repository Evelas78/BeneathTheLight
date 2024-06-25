using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_solidFloor : MonoBehaviour
{
    public IDScript objectIDs;
    void Awake()
    {
        objectIDs = gameObject.AddComponent<IDScript>();
        objectIDs.objectType = GLOBAL_CONSTANTS.objectType.isSolid;
        objectIDs.damage_level = 0;
        objectIDs.hasEffectScript = false;
    }
}
