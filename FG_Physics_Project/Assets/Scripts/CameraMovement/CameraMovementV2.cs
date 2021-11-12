using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementV2 : MonoBehaviour
{
    
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float smoothingSpeed = 0.3f;
    
    private Camera camera;
    private Vector2 playerScreenPosition;
    private float cameraZ;

    void Start()
    {
        camera = GetComponent<Camera>();
        cameraZ = transform.position.z;
    }

    void Update()
    {
        playerScreenPosition = camera.WorldToViewportPoint(playerTransform.position);
        bool playerNearEdge = playerScreenPosition.x < 0.3f 
                              || playerScreenPosition.x > 0.7f
                              || playerScreenPosition.y < 0.3f 
                              || playerScreenPosition.y > 0.7f;

        if (playerNearEdge)
        {
            float distanceMultiplier = Vector2.Distance(playerTransform.position, transform.position);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position,
                new Vector3(playerTransform.position.x, playerTransform.position.y, cameraZ),
                smoothingSpeed * Time.deltaTime * distanceMultiplier);
            transform.position = smoothedPosition;
        }
    }
}
