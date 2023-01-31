using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateData
{
    protected StateMachine m_StateMachine;

    public StateData(StateMachine stateMachine)
    {
        m_StateMachine = stateMachine;
    }
}
