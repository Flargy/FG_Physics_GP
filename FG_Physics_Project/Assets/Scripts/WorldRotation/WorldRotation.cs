using Unity.VisualScripting;
using UnityEngine;

public class WorldRotation : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField, Range(0.1f, 100)] private float rotationTime;
    [SerializeField, Range(0.0f, 1.0f)] private float gravitySwapCutoff = 0.7f;
    [SerializeField] private Transform player;
    
     private Quaternion rotationDegrees = new Quaternion(0, 0, 0.7071f, 0.7071f);
     private Quaternion desiredRotation = Quaternion.identity;
     private Quaternion previousRotation = Quaternion.identity;
     private float timeAccumulator = 0.0f;
     private Vector3 gravityVector = Vector3.down;
     private bool hasFlipped = false;
    
    void Start()
    {
        desiredRotation = camera.rotation * rotationDegrees;
        previousRotation = camera.rotation;

    }

    private void Update()
    {
        timeAccumulator += Time.deltaTime / rotationTime;

        camera.rotation = Quaternion.Slerp(previousRotation, desiredRotation, timeAccumulator);

        if (timeAccumulator >= gravitySwapCutoff && hasFlipped == false)
        {
            Physics2D.gravity = desiredRotation * Physics.gravity;
            gravityVector = Physics.gravity;
            hasFlipped = true;
            player.rotation = desiredRotation;
            player.gameObject.GetComponent<RotatingPlayer>().DetachFromWall();
        }

        if (timeAccumulator >= 1.0f)
        {
            timeAccumulator = 0.0f;
            previousRotation = desiredRotation;
            desiredRotation = desiredRotation * rotationDegrees;
            hasFlipped = false;
        }
    }
    
}
