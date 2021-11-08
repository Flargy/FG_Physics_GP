using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public struct BoxData
{
    public Vector2 connectionPoint;
    public Vector2 centerPoint;

    public BoxData(Vector2 connection, Vector2 center)
    {
        connectionPoint = connection;
        centerPoint = center;
    }
}

public class SwingingPlayer : MonoBehaviour
{

    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float maxRopeLength = 50.0f;

    private Vector2 movementInput;
    private Rigidbody2D body;
    private LineRenderer lineRenderer;
    private bool isAttached = false;

    private List<Vector2> linePoints = new List<Vector2>();
    private Vector2[] line = new Vector2[2];
    private bool lookForPositive = false;

    private float currentRopeLength = 0.0f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    
    void Update()
    {
        HandleInput();
        HandleMouseInput();
        if (isAttached)
        {
            TestConnectionBreak();
            UpdateRope();
        }
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

        Vector2.ClampMagnitude(movementInput, movementSpeed);
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
            hit = Physics2D.Raycast(transform.position, direction, 500.0f, LayerMask.GetMask("RopeLayer"));

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
        
        
        hit = Physics2D.Raycast(linePoints[linePoints.Count - 2], direction, distanceToPlayer, LayerMask.GetMask("RopeLayer"));
        
        if (hit)
        {
            Vector2 cornerHit = hit.collider.gameObject.GetComponent<RopeBox>().GetClosestCorner(hit.point);

            linePoints.RemoveAt(linePoints.Count - 1);
            linePoints.Add(cornerHit);
            linePoints.Add(transform.position);
            lineRenderer.positionCount = linePoints.Count;
            for (int i = 0 ; i < linePoints.Count ; i++)
            {
                lineRenderer.SetPosition(i, linePoints[i]);
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

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        
        body.velocity = (movementInput * Time.fixedDeltaTime);
        movementInput = Vector2.zero;
        
    }
}
