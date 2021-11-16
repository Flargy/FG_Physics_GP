using UnityEngine;

[CreateAssetMenu(menuName = "States/Enemy/StickyDeathState")]
public class StickyEnemyDeath : BaseState
{
    [SerializeField] private float deathTime = 1.0f;
    
    private StickyEnemyStateMachine actor;
    private Rigidbody2D body;

    
    public override void Initialize(StateMachine NewOwner)
    {
        Owner = NewOwner;
        actor = (StickyEnemyStateMachine)Owner.GetPlayer();
        body = Owner.GetPlayerRB();

    }

    public override void OnEnter()
    {
        body.velocity = Vector2.zero;
        actor.anim.SetBool("IsDead", true);
        Destroy(actor.gameObject, deathTime);
    }

    public override void OnUpdate()
    {
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnExit()
    {
    }
}
