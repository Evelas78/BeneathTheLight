using UnityEngine;

public class scr_camSpot
{
    public Vector2 velocity = new Vector2();
    public Vector2 targetPos;
    public void InstantiateCamSpot(Vector2 _targetPos)
    {
        targetPos = _targetPos;
    }
    public void updatePosition()
    {

    }
}