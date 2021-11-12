using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    /*
     * The current problem is that the calculations in this doesnt take the rotation into account
     * vertical and horizontal swaps position with 90 degree turns
     * 
     */

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float smoothingSpeed = 0.3f;
    [SerializeField] private List<Transform> horizontalBlockers;
    [SerializeField] private List<Transform> verticalBlockers;
    [SerializeField] private float blockerMargin = 0.2f;

    private Camera camera;
    private Vector2 playerScreenPosition;
    private bool playerNearHorizontal = false;
    private bool playerNearVertical = false;
    private float cameraZ;

    private enum CameraMovementDirection
    {
        RIGHT,
        LEFT,
        UP,
        DOWN
    }

    private CameraMovementDirection horizontalMovement;
    private CameraMovementDirection verticalMovement;
    
    void Start()
    {
        camera = GetComponent<Camera>();
        cameraZ = transform.position.z;
    }

    void LateUpdate()
    {
        playerScreenPosition = camera.WorldToViewportPoint(playerTransform.position);
        playerNearHorizontal = playerScreenPosition.x < 0.3f || playerScreenPosition.x > 0.7f;
        playerNearVertical = playerScreenPosition.y < 0.3f || playerScreenPosition.y > 0.7f;

        if (playerNearHorizontal)
        {
            horizontalMovement = playerScreenPosition.x < 0.3f
                ? CameraMovementDirection.LEFT
                : CameraMovementDirection.RIGHT;
            
            MoveHorizontal();
        }

        if (playerNearVertical)
        {
            verticalMovement = playerScreenPosition.y < 0.3f
                ? CameraMovementDirection.DOWN
                : CameraMovementDirection.UP;
            MoveVertical();
        }
        
    }

    private void MoveHorizontal()
    {
        if (IsAtStopper(horizontalBlockers, true))
        {
            return;
        }

        Vector3 desiredPosition;
        if (horizontalMovement == CameraMovementDirection.RIGHT)
        {
            desiredPosition = horizontalBlockers[0].position.x > horizontalBlockers[1].position.x
                ? horizontalBlockers[0].position
                : horizontalBlockers[1].position;
        }
        else
        {
            desiredPosition = horizontalBlockers[0].position.x < horizontalBlockers[1].position.x
                ? horizontalBlockers[0].position
                : horizontalBlockers[1].position;
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position,
            new Vector3(desiredPosition.x, transform.position.y, cameraZ),
            smoothingSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    private void MoveVertical()
    {
        if (IsAtStopper(verticalBlockers, false))
        {
            return;
        }
        
        Vector3 desiredPosition;
        if (verticalMovement == CameraMovementDirection.UP)
        {
            desiredPosition = verticalBlockers[0].position.y > verticalBlockers[1].position.y
                ? verticalBlockers[0].position
                : verticalBlockers[1].position;
        }
        else
        {
            desiredPosition = verticalBlockers[0].position.y < verticalBlockers[1].position.y
                ? verticalBlockers[0].position
                : verticalBlockers[1].position;
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position,
            new Vector3(transform.position.x, desiredPosition.y, cameraZ),
            smoothingSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    private bool IsAtStopper(List<Transform> testingList, bool horizontal)
    {
        Vector2 topPos = camera.ViewportToWorldPoint(new Vector3(0.5f, 1, 0));
        Vector2 bottomPos = camera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0));
        Vector2 rightPos = camera.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        Vector2 leftPos = camera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));

        if (horizontal)
        {
            Vector2 testPosition = horizontalMovement == CameraMovementDirection.RIGHT ? rightPos : leftPos;

            foreach (Transform positions in testingList)
            {
                if (Mathf.Abs(testPosition.x - positions.position.x) < blockerMargin)
                {
                    return true;
                }
            }
        }
        else
        {
            Vector2 testPosition = verticalMovement == CameraMovementDirection.UP ? topPos : bottomPos;
            
            foreach (Transform positions in testingList)
            {
                if (Mathf.Abs(testPosition.y - positions.position.y) < blockerMargin)
                {
                    return true;
                }
            }
            
        }

        return false;
    }
}
