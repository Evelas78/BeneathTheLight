using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scr_Button_Press : MonoBehaviour
{

    Button currButton; 

    // Start is called before the first frame update
    void Start()
    {
        currButton = gameObject.GetComponent<Button>();
        currButton.onClick.AddListener(sceneChange);
    }   

    void sceneChange()
    {
        SceneManager.LoadScene(1);
    }
}
