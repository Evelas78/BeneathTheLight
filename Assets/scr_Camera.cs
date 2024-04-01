using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Camera : MonoBehaviour
{
    public GameObject mainChar;
    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position = new UnityEngine.Vector3(mainChar.transform.position.x, mainChar.transform.position.y, gameObject.transform.position.z);
    }
}
