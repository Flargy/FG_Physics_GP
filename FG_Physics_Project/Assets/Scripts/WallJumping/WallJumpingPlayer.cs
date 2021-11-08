using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpingPlayer : MonoBehaviour
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
        HandleMouseInput();
        MovePlayer();
        if (isAttached)
        {
            TestConnectionBreak();
            UpdateRope();
        }
        
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

    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isAttached)
            {
                linePoints.Clear();
                lineRenderer.positionCount = 0;
                isAttached = false;
                return;
            }

            Vector2 bodyTransform = transform.position;
            
            linePoints.Clear();
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (clickPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, direction, 500.0f, LayerMask.GetMask("Default"));

            if (hit)
            {
                Vector2 cornerHit = hit.collider.gameObject.GetComponent<RopeBox>().GetClosestCorner(hit.point);

                linePoints.Add(cornerHit);
                linePoints.Add(bodyTransform);
                isAttached = true;
                lineRenderer.positionCount = linePoints.Count;
                for (int i = 0 ; i < linePoints.Count ; i++)
                {
                    lineRenderer.SetPosition(i, linePoints[i]);
                }
                
            }
        }
    }

    private void TestConnectionBreak()
    {
        if (linePoints.Count < 3)
        {
            return;
        }
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        float sign = (line[1].x - line[0].x) * (pos.y - line[0].y) - (line[1].y - line[0].y) * (pos.x - line[0].x);

        if (lookForPositive)
        {
            if (sign > 0)
            {
                BreakRope();
            }
        }
        else
        {
            if (sign < 0)
            {
                BreakRope();
            }
        }
        
    }

    private void BreakRope()
    {
        linePoints.RemoveAt(linePoints.Count - 2);
        lineRenderer.positionCount = lineRenderer.positionCount - 1;

        if (linePoints.Count >= 3)
        {
            CalculateLine();
        }
    }

    private void UpdateRope()
    {
        Vector2 vectorToPlayer = new Vector2(transform.position.x, transform.position.y) - linePoints[linePoints.Count - 2];
        float distanceToPlayer = vectorToPlayer.magnitude;
        Vector2 direction = vectorToPlayer.normalized;
        RaycastHit2D hit;

        float length = 0;

        for (int i = 1; i < linePoints.Count; i++)
        {
            length += Vector2.Distance(linePoints[i - 1], linePoints[i]);
        }

        currentRopeLength = length;
        
        
        hit = Physics2D.Raycast(linePoints[linePoints.Count - 2], direction, distanceToPlayer, LayerMask.GetMask("Default"));
        
        if (hit)
        {
            Vector2 cornerHit = hit.collider.gameObject.GetComponent<RopeBox>().GetClosestCorner(hit.point);

            if (cornerHit != linePoints[linePoints.Count - 2])
            {
                linePoints.RemoveAt(linePoints.Count - 1);
                linePoints.Add(cornerHit);
                linePoints.Add(transform.position);
                lineRenderer.positionCount = linePoints.Count;
                for (int i = 0 ; i < linePoints.Count ; i++)
                {
                    lineRenderer.SetPosition(i, linePoints[i]);
                }
            }
            

            CalculateLine();

        }
        
        lineRenderer.SetPosition(linePoints.Count - 1, transform.position);
    }

    private void CalculateLine()
    {
        line[0] = linePoints[linePoints.Count - 3];
        Vector2 dir = (linePoints[linePoints.Count - 2] - linePoints[linePoints.Count - 3]).normalized * 10000000;
        line[1] = dir;
            
        float sign = (line[1].x - line[0].x) * (transform.position.y - line[0].y) - (line[1].y - line[0].y) * (transform.position.x - line[0].x);

        lookForPositive = sign < 0;
    }
   
}
