using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataStorage
{
    protected StateMachine m_StateMachine;

    public DataStorage(StateMachine stateMachine)
    {
        m_StateMachine = stateMachine;
    }

    public virtual void OnInit() { }

    public virtual void End() { }
}
