using UnityEngine;

public class IdleState : IState
{
    private readonly StateMachine stateMachine;

    public IdleState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Entering Idle State");
    }

    public void Execute()
    {
        // Check for transitions
        // For example, if a condition is met, transition to MoveState
        // if (condition)
        // {
        //     stateMachine.SetState(new MoveState(stateMachine));
        // }
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

