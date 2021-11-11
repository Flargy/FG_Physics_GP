using UnityEngine;

[CreateAssetMenu(menuName = "States/FallingState")]
public class FallingState : BaseState
{
    [SerializeField] private float fallingMoveSpeed = 5.0f;
    [SerializeField] private float gravityAcceleration = 1.5f;
    [SerializeField] private float maxVelocity = 20.0f;

    
    private Vector2 movementInput = Vector2.zero;
    private BoxCollider2D collider;
    private Transform bodyTransform;
    private Rigidbody2D body;
    private PlayerStateMachine player;
    private float raycastLength;

    public override void Initialize(StateMachine NewOwner)
    {
        Owner = NewOwner;
        body = NewOwner.GetPlayerRB();
        bodyTransform = body.transform;
        collider = body.gameObject.GetComponent<BoxCollider2D>();
        player = Owner.GetPlayer();
        raycastLength = collider.bounds.extents.magnitude + 0.01f;

    }

    public override void OnEnter()
    {
        body.gravityScale *= gravityAcceleration;
    }

    public override void OnUpdate()
    {
        HandleInput();
        float direction = Vector2.Dot(body.velocity, (Vector2)bodyTransform.up);

        if (ConnectedWithWall())
        {
            Owner.ChangeState(StateEnums.WALLCLIMBING);
        }
        
        if (IsGrounded())
        {
            Owner.ChangeState(StateEnums.WALKING);
        }
    }

    public override void OnFixedUpdate()
    {
        MovePlayer();
    }

    public override void OnExit()
    {
        body.gravityScale = player.baseGravity;
    }
    
    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            movementInput = bodyTransform.right * fallingMoveSpeed;
            player.direction = StateMovementDiraction.RIGHT;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput = -bodyTransform.right * fallingMoveSpeed;
            player.direction = StateMovementDiraction.LEFT;
        }
        else
        {
            player.direction = StateMovementDiraction.NONE;
        }
        if (Input.GetKeyDown(KeyCode.K) && player.canDash)
        {
            Owner.ChangeState(StateEnums.DASH);
        }
    }

    private void MovePlayer()
    {
        body.velocity += movementInput * (Time.fixedDeltaTime);
        movementInput = Vector2.zero;
        body.velocity = Vector2.ClampMagnitude(body.velocity, maxVelocity);
    }
    
    private bool IsGrounded()
    {
        // this works as a safety for avoiding wall collisions being seen as negative
        if (Physics2D.Raycast(bodyTransform.position, -bodyTransform.up, collider.bounds.extents.magnitude *1.05f, LayerMask.GetMask("Default")))
        {
            return true;
        }
        RaycastHit2D hit = Physics2D.BoxCast(bodyTransform.position,
            collider.size * (bodyTransform.localScale + Vector3.one * 0.05f),
            0, -bodyTransform.up, 0.05f, LayerMask.GetMask("Default"));
        
        Vector2 directionToHit = (hit.point - (Vector2)bodyTransform.position).normalized;
        if (hit && Vector2.Dot(directionToHit, -(Vector2)bodyTransform.up) > 0.1f)
        {
            return true;
        }

        return false;
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