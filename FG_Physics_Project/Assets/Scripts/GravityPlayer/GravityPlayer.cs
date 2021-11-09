using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlayer : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float circularForce = 5.0f;
    [SerializeField] private float impulseForce = 500;
    
    private Vector2 movementInput;
    private Rigidbody2D body;
    private bool useGravity = true;
    private List<Rigidbody2D> affectedBodies = new List<Rigidbody2D>();
    
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        HandleInput();
        HandleMouseInput();
        MovePlayer();
        if(useGravity)
            AffectGravityObjects();
    }

    private void AffectGravityObjects()
    {
        
        foreach (Rigidbody2D affectedBody in affectedBodies)
        {
            Vector2 centripedalForceDirection =
                (transform.position - affectedBody.gameObject.transform.position).normalized;
            Vector2 tangetVelocityDirection = GetPerpendicularVelocity(centripedalForceDirection);
            affectedBody.velocity = body.velocity + tangetVelocityDirection * 1;
            


        }
    }

    private Vector2 GetPerpendicularVelocity(Vector2 vector)
    {
        return (new Vector2(vector.y, -vector.x)).normalized;
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            movementInput += Vector2.right * movementSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput += -Vector2.right * movementSpeed;
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            movementInput += Vector2.up * movementSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movementInput += -Vector2.up * movementSpeed;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(useGravity)
                EmmitPulse();
        }
    }

    private void EmmitPulse()
    {
        StartCoroutine(StopGravity());
        foreach (Rigidbody2D affectedBody in affectedBodies)
        {
            Vector2 forceDirection = (affectedBody.transform.position - transform.position).normalized;
            affectedBody.AddForce(forceDirection * impulseForce);
        }
    }

    private IEnumerator StopGravity()
    {
        useGravity = false;
        yield return new WaitForSeconds(1);
        useGravity = true;
    }
    
    private void MovePlayer()
    {
        body.velocity = (movementInput * Time.fixedDeltaTime);
        movementInput = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D otherBody = other.gameObject.GetComponent<Rigidbody2D>();
        if (otherBody)
        {
            affectedBodies.Add(otherBody);
            otherBody.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D otherBody = other.gameObject.GetComponent<Rigidbody2D>();
        if (otherBody)
        {
            affectedBodies.Remove(otherBody);
            otherBody.gravityScale = 1;
        }
        
    }
}
