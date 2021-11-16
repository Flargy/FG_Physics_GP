using UnityEngine;

public class StickyEnemyStateMachine : EnemyBaseStateMachine
{
    [SerializeField] private StateMachine myStateMachine;
    [HideInInspector] public Transform sprite;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public Animator anim;

    private Rigidbody2D body;
    private float xScale;
    
    protected override void Damage()
    {
        base.Damage();
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        Transform[] tempList = GetComponentsInChildren<Transform>();
        foreach (var variable in tempList)
        {
            if (variable.gameObject != this.gameObject)
            {
                sprite = variable;
                break;
            }
        }
        xScale = transform.localScale.y;
        myStateMachine.SetUpStateMashine(this, body);
    }

    private void Update()
    {
        myStateMachine.UpdateStateMachine();
        
    }
    
    private void FixedUpdate()
    {
        myStateMachine.UpdateStateMachineFixed();
    }
    
    public void FaceRight(bool value)
    {
        float direction = value ? xScale: -xScale;
        transform.localScale = new Vector3(direction ,transform.localScale.y, transform.localScale.z);
    }
    
    private void Die()
    {
        isDead = true;
        myStateMachine.ChangeState(StateEnums.DEATH);
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var variable in cols)
        {
            variable.enabled = false;
        }
        sprite.gameObject.GetComponent<SpriteRenderer>().renderingLayerMask = 50;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            float dot = Vector2.Dot(direction, -other.transform.up);

            if (dot < -0.5f)
            {
                Die();
            }
            else
            {
                RespawnManager.Instance.Respawn();
            }
        }
    }
}
