using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/NormalFallingState")]
public class NormalEnemyFallingState : BaseState
{
    [SerializeField] private float maximumFallSpeed = 6.0f;
    [SerializeField] private float fallingGravity = 5.0f;
    [SerializeField, Min(0.1f)] private float fullSpinDuration = 0.5f;
    
    private Rigidbody2D body;
    private Transform bodyTransform;
    private BoxCollider2D collider;
    private NormalEnemyStateMachine actor;

    private float currentMovement;
    private float raycastLength;
    
    public override void Initialize(StateMachine NewOwner)
    {
        Owner = NewOwner;
        body = Owner.GetPlayerRB();
        bodyTransform = body.transform;
        collider = body.gameObject.GetComponent<BoxCollider2D>();
        actor = (NormalEnemyStateMachine)Owner.GetPlayer();
        raycastLength = collider.size.x * 0.55f;
    }

    public override void OnEnter()
    {
        body.velocity = Vector2.zero;
        body.gravityScale = fallingGravity;
    }

    public override void OnUpdate()
    {
        SpinAround();
        body.velocity = Vector2.ClampMagnitude(body.velocity, maximumFallSpeed);
        if (actor.isDead)
        {
            return;
        }
        if (GroundCheck())
        {
            Owner.ChangeState(StateEnums.WALKING);
        }
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnExit()
    {
        body.velocity = Vector2.zero;
        bodyTransform.up = -Physics2D.gravity.normalized;
        actor.sprite.localRotation = Quaternion.identity;
    }
    
    public bool GroundCheck()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(bodyTransform.position, Physics2D.gravity.normalized, raycastLength, LayerMask.GetMask("Default"));

        if (hit)
        {
            return true;
        }
        
        hit = Physics2D.BoxCast(bodyTransform.position,
            collider.bounds.extents * 2,
            0, Physics2D.gravity.normalized,
            0.1f, LayerMask.GetMask("Default"));
        if (hit)
        {
            return true;
        }

        return false;
    }

    public void SpinAround()
    {
        actor.sprite.localRotation = Quaternion.Euler(0, 0, actor.sprite.localRotation.eulerAngles.z + 360 * Time.deltaTime / fullSpinDuration);
    }
}
