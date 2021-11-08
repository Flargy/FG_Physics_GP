using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBox : MonoBehaviour
{
    private BoxCollider2D collider;
    private Vector3 extents;
    private Vector2[] connectionPoints = new Vector2[4];

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        extents = collider.bounds.extents;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 center = collider.bounds.center;
        
        Vector2 extentDirection = new Vector2(extents.x, extents.y).normalized * 0.15f;
        connectionPoints[0] = new Vector2(extents.x, extents.y) + extentDirection + position;
        
        extentDirection = new Vector2(-extents.x, extents.y).normalized * 0.15f;
        connectionPoints[1] = new Vector2(-extents.x, extents.y) + extentDirection + position;
        
        extentDirection = new Vector2(-extents.x, -extents.y).normalized * 0.15f;
        connectionPoints[2] = new Vector2(-extents.x, -extents.y) + extentDirection + position;
        
        extentDirection = new Vector2(extents.x, -extents.y).normalized * 0.15f;
        connectionPoints[3] = new Vector2(extents.x, -extents.y) + extentDirection + position;
    }

    public Vector2 GetClosestCorner(Vector2 hitLocation)
    {
        Vector2 closestPoint = new Vector2(100000, 100000);
        foreach (Vector2 point in connectionPoints)
        {
            if (Vector2.Distance(hitLocation, point) < Vector2.Distance(hitLocation, closestPoint))
            {
                closestPoint = point;
            }
        }
        return closestPoint;
    }
    
}
