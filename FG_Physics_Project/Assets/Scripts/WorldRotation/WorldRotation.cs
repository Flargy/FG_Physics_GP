using System;
using System.Collections;
using System.Collections.Generic;
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
    private static WorldRotation instance = null;

    public static WorldRotation Instance
    {
        get { return instance; }
    }
    
    [SerializeField] private Transform camera;
    [SerializeField, Range(0.1f, 100)] private float rotationTime;
    [SerializeField, Range(0.0f, 1.0f)] private float gravitySwapCutoff = 0.7f;
    [SerializeField] private float rotationDelay = 5.0f;
    [SerializeField] private float rotationDelayMax = 10.0f;
    [SerializeField] private bool useRandomRange = false;
    [SerializeField] private Transform player;
    [SerializeField] private RotationType rotationType = RotationType.FullRotation;
    [SerializeField] private Transform background;
    
     private Quaternion rotationDegrees = new Quaternion(0, 0, 0.7071f, 0.7071f);
     private Quaternion negativeRotationDegrees = new Quaternion(0, 0, -0.7071f, 0.7071f);

     private Quaternion desiredRotation = Quaternion.identity;
     private Quaternion previousRotation = Quaternion.identity;
     private float timeAccumulator = 0.0f;
     private bool hasFlipped = false;

     private bool isRotating = false;
     private bool goPositive = true;
     private float randomDelay;
     private float countDown;

     private List<NormalEnemyStateMachine> enemyList = new List<NormalEnemyStateMachine>();

     private void Awake()
     {
         if (instance == null)
         {
             instance = this;
         }
     }

     void Start()
    {
        desiredRotation = camera.rotation * rotationDegrees;
        previousRotation = camera.rotation;
        randomDelay = Mathf.Max(rotationDelay, rotationDelayMax);
        countDown = rotationDelay;
        if (useRandomRange)
            countDown = randomDelay;
        
    }

    private void Update()
    {
        if (isRotating)
        {
            return;
        }

        if (!useRandomRange)
            timeAccumulator += Time.deltaTime / rotationDelay;
        else
            timeAccumulator += Time.deltaTime / randomDelay;

        countDown -= Time.deltaTime;
        
        UI_Manager.Instance.SetTimerBar(1-timeAccumulator);
        UI_Manager.Instance.SetTimerText((int)(countDown + 0.55f));

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
            //background.rotation = Quaternion.Slerp(previousRotation, desiredRotation, rotation);

            if (rotation >= gravitySwapCutoff && hasFlipped == false)
            {
                Physics2D.gravity = desiredRotation * Physics.gravity;
                hasFlipped = true;
                player.rotation = desiredRotation;
                foreach (NormalEnemyStateMachine enemy in enemyList)
                {
                    enemy.Rotate();
                }
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
                desiredRotation = desiredRotation * rotationDegrees;
            }
            else
            {
                desiredRotation = desiredRotation * negativeRotationDegrees;
            }

        }

        hasFlipped = false;
        isRotating = false;
        countDown = rotationDelay;
        if (useRandomRange)
        {
            randomDelay = Mathf.Max(rotationDelay, rotationDelayMax);
            countDown = randomDelay;
        }

    }

    public void RegisterEnemy(NormalEnemyStateMachine enemy)
    {
        enemyList.Add(enemy);
    }

    public void UnregisterEnemy(NormalEnemyStateMachine enemy)
    {
        if (enemyList.Contains(enemy))
        {
            enemyList.Remove(enemy);
        }
    }
    
}
