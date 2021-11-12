using System;
using UnityEngine;

public enum StateMovementDiraction
{
    RIGHT,
    LEFT,
    NONE
}

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private StateMachine myStateMachine;
    [HideInInspector] public Vector2 wallNormal;
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public StateMovementDiraction direction = StateMovementDiraction.NONE;
    [HideInInspector] public float baseGravity;
    [HideInInspector] public Vector2 wallInputDirection;
    private Rigidbody2D body;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
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
        myStateMachine.ChangeState(StateEnums.FALLING);
    }
}
