using System.Collections.Generic;
using UnityEngine;

public class RotatingPlayer : MonoBehaviour
{
    
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float jumpStrength = 20.0f;
    [SerializeField] private float detachStrength = 30.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float airControl = 0.3f;

    private Vector2 movementInput;
    private Vector2 jumpVelocity;
    private BoxCollider2D collider;
    private Rigidbody2D body;
    private float gravityScale;
    private float wallDirection;
    private float controlModifier = 1.0f;

    private bool grounded = false;
    private bool isAttachedToWall = false;
    
    private bool isAttached = false;
    


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        gravityScale = body.gravityScale;

    }
    
    void Update()
    {
        TestGrounded();
        TestWallConnection();
        HandleInput();
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
        if (Input.GetKey(KeyCode.D))
        {
            movementInput = transform.right * movementSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput = -transform.right * movementSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded || isAttachedToWall)
            {
                jumpVelocity = transform.up * jumpStrength;
                jumpVelocity += new Vector2(transform.right.x, transform.right.y) * (-wallDirection * detachStrength);
                isAttachedToWall = false;
                wallDirection = 0;
            }
        }
        Vector2.ClampMagnitude(movementInput, movementSpeed);
    }

    private void MovePlayer()
    {
        controlModifier = grounded == true ? 1 : airControl;
        body.velocity += jumpVelocity + movementInput * (Time.deltaTime * controlModifier);
        jumpVelocity = Vector2.zero;
        movementInput = Vector2.zero;
    }

    private void TestWallConnection()
    {
        
        Debug.DrawRay(transform.position, body.velocity * 10, Color.green);

        float direction = wallDirection != 0 ? wallDirection : body.velocity.x;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),
            (transform.rotation * new Vector2(direction, 0)).normalized, 0.3f, LayerMask.GetMask("Default"));
        if(hit.collider)
        {
            if (wallDirection == 0)
            {
                wallDirection =  body.velocity.x > 0 ? 1 : -1;
            }
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.98f);
            body.gravityScale = 0;
            isAttachedToWall = true;
            return;
        }
        
        isAttachedToWall = false;
        wallDirection = 0;
        body.gravityScale = gravityScale;
    }


    
}
