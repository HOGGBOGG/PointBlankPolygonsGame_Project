using UnityEngine;
using UnityEngine.AI;

public class AiChaseTargetState : AiState
{
    public AiStateID GetID()
    {
        return AiStateID.ChaseTarget;
    }
    public void Enter(AiAgent agent)
    {
        if (agent.isStationary) return;
        if (agent.targeting.HasTarget) // VVIMP
        {
            agent.navMeshAgent.SetDestination(agent.targeting.TargetPosition);
        }
        //Debug.Log("Enter Chase State.");
    }

    public void Exit(AiAgent agent)
    {
        //Debug.Log("Exit Chase State.");
    }

    public void Update(AiAgent agent) // wait until no target has been found for a while
    {
        if (agent.isStationary)
            return; // persist the state here so that the turret keeps shooting, base stays in one place
        if (!agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateID.AttackBase);
            return;
        }

    }


}
