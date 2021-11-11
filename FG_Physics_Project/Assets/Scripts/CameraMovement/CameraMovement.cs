using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    /*
     * Things that are needed
     * a top, bottom, right and left markers that the camera wont be able to pass
     * a smoothing of movement dependant on the players position relative to the camera center
     * the marker points will need to follow the cameras rotation to make sure the aspect ration remains consistent
     * the marker points could be children to the background to ensure they rotate with the camera without moving with it
     */

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float smoothing = 2.0f;
    [SerializeField] private List<Transform> blockers;

    private Camera camera;
    private Vector2 playerScreenPosition;
    private bool playerNearHorizontal = false;
    private bool playerNearVertical = false;
    
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        playerScreenPosition = camera.WorldToViewportPoint(playerTransform.position);

        playerNearHorizontal = playerScreenPosition.x < 0.3f || playerScreenPosition.x > 0.7f;
        playerNearVertical = playerScreenPosition.y < 0.3f || playerScreenPosition.y > 0.7f;

        if (playerNearHorizontal)
        {
            Debug.Log("player near horizontal");
        }

        if (playerNearVertical)
        {
            Debug.Log("player near vertical");
        }
        // this works. just fix a movement smoothing with a maximum allowed movement area for vertical and horizontal movement
    }
}
