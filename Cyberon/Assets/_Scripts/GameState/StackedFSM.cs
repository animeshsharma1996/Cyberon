using System.Collections.Generic;
using UnityEngine;

public class StackedFSM
{
    protected Stack<IState> stateHistory;
    protected IState activeState;

    public StackedFSM()
    {
        stateHistory = new Stack<IState>();
    }

    public void OnUpdate()
    {
        if (activeState == null) return;
        activeState.OnUpdate();
    }

    public void Clear()
    {
        while (stateHistory.Count > 0)
        {
            stateHistory.Peek().OnStateExit();
            stateHistory.Pop();
        }
        activeState.OnStateExit();
        activeState = null;
    }

    public void AddState(IState state)  // Push new on stack
    {
        if (activeState != null)
            stateHistory.Push(activeState);
        activeState = state;
        activeState.OnStateEnter();
    }

    public void SetState(IState state) // Replaces the state (pop + push)
    {
        if (activeState != null)
            activeState.OnStateExit();
        if (stateHistory.Count != 0)
            stateHistory.Pop();
        activeState = state;
        activeState.OnStateEnter();
    }

    public void ExitState() // Removes the state and continues with the previous state
    {

        if (stateHistory.Count <= 0)
        {
            // On reaching this error means that your State machine will have had no STATE to exist in
            Debug.LogError("StackedFSM.ExitState(): State machine in has no states in history to replace current state.");
            return;
        }
        activeState.OnStateExit();
        activeState = stateHistory.Pop();
    }
}