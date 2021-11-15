using UnityEngine;

[CreateAssetMenu(menuName = "States/WallClimbState")]
public class WallClimbState : BaseState
{
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
        player = (PlayerStateMachine)Owner.GetPlayer();
        raycastLength = collider.bounds.extents.magnitude + 0.01f;
    }

    public override void OnEnter()
    {
        body.gravityScale = 0.0f;
        player.canDash = true;
    }

    public override void OnUpdate()
    {
        HandleInput();
        
        if (!ConnectedWithWall())
        {
            Owner.ChangeState(StateEnums.FALLING);
        }
        
        body.velocity *= 0.98f; // this is lacking delta time for now
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnExit()
    {
        body.gravityScale = player.baseGravity;
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            player.wallInputDirection = bodyTransform.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            player.wallInputDirection = -bodyTransform.right;
        }
        else
        {
            player.wallInputDirection = Vector2.zero;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Owner.ChangeState(StateEnums.WALLJUMPING);
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
            return true;
        }

        return false;

    }
}
