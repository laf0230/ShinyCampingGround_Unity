using UnityEngine;

public class BuildState : IState
{
    private readonly StateMachine stateMachine;

    public BuildState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void Enter()
    {
        Debug.Log("Entering Build State");
    }

    public void Execute()
    {
        // Perform build actions
        // Check if build is complete
        // if (isBuildComplete)
        // {
        //     stateMachine.SetState(new IdleState(stateMachine));
        // }
    }

    public void Exit()
    {
        Debug.Log("Exiting Build State");
    }
}
