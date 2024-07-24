using UnityEngine;

public class PackingState : IState
{
    private readonly StateMachine stateMachine;

    public PackingState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Entering Packing State");
    }

    public void Execute()
    {
        // Perform packing actions
        // Check if packing is complete
        // if (isPackingComplete)
        // {
        //     stateMachine.SetState(new IdleState(stateMachine));
        // }
    }

    public void Exit()
    {
        Debug.Log("Exiting Packing State");
    }
}