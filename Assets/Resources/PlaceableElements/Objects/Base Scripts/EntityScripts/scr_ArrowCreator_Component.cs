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
    [SerializeField] private scr_gameController gameController;

    public void instantiateComponent(int _maxArrowSegments,  GameObject _arrowSegmentTemplateObject,  GameObject _arrowHeadTemplateObject, scr_gameController _gameController)
    {
        clearArrows();
        if(arrowSegmentList == null)
        {
            arrowSegmentList = new List<GameObject>();
        }

        gameController = _gameController;
        //Debug.Log(gameController.itemPool);
        obj_itemPool = gameController.itemPool;
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

    public void bringPiece(Vector3 _placePosition, Quaternion _rotQuat, ref GameObject _target)
    {
        _target.transform.position = _placePosition;

        _target.transform.rotation = _rotQuat;
    }
    public void drawSegments(Vector2 _offsettedStart, Vector2 _endPos, Vector2 _angleVector, Quaternion _rotQuat, int _totalNumofSegments, float _offsetDistanceBetween, float _zPos)
    {
        float distanceCurrently = 0;
        Vector3 targetPosition;
        for(int i = 0; i < _totalNumofSegments; i++)
        {
            //We add the segment dimensions/2 so we can get the actual bottom spot, and have it so the segments lines up right on the middle of the angle vec
            targetPosition = _offsettedStart + (distanceCurrently + segmentDimensions.x) * _angleVector;

            GameObject currArrowSegment = arrowSegmentList.ElementAt(i);
            targetPosition.z = _zPos;
            bringPiece(targetPosition, _rotQuat, ref currArrowSegment);

            //Add the length of the arrow segment
            distanceCurrently += segmentDimensions.x;
        }
    }
    public void drawHead(Vector2 _offsettedStart, Vector2 _endPos, Vector2 _angleVector, Quaternion _rotQuat, int _totalNumofSegments, float _offsetDistanceBetween, float _zPos)
    {
        //Default to end spot
        Vector3 headPosition = _endPos;
        
        //So our max distance is our arrow segment cumulative total distance plus our half our head as explained in belows calculation
        float maxDistance = maxArrowSegments * segmentDimensions.x + headDimensions.x/2;
        //If the arrow is = to its max distance then it has stretched too far
        if(_totalNumofSegments == maxArrowSegments && _offsetDistanceBetween > maxDistance)
        {
            //Similar to the arrow segments, we just set it to the max distance linear combination to get it to the very end
            //Divide by two since arrowheads position is in the center and thus only want to go half out the full length of the sprite to avoid a gap
            headPosition = _offsettedStart + maxDistance * _angleVector;
        }
        headPosition.z = _zPos;
        bringPiece(headPosition, _rotQuat, ref obj_arrowHead);
    }
    //100% organize this function into multiple smaller functions, its becoming a mess
    public void drawArrow(Vector2 _startPos, Vector2 _endPos, float _offset)
    {   
        //Always do this, so pieces dont stay where they were earlier
        storeArrowPieces();

        //Pythagorean thm to get the distance between start and end, getting raw values, no offset
        float distanceBetween = Mathf.Pow(Mathf.Pow(_endPos.x - _startPos.x, 2) + Mathf.Pow(_endPos.y - _startPos.y, 2), (1/2f));
        Vector2 angleVector = (_endPos - _startPos) / distanceBetween; //Represents change of x and y, then we normalize it to make it a base linear combination
        DebugAngleVec = angleVector;

        float angleOfElevation = Mathf.Rad2Deg * Mathf.Atan(angleVector.y / angleVector.x);
        angleOfElevation = (angleVector.x < 0) ? angleOfElevation + 180: angleOfElevation;

        Vector3 fixedAngleVecQuat = new Vector3(angleVector.x, angleVector.y, 1);
        Quaternion quaternionFeed = Quaternion.Euler(0, 0, angleOfElevation);


        //Since the two above are now RAW values, we have the RAW angle and RAW distance
        //We use the raw angle to add to the original start position

        Vector2 offsettedStart = _startPos + _offset * angleVector; //Offset * angle vector = aimed linear combination to add 
        float offsetDistanceBetween = distanceBetween - _offset; //Also fix a correct offsetDistanceBetween for correct usage

        debugPosition = offsettedStart;

        //Just calc how much the distanceBetween can carry with segmentDimensions, allowing int casting to drop the decimal
        float calcNumOfSegments = distanceBetween / (segmentDimensions.x);
        int totalNumOfSegments = (int)Math.Clamp(calcNumOfSegments, 0, maxArrowSegments);

        //Using the list, start from to 0 to the totalNumOfSegments, placing each one next to each other
        drawSegments(offsettedStart, _endPos, angleVector, quaternionFeed, totalNumOfSegments, offsetDistanceBetween, 2);
        drawHead(offsettedStart, _endPos, angleVector, quaternionFeed, totalNumOfSegments, offsetDistanceBetween, 1);
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
    private Vector2 debugPosition = new Vector2(0,0);
    private void OnDrawGizmos() {
        UnityEngine.Vector3 cubeDebugSize = new UnityEngine.Vector3(0.1f,0.1f,0.1f);
        Gizmos.DrawCube(debugPosition, cubeDebugSize);
    }
}