using System.Collections;
using System.Collections.Generic;
using UnityEditor.Scripting;
using UnityEngine;

[CreateAssetMenu(menuName = "States/JumpingState")]
public class JumpingState : BaseState
{
    [SerializeField] private float airMovementSpeed = 5.0f;
    [SerializeField] private float jumpStrength = 15.0f;
    
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
        body.velocity += (Vector2)bodyTransform.up * jumpStrength;
    }

    public override void OnUpdate()
    {
        float direction = Vector2.Dot(body.velocity, (Vector2)bodyTransform.up);
        if (direction <= 0.0f)
        {
            Owner.ChangeState(StateEnums.FALLING);
        }
        HandleInput();

        if (ConnectedWithWall())
        {
            Owner.ChangeState(StateEnums.WALLCLIMBING);
        }
    }

    public override void OnFixedUpdate()
    {
        MovePlayer();
    }

    public override void OnExit()
    {
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            movementInput = bodyTransform.right * airMovementSpeed;
            player.direction = StateMovementDiraction.RIGHT;

        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput = -bodyTransform.right * airMovementSpeed;
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
