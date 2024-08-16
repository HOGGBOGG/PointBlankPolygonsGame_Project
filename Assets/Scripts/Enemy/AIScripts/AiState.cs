using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AiStateID
{
    ChaseTarget,
    Death,
    AttackBase
}

public interface AiState
{
    AiStateID GetID();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}
