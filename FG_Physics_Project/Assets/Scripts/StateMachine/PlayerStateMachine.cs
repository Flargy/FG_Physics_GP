using System;
using UnityEngine;

public enum StateMovementDiraction
{
    RIGHT,
    LEFT,
    NONE
}

public class PlayerStateMachine : StateMachineBase
{
    [SerializeField] private StateMachine myStateMachine;
    [HideInInspector] public Vector2 wallNormal;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public StateMovementDiraction direction = StateMovementDiraction.NONE;
    [HideInInspector] public float baseGravity;
    [HideInInspector] public Vector2 wallInputDirection;
    [HideInInspector] public Animator anim;
    [HideInInspector] public float drag;
    private Rigidbody2D body;
    private float xScale;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        drag = body.drag;
        xScale = transform.localScale.y;
        myStateMachine.SetUpStateMashine(this, body);
    }

    private void Start()
    {
        RespawnManager.Instance.RegisterPlayer(this);
    }

    private void Update()
    {
        myStateMachine.UpdateStateMachine();
    }

    private void FixedUpdate()
    {
        myStateMachine.UpdateStateMachineFixed();
    }

    public void WorldRotation()
    {
        myStateMachine.ChangeState(StateEnums.FALLING);
    }

    public void Respawn(Vector2 pos)
    {
        transform.position = pos;
        body.velocity = Vector2.zero;
        myStateMachine.ChangeState(StateEnums.FALLING);
    }

    public void FaceRight(bool value)
    {
        float direction = value ? xScale: -xScale;
        transform.localScale = new Vector3(direction ,transform.localScale.y, transform.localScale.z);
    }
    
    
}
