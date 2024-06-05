using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class scr_animController : MonoBehaviour
{
    protected Dictionary<int,scr_animations> animationDictionary = new Dictionary<int, scr_animations>();
    protected SpriteRenderer objRenderer;
    protected GameObject currObject; 
    protected scr_Basic_Entity currEntityScript;
    void Awake()
    {
        currObject = gameObject;
        objRenderer = gameObject.GetComponent<SpriteRenderer>();
        currEntityScript = gameObject.GetComponent<scr_Basic_Entity>();
        fillDictionary();
    }

    // Update is called once per frame
    void Update()
    {  
        Sprite currentSprite = determineSpriteUpdate();
        objRenderer.flipX = (currEntityScript.getVelocity().x >= 0) ? false : true;
        determineSpriteUpdate();
    }

    public abstract void fillDictionary();
    public abstract Sprite determineSpriteUpdate();
}
