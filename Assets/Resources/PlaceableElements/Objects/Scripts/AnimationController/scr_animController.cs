using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_animController : MonoBehaviour
{   
    protected SpriteRenderer objRenderer;
    protected scr_Basic_Entity currEntityScript;
    
    void Awake()
    {
        //Grabs important items 
        objRenderer = gameObject.GetComponent<SpriteRenderer>();
        currEntityScript = gameObject.GetComponent<scr_Basic_Entity>();
    }

    protected scr_animations currAnim = null;

    // Update is called once per frame
    void Update()
    {  
        Sprite currentSprite = currAnim.animationScript(objRenderer, currEntityScript, this);
        //If positive, flip, else if negative, flip. Otherwise, stay the same.
        objRenderer.flipX = (currEntityScript.getVelocity().x > 0) ? false : (currEntityScript.getVelocity().x < 0) ? true : objRenderer.flipX;
        objRenderer.sprite = currentSprite;
    }
    
    //thisll be called everytime we want to play a new animation/reset
    public void spriteLoad(scr_animations targetAnim)
    {
        //Check if previous state is the same, else we wanna flush out the previous states data
        if(currAnim != targetAnim)
           Destroy(currAnim);

        currAnim = targetAnim;
    }

}
