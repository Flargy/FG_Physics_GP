using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float jumpStrength = 10.0f;
    
    
    private Rigidbody2D rbd2d;
    private Vector2 movementVector;
    private bool jump = false;
    
    private void Awake()
    {
        rbd2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        } 
        
        if (Input.GetKey(KeyCode.D))
        {
            movementVector = (new Vector2(movementSpeed, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementVector =(new Vector2(-movementSpeed, 0));
        }
    }

    private void FixedUpdate()
    {
        if (!Mathf.Approximately(movementVector.magnitude, 0.0f))
        {
            rbd2d.AddForce(movementVector * Time.fixedDeltaTime);
            movementVector = Vector2.zero;
        }

        if (jump)
        {
            rbd2d.AddForce(new Vector2(0f, jumpStrength), ForceMode2D.Impulse);
            jump = false;
        }

    }
}
