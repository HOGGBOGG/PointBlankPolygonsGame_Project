using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiStateMachine
{
    public AiState[] states;
    public AiAgent agent;
    public AiStateID currentStateID;

    public AiStateMachine(AiAgent agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(AiStateID)).Length;
        states = new AiState[numStates]; // allocate memory to new states
    }

    public void RegisterState(AiState state) // add the state to the statemachine
    {
        int index = (int)state.GetID();
        states[index] = state; 
    }

    public AiState GetState(AiStateID stateID) // get state from statemachine through enum
    {
        int index = (int)stateID;
        return states[index];
    }

    public void Update() // acheive abstraction and call Update method of the current state
    {
        GetState(currentStateID)?.Update(agent);
    }

    public void ChangeState(AiStateID newStateID)
    {
        GetState(currentStateID)?.Exit(agent);
        currentStateID = newStateID;
        GetState(currentStateID)?.Enter(agent);
    }
}
