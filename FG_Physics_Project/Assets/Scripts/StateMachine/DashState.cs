using UnityEngine;

[CreateAssetMenu(menuName = "States/DashState")]
public class DashState : BaseState
{
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashDistance = 3.0f;
    
    private Rigidbody2D body;
    private Vector2 movementInput = Vector2.zero;
    private PlayerStateMachine player;
    private Transform bodyTransform;
    private BoxCollider2D collider;
    private float raycastLength;
    private float linearDrag;
    private float timeAccumulator = 0;
    
    public override void Initialize(StateMachine NewOwner)
    {
        Owner = NewOwner;
        body = NewOwner.GetPlayerRB();
        player = Owner.GetPlayer();
        bodyTransform = player.gameObject.GetComponent<Transform>();
        collider = body.gameObject.GetComponent<BoxCollider2D>();
        raycastLength = collider.bounds.extents.magnitude + 0.01f;
    }

    public override void OnEnter()
    {
        body.gravityScale = 0;
        linearDrag = body.drag;
        body.drag = 0.0f;
        player.canDash = false;
        body.velocity = Vector2.zero;
        CalculateDashValues();
    }

    public override void OnUpdate()
    {
        if (ConnectedWithWall())
        {
            Owner.ChangeState(StateEnums.WALLCLIMBING);
        }
        
        DashTimer();
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnExit()
    {
        body.gravityScale = player.baseGravity;
        body.drag = linearDrag;
        timeAccumulator = 0.0f;
        body.velocity *= 0.5f;
    }

    private void CalculateDashValues()
    {
        Vector2 dashDirection = Vector2.zero;
        
        if (Input.GetKey(KeyCode.D))
        {
            dashDirection = bodyTransform.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dashDirection = -bodyTransform.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            dashDirection += (Vector2) bodyTransform.up;
        }
        dashDirection.Normalize();

        float dashVelocity = dashDistance / dashDuration;
        body.velocity = dashDirection * dashVelocity;

    }

    private void DashTimer()
    {
        if (timeAccumulator >= dashDuration)
        {
            Owner.ChangeState(StateEnums.FALLING);
        }

        timeAccumulator += Time.deltaTime;
    }
    
    private bool ConnectedWithWall()
    {
        Vector2 movementDirection;
        if (player.direction == StateMovementDiraction.RIGHT)
        {
            movementDirection = bodyTransform.right;
        }
        else if (player.direction == StateMovementDiraction.LEFT)
        {
            movementDirection = -bodyTransform.right;
        }
        else
        {
            movementDirection = Vector2.zero;
        }

        RaycastHit2D hit;

        hit = Physics2D.Raycast(bodyTransform.position, movementDirection.normalized, raycastLength,
            LayerMask.GetMask("Default"));

        if (hit)
        {
            player.wallNormal = hit.normal;
            return true;
        }

        return false;
    }
}
