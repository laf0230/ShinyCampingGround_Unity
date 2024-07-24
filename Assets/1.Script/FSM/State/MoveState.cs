using UnityEngine;

public class MoveState : IState
{
    private readonly StateMachine stateMachine;

    public MoveState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Entering Move State");
    }

    public void Execute()
    {
        // Perform move actions
        // Check if move is complete
        // if (isMoveComplete)
        // {
        //     stateMachine.SetState(new IdleState(stateMachine));
        // }
    }

    public void Exit()
    {
        Debug.Log("Exiting Move State");
    }
}

