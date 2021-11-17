using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementV2 : MonoBehaviour
{
    
    [SerializeField] private Transform playerTransform;
    private float smoothingSpeed = 0.5f;
    [SerializeField, Range(0.1f, 0.9f)] private float movementRange = 0.3f;
    
    private float multiplier = 0.15f;
    private Camera camera;
    private Vector2 playerScreenPosition;
    private float cameraZ;
    private bool currentlyMoving = false;

    void Start()
    {
        camera = GetComponent<Camera>();
        cameraZ = transform.position.z;
    }

    void Update()
    {
        playerScreenPosition = camera.WorldToViewportPoint(playerTransform.position);
        
        bool playerNearEdge = playerScreenPosition.x < 0 + movementRange
                              || playerScreenPosition.x > 1 - movementRange
                              || playerScreenPosition.y < 0 + movementRange
                              || playerScreenPosition.y > 1 - movementRange;

        if (playerNearEdge || currentlyMoving)
        {
            currentlyMoving = true;
            float distanceMultiplier = multiplier * Vector2.Distance(playerTransform.position, transform.position);
            
            Vector3 smoothedPosition = Vector3.Lerp(transform.position,
                new Vector3(playerTransform.position.x, playerTransform.position.y, cameraZ),
                smoothingSpeed * Time.deltaTime * distanceMultiplier);
            transform.position = smoothedPosition;
            
            bool stopMoving = playerScreenPosition.x < 0.6
                              && playerScreenPosition.x > 0.4
                              && playerScreenPosition.y < 0.6
                              && playerScreenPosition.y > 0.4;
            if (stopMoving)
            {
                currentlyMoving = false;
            }
        }
    }
}
