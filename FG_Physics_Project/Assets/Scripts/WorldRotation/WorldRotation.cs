using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum RotationType
{
    FullRotation,
    TwoSideRotation,
    ThreeSideRotation
    
}

public class WorldRotation : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField, Range(0.1f, 100)] private float rotationTime;
    [SerializeField, Range(0.0f, 1.0f)] private float gravitySwapCutoff = 0.7f;
    [SerializeField] private float rotationDelay = 5.0f;
    [SerializeField] private Transform player;
    [SerializeField] private RotationType rotationType = RotationType.FullRotation;
    
     private Quaternion rotationDegrees = new Quaternion(0, 0, 0.7071f, 0.7071f);
     private Quaternion negativeRotationDegrees = new Quaternion(0, 0, -0.7071f, 0.7071f);

     private Quaternion desiredRotation = Quaternion.identity;
     private Quaternion previousRotation = Quaternion.identity;
     private float timeAccumulator = 0.0f;
     private Vector3 gravityVector = Vector3.down;
     private bool hasFlipped = false;

     private bool isRotating = false;
     private bool goPositive = true;

     void Start()
    {
        desiredRotation = camera.rotation * rotationDegrees;
        previousRotation = camera.rotation;

    }

    private void Update()
    {
        if (isRotating)
        {
            return;
        }
        timeAccumulator += Time.deltaTime / rotationDelay;

        if (timeAccumulator >= 1.0f)
        {
            StartCoroutine(StartWorldRotation());
            timeAccumulator = 0.0f;
        }
    }

    private IEnumerator StartWorldRotation()
    {
        isRotating = true;
        float rotation = 0;
        while (rotation <= 1)
        {
            yield return null;
            rotation += Time.deltaTime / rotationTime;
            
            camera.rotation = Quaternion.Slerp(previousRotation, desiredRotation, rotation);


            if (rotation >= gravitySwapCutoff && hasFlipped == false)
            {
                Physics2D.gravity = desiredRotation * Physics.gravity;
                gravityVector = Physics.gravity;
                hasFlipped = true;
                player.rotation = desiredRotation;
                RotatingPlayer rotatingPlayer = player.gameObject.GetComponent<RotatingPlayer>();
                PlayerStateMachine playerStateMachine = player.GetComponent<PlayerStateMachine>();
                if (playerStateMachine)
                {
                    playerStateMachine.WorldRotation();
                }
                else
                {
                    rotatingPlayer.DetachFromWall();
                }
            }
            
        }

        if (rotationType == RotationType.FullRotation)
        {
            previousRotation = desiredRotation;
            desiredRotation = desiredRotation * rotationDegrees;
        }
        else if(rotationType == RotationType.TwoSideRotation)
        {
            previousRotation = desiredRotation;
            if(desiredRotation == Quaternion.identity)
                desiredRotation = desiredRotation * rotationDegrees;
            else
                desiredRotation = quaternion.identity;
        }
        else if (rotationType == RotationType.ThreeSideRotation)
        {
            previousRotation = desiredRotation;
            if (desiredRotation != quaternion.identity)
            {
                desiredRotation = quaternion.identity;
                goPositive = !goPositive;
            }
            else if (goPositive)
            {
                Debug.Log("positive");
                desiredRotation = desiredRotation * rotationDegrees;
            }
            else
            {
                Debug.Log("negative");
                desiredRotation = desiredRotation * negativeRotationDegrees;
            }

        }

        hasFlipped = false;
        isRotating = false;
    }
    
}
