using System;
using UnityEngine;

public class NormalEnemyStateMachine : EnemyBaseStateMachine
{
    [SerializeField] private StateMachine myStateMachine;
    [HideInInspector] public Transform sprite;
    [HideInInspector] public bool isDead = false;

    private Rigidbody2D body;
    private Animator anim;
    private float xScale;
    
    protected override void Damage()
    {
        base.Damage();
    }
    
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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

    private void Start()
    {
        if(WorldRotation.Instance)
            WorldRotation.Instance.RegisterEnemy(this);
    }

    private void Update()
    {
        myStateMachine.UpdateStateMachine();
        
    }

    private void Die()
    {
        isDead = true;
        myStateMachine.ChangeState(StateEnums.FALLING);
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var variable in cols)
        {
            variable.enabled = false;
        }
        WorldRotation.Instance.UnregisterEnemy(this);
        Destroy(this, 2.5f);
        sprite.gameObject.GetComponent<SpriteRenderer>().renderingLayerMask = 50;
    }

    private void FixedUpdate()
    {
        myStateMachine.UpdateStateMachineFixed();
    }

    public void Rotate()
    {
        myStateMachine.ChangeState(StateEnums.FALLING);
    }
    
    public void FaceRight(bool value)
    {
        float direction = value ? xScale: -xScale;
        transform.localScale = new Vector3(direction ,transform.localScale.y, transform.localScale.z);
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
