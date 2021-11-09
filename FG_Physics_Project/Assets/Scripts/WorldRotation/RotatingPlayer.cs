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

    private LineRenderer lineRenderer;
    private bool grounded = false;
    private bool isAttachedToWall = false;
    
    private bool isAttached = false;

    private List<Vector2> linePoints = new List<Vector2>();
    private Vector2[] line = new Vector2[2];
    private bool lookForPositive = false;
    private float currentRopeLength = 0.0f;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        gravityScale = body.gravityScale;
        lineRenderer = GetComponent<LineRenderer>();

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
        if (Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y), collider.size * transform.localScale,
            0, Vector2.down, 0.3f, LayerMask.GetMask("Default")))
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
            movementInput = Vector2.right * movementSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput = -Vector2.right * movementSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded || isAttachedToWall)
            {
                jumpVelocity = Vector2.up * jumpStrength;
                jumpVelocity += Vector2.right * (-wallDirection * detachStrength);
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

        float direction = wallDirection != 0 ? wallDirection : body.velocity.x;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y),
            new Vector2(direction, 0).normalized, 0.3f, LayerMask.GetMask("Default"));
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
