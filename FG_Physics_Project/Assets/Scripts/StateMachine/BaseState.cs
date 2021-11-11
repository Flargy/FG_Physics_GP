using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public abstract class BaseState : ScriptableObject
{
    [SerializeField] protected StateEnums StateName;
    protected StateMachine Owner;
    protected Vector2 inputDirection;
    public StateEnums GetName()
    {
        return StateName;
    }
    
    public abstract void Initialize(StateMachine NewOwner);
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnExit();
    
    public bool CompareTo(StateEnums Other)
    {
        return Other == StateName;
    }
}