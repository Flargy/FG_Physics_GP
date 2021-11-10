using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] private float dashDistance = 3.0f;
    [SerializeField] private float dashTime = 0.2f;

    private Vector2 movementInput;
    private Vector2 jumpVelocity;
    private Vector2 wallNormal;
    private Vector2 movementDirection;
    private BoxCollider2D collider;
    private Rigidbody2D body;
    private float gravityScale;
    private float controlModifier = 1.0f;
    private float raycastLength;
    private float linearDrag;
    private float angularDrag;

    private bool grounded = false;
    private bool isAttachedToWall = false;
    private InputDirection inputDirection = InputDirection.None;
    private bool canAttachToWall = true;
    private bool holdingRight = false;
    private bool holdingLeft = false;
    private bool holdingUp = false;
    private bool canDash = true;
    private bool isDashing = false;



    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        gravityScale = body.gravityScale;
        raycastLength = collider.bounds.extents.magnitude + 0.01f;
        linearDrag = body.drag;
        angularDrag = body.angularDrag;
        RespawnManager.Instance.RegisterPlayer(this);
    }
    
    void Update()
    {
        HandleInput();
        TestWallConnection();
        TestGrounded();

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void TestGrounded()
    {
        // this works as a safety for avoiding wall collisions being seen as negative
        if (Physics2D.Raycast(transform.position, -transform.up, collider.bounds.extents.magnitude *1.05f, LayerMask.GetMask("Default")))
        {
            grounded = true;
            RefreshDash();
            return;
        }
        
        RaycastHit2D hit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y),
            collider.size * transform.localScale,
            0, -transform.up, 0.3f, LayerMask.GetMask("Default"));
        
        Debug.DrawRay(transform.position, -transform.up * 10, Color.red);
        Vector2 directionToHit = (hit.point - (Vector2)transform.position).normalized;
        if (hit && Vector2.Dot(directionToHit, -(Vector2)transform.up) > 0.1f)
        {
            grounded = true;
            RefreshDash();
            return;
        }
        grounded = false;
    }
    
    private void HandleInput()
    {
        if (!isAttachedToWall && !isDashing)
        {
            if (Input.GetKey(KeyCode.D))
            {
                movementInput = transform.right * movementSpeed;
                inputDirection = InputDirection.Right;
                holdingRight = true;
                holdingLeft = false;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                movementInput = -transform.right * movementSpeed;
                inputDirection = InputDirection.Left;
                holdingLeft = true;
                holdingRight = false;
            }
            else
            {
                holdingRight = false;
                holdingLeft = false;
            }

            if (Input.GetKey(KeyCode.W))
            {
                holdingUp = true;
            }
            else
            {
                holdingUp = false;
            }
            
            if (Input.GetKeyDown(KeyCode.K) && canDash && !grounded)
            {
                Dash();
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

    private void Dash()
    {
        if (!canDash)
        {
            return;
        }

        canDash = false;
        body.velocity = Vector2.zero;
        Vector2 dashDirection = Vector2.zero;
        
        if (holdingUp)
            dashDirection += new Vector2(transform.up.x, transform.up.y);
        if (holdingRight)
            dashDirection += new Vector2(transform.right.x, transform.right.y);
        else if(holdingLeft)
            dashDirection += new Vector2(-transform.right.x, -transform.right.y);

        if (dashDirection == Vector2.zero)
        {
            dashDirection = transform.up;
        }

        isDashing = true;
        dashDirection.Normalize();
        body.drag = 0;
        body.angularDrag = 0;

        float dashVelocity = dashDistance / dashTime;
        body.velocity = dashDirection * dashVelocity;

        StartCoroutine(DashGravityRoutine());
    }

    public void RefreshDash()
    {
        canDash = true;
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
        body.velocity += jumpVelocity + movementInput * (Time.fixedDeltaTime * controlModifier);
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
        hit = Physics2D.Raycast(transform.position,
            movementDirection.normalized, raycastLength, LayerMask.GetMask("Default"));
        
        if(hit.collider)
        {
            wallNormal = hit.normal;
            body.velocity = new Vector2(body.velocity.x, body.velocity.y) * 0.98f;
            body.gravityScale = 0;
            isAttachedToWall = true;
        }
        else if(canDash)
        {
            DetachFromWall();
        }
       
    }

    public void Respawn(Vector2 position)
    {
        DetachFromWall();
        transform.position = position;
        body.velocity = Vector2.zero;
    }

    private IEnumerator AttachToWallDelay()
    {
        yield return new WaitForSeconds(0.05f);
        canAttachToWall = true;
    }

    private IEnumerator DashGravityRoutine()
    {
        body.gravityScale = 0;
        float timer = 0;
        bool abort = false;

        while (timer <= dashTime && abort == false)
        {
            yield return null;
            timer += Time.deltaTime;
            if (isAttachedToWall)
            {
                abort = true;
            }
        }
        body.drag = linearDrag;
        body.angularDrag = angularDrag;
        isDashing = false;
        body.velocity = body.velocity / 2;    
        if (!abort)
        {
            body.gravityScale = gravityScale;
        }
        
    }
    
}
