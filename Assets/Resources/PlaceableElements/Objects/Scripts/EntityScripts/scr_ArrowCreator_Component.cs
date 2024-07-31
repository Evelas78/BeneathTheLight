using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class scr_ArrowCreator_Component : MonoBehaviour
{
    //Maybe a scriptable Object might be nice here?
    [SerializeField] private GameObject obj_arrowHead;
    [SerializeField] private GameObject obj_arrowSegmentTemplate;
    [SerializeField] private GameObject obj_itemPool;

    [SerializeField] private List<GameObject> arrowSegmentList;
    [SerializeField] private int maxArrowSegments = -1;
    [SerializeField] private Vector2 segmentDimensions;
    [SerializeField] private Vector2 headDimensions;
    [SerializeField] private Vector2 DebugAngleVec;
    [SerializeField] private float DebugAimedAngleFloat;
    [SerializeField] private float DebugAngleFloat;
    private scr_levelController levelController;

    public void instantiateComponent(int _maxArrowSegments,  GameObject _arrowSegmentTemplateObject,  GameObject _arrowHeadTemplateObject, scr_levelController _levelController)
    {
        clearArrows();
        if(arrowSegmentList == null)
        {
            arrowSegmentList = new List<GameObject>();
        }

        levelController = _levelController;
        obj_itemPool = levelController.itemPool;
        instantiateArrowSegments(_maxArrowSegments, _arrowSegmentTemplateObject); 
        changeArrowHead(_arrowHeadTemplateObject);
    }
    private void instantiateArrowSegments(int _maxArrowSegments, GameObject _arrowSegmentTemplateObject)
    {
        obj_arrowSegmentTemplate = _arrowSegmentTemplateObject;
        maxArrowSegments = _maxArrowSegments;

        GameObject newArrowObject; 
        for(int i = 0; i < maxArrowSegments; i++)
        {
            newArrowObject = Instantiate(obj_arrowSegmentTemplate, obj_itemPool.transform);
            arrowSegmentList.Add(newArrowObject);
            //Dont set parent, theres not really a necessity to do so as this game doesn't require multiple hundreds of instances at once
            //and transform is what its paired to so it can mess it up as relative location matters (which is why i will switch off this engine after this game)
        }

        SpriteRenderer currSpriteRenderer = _arrowSegmentTemplateObject.GetComponent<SpriteRenderer>();

        segmentDimensions = new Vector2(currSpriteRenderer.bounds.size.x, currSpriteRenderer.bounds.size.y);
    }
    public void changeArrowHead(GameObject _arrowHeadTemplate)
    {
        if(obj_arrowHead != null)
        {
            Destroy(obj_arrowHead);
        }
        obj_arrowHead = Instantiate(_arrowHeadTemplate, obj_itemPool.transform);

        SpriteRenderer currSpriteRenderer = _arrowHeadTemplate.GetComponent<SpriteRenderer>();

        headDimensions = new Vector2(currSpriteRenderer.bounds.size.x, currSpriteRenderer.bounds.size.y);
    }

    public void bringPiece(Vector2 _placePosition, Quaternion _rotQuat, ref GameObject _target)
    {
        _target.transform.position = _placePosition;

        _target.transform.rotation = _rotQuat;
    }
    public void drawSegments(Vector2 _offsettedStart, Vector2 _angleVector, Vector2 _perpVector, Quaternion _rotQuat, int _totalNumofSegments)
    {
        float distanceCurrently = 0;
        Vector2 targetPosition;
        for(int i = 0; i < _totalNumofSegments; i++)
        {
            //We add the segment dimensions/2 so we can get the actual bottom spot, and have it so the segments lines up right on the middle of the angle vec
            targetPosition = _offsettedStart + (distanceCurrently + segmentDimensions.x / 2) * _angleVector;
            targetPosition += (segmentDimensions.y / 2) * _perpVector; //We just add the minor adjustments as vector arithmetic works the way it does

            GameObject currArrowSegment = arrowSegmentList.ElementAt(i);
            bringPiece(targetPosition, _rotQuat, ref currArrowSegment);

            //Add the length of the arrow segment
            distanceCurrently += segmentDimensions.x;
        }
    }
    public void drawHead(Vector2 _offsettedStart, Vector2 _endPos, Vector2 _angleVector, Vector2 _perpVector, Quaternion _rotQuat, int _totalNumofSegments, float _offsetDistanceBetween)
    {
        //Default to end spot
        Vector2 headPosition = _endPos;
        
        float maxDistance = maxArrowSegments * segmentDimensions.x;
        //If the arrow is = to its max amount of segments, then its possible to be stretched too far with endPos
        if(_totalNumofSegments == maxArrowSegments && _offsetDistanceBetween > maxDistance)
        {
            //Similar to the arrow segments, we just set it to the max distance linear combination to get it to the very end
            headPosition = _offsettedStart + (maxDistance + headDimensions.x) * _angleVector;
            //We then also apply the perpendicular angle to get it right as well
            headPosition += headDimensions.y / 2 * _perpVector;
        }

        bringPiece(headPosition, _rotQuat, ref obj_arrowHead);
    }
    //100% organize this function into multiple smaller functions, its becoming a mess
    public void drawArrow(Vector2 _startPos, Vector2 _endPos, float offset)
    {   
        //Pythagorean thm to get the distance between start and end, getting raw values, no offset
        float distanceBetween = Mathf.Pow(Mathf.Pow(_endPos.x - _startPos.x, 2) + Mathf.Pow(_endPos.y - _startPos.y, 2), (1/2f));
        Vector2 angleVector = (_endPos - _startPos) / distanceBetween; //Represents change of x and y, then we normalize it to make it a base linear combination
        DebugAngleVec = angleVector;
        
        //Perpendicular vector that is aimed clockwise (so it goes downwards rather than upwards)
        Vector2 perpVector = new Vector2(angleVector.y, -angleVector.x); //remember, dot product = 0 means perpendicular and just visualize drawing the arrow in your mind)
        Quaternion targetRotation = Quaternion.LookRotation(angleVector, Vector2.up);

        //Since the two above are now RAW values, we have the RAW angle and RAW distance
        //We use the raw angle to add to the original start position

        Vector2 offsettedStart = _startPos + offset * angleVector; //Offset * angle vector = aimed linear combination to add 
        float offsetDistanceBetween = distanceBetween - offset; //Also fix a correct offsetDistanceBetween for correct usage

        int totalNumofSegments = (Math.Ceiling(offsetDistanceBetween) > maxArrowSegments) ? maxArrowSegments : (int)Math.Ceiling(offsetDistanceBetween);
        //Using the list, start from to 0 to the totalNumOfSegments, placing each one next to each other
        drawSegments(offsettedStart, angleVector, perpVector, targetRotation, totalNumofSegments);
        drawHead(offsettedStart, _endPos, angleVector, perpVector, targetRotation, totalNumofSegments, offsetDistanceBetween);
    }

    public void storeArrowPieces()
    {
        for(int i = 0; i < maxArrowSegments; i++)
        {
            GameObject currArrowSegment = arrowSegmentList.ElementAt(i);
            bringPiece(obj_itemPool.transform.position, Quaternion.identity, ref currArrowSegment);
        }
        bringPiece(obj_itemPool.transform.position, Quaternion.identity, ref obj_arrowHead);
    }
    public void clearArrows()
    {   
        if(arrowSegmentList != null)
        {
            for(int i = 0; i < maxArrowSegments; i++)
            {
                GameObject currArrowSegment = arrowSegmentList.ElementAt(0);
                arrowSegmentList.RemoveAt(0);
                Destroy(currArrowSegment);
            }
        }
        if(obj_arrowHead != null)
        {
            Destroy(obj_arrowHead);
            obj_arrowHead = null;
        }
        
    }
}