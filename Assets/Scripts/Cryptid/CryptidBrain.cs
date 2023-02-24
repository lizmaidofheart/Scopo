using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryptidBrain : MonoBehaviour
{
    // finite state machine to handle the cryptid's behaviour

    public BrainState currentState;

    [SerializeField] float idleTime = 5;

    BrainState idleState;
    BrainState wanderState;
    BrainState followState;

    private void Awake()
    {
        wanderState = new Wander(this);
        followState = new Follow(this);
        idleState = new Idle(this, idleTime, wanderState);
    }

    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    void LateUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }

    public void ChangeState(BrainState newState)
    {
        currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    protected virtual BrainState GetInitialState()
    {
        return idleState;
    }
}
