using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputDirection
{
    Right,
    Left,
    None
}

public class RotatingPlayer : MonoBehaviour
{
    
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float jumpStrength = 20.0f;
    [SerializeField] private float detachStrength = 30.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float airControl = 0.3f;
    [SerializeField] private AnimationCurve jumpCurve; // work in progress
    [SerializeField] private bool autoAttachOnRotation = false;

    private Vector2 movementInput;
    private Vector2 jumpVelocity;
    private Vector2 wallNormal;
    private Vector2 movementDirection;
    private BoxCollider2D collider;
    private Rigidbody2D body;
    private float gravityScale;
    private float controlModifier = 1.0f;

    private bool grounded = false;
    private bool isAttachedToWall = false;
    private InputDirection inputDirection = InputDirection.None;
    private bool canAttachToWall = true;
    


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        gravityScale = body.gravityScale;
    }
    
    void Update()
    {
        TestGrounded();
        HandleInput();
        TestWallConnection();
        MovePlayer();
    }

    void TestGrounded()
    {
        Debug.DrawRay(transform.position, -transform.up * 10, Color.red);
        if (Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y), collider.size * transform.localScale,
            0, -transform.up, 0.3f, LayerMask.GetMask("Default")))
        {
            grounded = true;
            return;
        }
        grounded = false;
    }
    
    private void HandleInput()
    {
        if (!isAttachedToWall)
        {
            if (Input.GetKey(KeyCode.D))
            {
                movementInput = transform.right * movementSpeed;
                inputDirection = InputDirection.Right;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                movementInput = -transform.right * movementSpeed;
                inputDirection = InputDirection.Left;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded || isAttachedToWall)
            {
                jumpVelocity = transform.up * jumpStrength;
                jumpVelocity += wallNormal * detachStrength;
                DetachFromWall();
            }
        }
        Vector2.ClampMagnitude(movementInput, movementSpeed);
    }

    public void DetachFromWall()
    {
        isAttachedToWall = false;
        wallNormal = Vector2.zero;
        body.gravityScale = gravityScale;
        movementDirection = Vector2.zero;
        canAttachToWall = false;
        StartCoroutine(AttachToWallDelay());
        if (!autoAttachOnRotation)
        {
            inputDirection = InputDirection.None;
        }

    }

    private void MovePlayer()
    {
        controlModifier = grounded ? 1 : airControl;
        body.velocity += jumpVelocity + movementInput * (Time.deltaTime * controlModifier);
        jumpVelocity = Vector2.zero;
        movementInput = Vector2.zero;
    }

    private void TestWallConnection()
    {
        if (grounded || !canAttachToWall)
        {
            return;
        }

        if (inputDirection == InputDirection.None)
        {
            movementDirection = Vector2.zero;
        }
        else if (inputDirection == InputDirection.Right)
        {
            movementDirection = transform.right;
        }
        else
        {
            movementDirection = -transform.right;
        }
        
        Debug.DrawRay(transform.position,  movementDirection.normalized * 0.4f, Color.green);
        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),
            movementDirection.normalized, 0.3f, LayerMask.GetMask("Default"));
        
        if(hit.collider)
        {
            wallNormal = hit.normal;
            body.velocity = new Vector2(body.velocity.x, body.velocity.y) * 0.98f;
            body.gravityScale = 0;
            isAttachedToWall = true;
        }
        else
        {
            DetachFromWall();
        }
       
    }

    private IEnumerator AttachToWallDelay()
    {
        yield return new WaitForSeconds(0.2f);
        canAttachToWall = true;
    }
    
    


    
}
