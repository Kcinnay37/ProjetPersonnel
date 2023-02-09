using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerManageMap : State
{
    private DataManager data;

    private GameObject m_MapObject;
    private StateMachineMap m_StateMachineMap;
    private StateMapManager m_StateMapManager;

    public StateManagerManageMap(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        data = (DataManager)m_StateMachine.GetData();

        m_MapObject = Pool.m_Instance.GetObject(data.map);

        m_StateMachineMap = m_MapObject.GetComponent<StateMachineMap>();
        m_StateMapManager = (StateMapManager)m_StateMachineMap.GetState(EnumStatesMap.manager);

        m_MapObject.SetActive(true);

        m_StateMachine.StartCoroutine(CheckMapLoad());
    }

    private IEnumerator CheckMapLoad()
    {
        while(true)
        {
            StateMapManager stateMapManager = (StateMapManager)m_StateMachineMap.GetState(EnumStatesMap.manager);
            if(stateMapManager.IsGenerate())
            {
                m_StateMachine.AddCurrState(EnumStatesManager.managePlayer);
                m_StateMachine.AddCurrState(EnumStatesManager.manageUI);
                break;
            }
            yield return null;
        }
    }

    public void SetPoint(Vector3 worldPos)
    {
        m_StateMapManager.SetPoint(worldPos);
    }

    public Vector3 GetPointToWorld()
    {
        return m_StateMapManager.GetPointToWorld();
    }

    public void PopBlockAt(Vector3 worldPos)
    {
        m_StateMapManager.PopBlockAt(worldPos);
    }
}
