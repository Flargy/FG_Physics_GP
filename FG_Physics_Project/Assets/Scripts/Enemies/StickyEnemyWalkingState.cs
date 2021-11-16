using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/StickyWalkingState")]
public class StickyEnemyWalkingState : BaseState
{
    [SerializeField] private float movementSpeed = 2.5f;
    
    private Rigidbody2D body;
    private Transform bodyTransform;
    private BoxCollider2D collider;
    private StickyEnemyStateMachine actor;

    private float currentMovement;
    private float raycastLength;

    public override void Initialize(StateMachine NewOwner)
    {
        Owner = NewOwner;
        body = Owner.GetPlayerRB();
        bodyTransform = body.transform;
        collider = body.gameObject.GetComponent<BoxCollider2D>();
        actor = (StickyEnemyStateMachine)Owner.GetPlayer();
        currentMovement = movementSpeed;
        raycastLength = collider.size.x;
    }

    public override void OnEnter()
    {
        body.gravityScale = 0;
    }

    public override void OnUpdate() // change movement type later
    {
        body.velocity += (Vector2) bodyTransform.right * currentMovement; // not using delta time cause this is cheap shit
        body.velocity = Vector2.ClampMagnitude(body.velocity, movementSpeed);

        RaycastHit2D hit;
        hit = Physics2D.Raycast(bodyTransform.position, body.velocity.normalized, raycastLength, LayerMask.GetMask("Default"));

        if (hit || !GroundCheck())
        {
            currentMovement *= -1;
            actor.FaceRight(currentMovement > 0);
        }

    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnExit()
    {
    }

    private bool GroundCheck()
    {
        Vector2 pos = bodyTransform.position;
        if (currentMovement > 0)
        {
            pos += (Vector2)bodyTransform.right;
        }
        else
        {
            pos -= (Vector2)bodyTransform.right;
        }

        RaycastHit2D hit;
        hit = Physics2D.Raycast(pos, -bodyTransform.up, raycastLength, LayerMask.GetMask("Default"));

        if (hit)
        {
            return true;
        }
        

        return false;
    }
}
