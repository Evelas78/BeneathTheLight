using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Camera : MonoBehaviour
{
    public GameObject target;
    // Update is called once per frame
    void LateUpdate()
    {
        //Keep same Z to preserve correct view
        gameObject.transform.position = new UnityEngine.Vector3(target.transform.position.x, target.transform.position.y, gameObject.transform.position.z);
    }
}
