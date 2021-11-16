using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/NormalWalkingState")]
public class NormalEnemyWalkingState : BaseState
{
    [SerializeField] private float movementSpeed = 5.0f;
    
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
        currentMovement = movementSpeed;
        raycastLength = collider.size.x;
    }

    public override void OnEnter()
    {
        collider.isTrigger = true;
        body.gravityScale = 0;
    }

    public override void OnUpdate()
    {
        body.velocity += (Vector2) bodyTransform.right * currentMovement; // not using delta time cause this is cheap shit
        body.velocity = Vector2.ClampMagnitude(body.velocity, movementSpeed);

        RaycastHit2D hit;
        hit = Physics2D.Raycast(bodyTransform.position, body.velocity.normalized, raycastLength, LayerMask.GetMask("Default"));

        if (hit)
        {
            currentMovement *= -1;
            actor.FaceRight(currentMovement > 0);
        }

        if (!GroundCheck())
        {
            Owner.ChangeState(StateEnums.FALLING);
        }
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnExit()
    {
        collider.isTrigger = false;
    }

    public bool GroundCheck()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(bodyTransform.position, -bodyTransform.up, raycastLength, LayerMask.GetMask("Default"));

        if (hit)
        {
            return true;
        }
        
        hit = Physics2D.BoxCast(bodyTransform.position,
            collider.bounds.extents * 2,
            0, -bodyTransform.up,
            0.05f, LayerMask.GetMask("Default"));
        if (hit)
        {
            return true;
        }

        return false;
    }
}
