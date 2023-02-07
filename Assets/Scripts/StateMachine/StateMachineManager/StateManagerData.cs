using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerData : StateData
{
    private DataManager data;

    private GameObject map;
    private StateMachineMap stateMachineMap;

    public StateManagerData(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        data = (DataManager)m_StateMachine.GetData();
        map = Pool.m_Instance.GetObject(data.map);
        stateMachineMap = map.GetComponent<StateMachineMap>();

        map.SetActive(true);
    }

    public override void End()
    {
        Pool.m_Instance.RemoveObject(map, data.map);
    }

    public StateMachineMap GetStateMachineMap()
    {
        return stateMachineMap;
    }
}
