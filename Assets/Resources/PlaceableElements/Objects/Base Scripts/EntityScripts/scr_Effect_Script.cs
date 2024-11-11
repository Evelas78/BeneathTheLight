using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class scr_Effect_Script : MonoBehaviour 
{
    protected enum EffectType 
    {
        Special,
        Damaging
    }
    
    public abstract void InstantiateEffectScript();

    public abstract void HitTarget(GameObject _hitBy, RaycastHit2D _currRaycastInfo, int hitType);
    //Function to remove itself from the currently affected object
    //(this basically allows for easier logic when iframes, or something conflicts)
    public void Escape(GameObject _hitBy, int _effectType)
    {
        scr_BaseEntity_Main currHitScript = _hitBy.GetComponent<scr_BaseEntity_Main>();
        
        switch (_effectType)
        {   
            //Special is a list and can be stacked
            case (int)EffectType.Special:
                int i = 0;
                ref List<scr_Effect_Script> currList = ref currHitScript.spEffectList;
                foreach(scr_Effect_Script testList in currList)
                {
                    if(testList == this)
                    {
                        currList.RemoveAt(i);
                        break;
                    }
                    i++;
                }
                break;

            case (int)EffectType.Damaging:
                currHitScript.dmgEffect = null;
                break;
        }

    }
}
