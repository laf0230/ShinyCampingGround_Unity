using UnityEngine;

public class ExitState : IState
{
    private readonly StateMachine stateMachine;

    public ExitState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Entering Exit State");
    }

    public void Execute()
    {
        // Perform exit actions
        // Check if exit is complete
        // if (isExitComplete)
        // {
        //     // Handle exit
        // }
    }

    public void Exit()
    {
        Debug.Log("Exiting Exit State");
    }
}