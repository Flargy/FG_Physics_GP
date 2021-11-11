using UnityEngine;

[CreateAssetMenu(menuName = "States/WallJumpingState")]
public class WallJumpingState : BaseState
{
    [SerializeField] private float noInputHorizontalStrength = 5.0f;
    [SerializeField] private float withInputHorizontalStrength = 15.0f;
    [SerializeField] private float withInputVerticalStrength = 20.0f;
    [SerializeField] private float wallJumpInputSpeed = 5.0f;
    
    private Rigidbody2D body;
    private Vector2 movementInput = Vector2.zero;
    private PlayerStateMachine player;
    private Transform bodyTransform;
    private BoxCollider2D collider;
    private float raycastLength;

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
        Vector2 jumpVelocity;
        if (Vector2.Dot(player.wallInputDirection, player.wallNormal) < 0.8f)
        {
            jumpVelocity = player.wallNormal * noInputHorizontalStrength;
            body.velocity += jumpVelocity;
            return;
        }

        jumpVelocity = player.wallInputDirection * withInputHorizontalStrength +
                        (Vector2)bodyTransform.up * withInputVerticalStrength;
        body.velocity += jumpVelocity;
    }

    public override void OnUpdate()
    {
        float direction = Vector2.Dot(body.velocity, (Vector2)bodyTransform.up);
        
        HandleInput();
        
        if (ConnectedWithWall())
        {
            Owner.ChangeState(StateEnums.WALLCLIMBING);
        }
        
        if (direction <= 0.0f)
        {
            Owner.ChangeState(StateEnums.FALLING);
        }
    }

    public override void OnFixedUpdate()
    {
        MovePlayer();
    }

    public override void OnExit()
    {
    }
    
    private void MovePlayer()
    {
        body.velocity += movementInput * (Time.fixedDeltaTime);
        movementInput = Vector2.zero;
    }
    
    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            movementInput = bodyTransform.right * wallJumpInputSpeed;
            player.direction = StateMovementDiraction.RIGHT;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput = -bodyTransform.right * wallJumpInputSpeed;
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
