using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public NPCController controller;
    public StateMachine stateMachine;

    public State(NPCController controller, StateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Execute() { }
}

public class StateMachine
{
    public State currentState;

    public void InitializeState(State state)
    {
        currentState = state;
        currentState.EnterState();
    }

    public void ChangeState(State newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
