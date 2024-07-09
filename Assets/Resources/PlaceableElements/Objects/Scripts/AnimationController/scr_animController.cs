using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class scr_animController : MonoBehaviour
{   
    [SerializeField] protected SpriteRenderer objRenderer;
    [SerializeField] protected scr_Basic_Entity currEntityScript;
    protected int initialAnimation = GLOBAL_CONSTANTS.CharacterStates.Idle;
    protected Dictionary<int,scr_animations> animationDictionary = new Dictionary<int, scr_animations>(); 
    void Awake()
    {
        //Grabs important items 
        objRenderer = gameObject.GetComponent<SpriteRenderer>();
        currEntityScript = gameObject.GetComponent<scr_Basic_Entity>();
        initializeDictionary();
        SpriteController(initialAnimation);
    }

    [SerializeField] protected scr_animations currAnim = null;

    //Logical vars for going through the frames of the animation
    [SerializeField] protected float currFrameProg = 0;
    [SerializeField] protected int currFrameNum = 0;
    
    //As most functions will contain a loop of frames, this is here to keep track of it
    [SerializeField] protected int currLoopCount = 0;


    // Update is called once per frame
    void Update()
    {  
        Sprite currentSprite = currAnim.animationScript(objRenderer, this, ref currFrameProg, ref currFrameNum, ref currLoopCount);
        //If positive, flip, else if negative, flip. Otherwise, stay the same.
        objRenderer.flipX = (currEntityScript.getMovementDirection().x > 0) ? false : (currEntityScript.getMovementDirection().x < 0) ? true : objRenderer.flipX;
        objRenderer.sprite = currentSprite;
    }
    
    //thisll be called everytime we want to play a new animation/reset
    public void spriteLoad(scr_animations _targetAnim)
    {
        //Check if previous state is the same, else we wanna flush out the previous states data
        if(currAnim != _targetAnim && currAnim != null)
            currFrameNum = 0;
            currFrameProg = 0;
            currLoopCount = 0;

        currAnim = _targetAnim;
    }
    public abstract void initializeDictionary();
    public abstract void SpriteController(int _characterState);

}
