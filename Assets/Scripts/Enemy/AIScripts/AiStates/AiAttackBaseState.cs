using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackBaseState : AiState
{
    public void Enter(AiAgent agent)
    {
        if (agent.isStationary) return;
        //Debug.Log("Enter AttackBase State.");
        for(int i = 0; i< 3; i++)
        {
            if (agent.EnemyBases[i] != null && agent.EnemyBases[i].gameObject.activeInHierarchy)
            {
                agent.navMeshAgent.SetDestination(agent.EnemyBases[i].transform.position);
                return;
            }
        }
        //if (agent.EnemyBase != null)
        //    //if(!agent.isStationary)
        //    agent.navMeshAgent.SetDestination(agent.EnemyBase.transform.position);
        //else
        //{
            //Debug.LogWarning("EnemyBase is null");
        //}
    }

    public void Exit(AiAgent agent)
    {
        //Debug.Log("Exit AttackBase State.");
    }

    public AiStateID GetID()
    {
        return AiStateID.AttackBase;
    }

    public void Update(AiAgent agent) // check if a target is present
    {
        if (agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateID.ChaseTarget);
            return;
        }
        if (agent.isStationary) return;
        for (int i = 0; i < 3; i++)
        {
            
            if (agent.EnemyBases[i] != null && agent.EnemyBases[i].gameObject.activeInHierarchy)
            {
                //Debug.Log("Set Destination to: " + agent.EnemyBases[i].gameObject.name);
                agent.navMeshAgent.SetDestination(agent.EnemyBases[i].transform.position);
                return;
            }
        }
        //if (agent.EnemyBase != null)
        //    if (!agent.navMeshAgent.hasPath)
        //    {
        //        agent.navMeshAgent.SetDestination(agent.EnemyBase.transform.position);
        //    }
        //if (agent.EnemyBase == null)
        //{
        //    Debug.LogWarning("Hostile base not set, AIATTACKBASE STATE wont work properly...");
        //}
    }
}
