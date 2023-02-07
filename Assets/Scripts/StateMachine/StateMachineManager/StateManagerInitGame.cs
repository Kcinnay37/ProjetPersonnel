using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerInitGame : State
{
    StateManagerData stateManagerData;

    public StateManagerInitGame(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        stateManagerData = (StateManagerData)m_StateMachine.GetStateData(EnumStatesManager.data);
    }

    public override void Update()
    {
        StateMapManager stateMapManager = (StateMapManager)stateManagerData.GetStateMachineMap().GetState(EnumStatesMap.manager);

        if (stateMapManager.IsGenerate())
        {
            m_StateMachine.AddCurrState(EnumStatesManager.spawn);
            m_StateMachine.PopCurrState(EnumStatesManager.initGame);
        }
    }
}
