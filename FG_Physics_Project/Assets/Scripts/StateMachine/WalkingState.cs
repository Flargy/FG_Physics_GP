using System.Collections;
using System.Collections.Generic;
using UnityEditor.Scripting;
using UnityEngine;

[CreateAssetMenu(menuName = "States/WalkingState")]
public class WalkingState : BaseState
{
    [SerializeField] private float movementSpeed = 20.0f;
    
    private Vector2 movementInput = Vector2.zero;
    private BoxCollider2D collider;
    private Transform bodyTransform;
    private Rigidbody2D body;
    private PlayerStateMachine player;

    public override void Initialize(StateMachine NewOwner)
    {
        Owner = NewOwner;
        body = NewOwner.GetPlayerRB();
        bodyTransform = body.transform;
        collider = body.gameObject.GetComponent<BoxCollider2D>();
        player = Owner.GetPlayer();
        player.baseGravity = body.gravityScale;
    }

    public override void OnEnter()
    {
        Owner.GetPlayer().canDash = true;
        if (body == null)
        {
            body = Owner.GetPlayerRB();
        }

    }

    public override void OnUpdate()
    {
        HandleInput();
        if (!IsGrounded())
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

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            movementInput = bodyTransform.right * movementSpeed;
            player.direction = StateMovementDiraction.RIGHT;

        }
        else if (Input.GetKey(KeyCode.A))
        {
            movementInput = -bodyTransform.right * movementSpeed;
            player.direction = StateMovementDiraction.LEFT;
        }
        else
        {
            player.direction = StateMovementDiraction.NONE;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Owner.ChangeState(StateEnums.JUMPING);
        }
        
        
    }

    private void MovePlayer()
    {
        body.velocity += movementInput * (Time.fixedDeltaTime);
        movementInput = Vector2.zero;
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
}
