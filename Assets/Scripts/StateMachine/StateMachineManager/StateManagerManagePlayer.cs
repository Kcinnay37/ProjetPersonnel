using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerManagePlayer : State
{
    DataManager data;

    GameObject m_PlayerObject;
    StateMachinePlayer m_StateMachinePlayer;

    const float offset = 0.5f;

    public StateManagerManagePlayer(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        // fais spawner le player
        DataManager data = (DataManager)m_StateMachine.GetData();
        m_PlayerObject = Pool.m_Instance.GetObject(data.player);

        StateManagerManageMap map = (StateManagerManageMap)m_StateMachine.GetState(EnumStatesManager.manageMap);

        Vector3 pos = map.GetPointToWorld();
        pos.z = -1;
        pos.x += offset;

        m_PlayerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        m_PlayerObject.transform.position = pos;

        m_PlayerObject.SetActive(true);
    }

    public Vector3 GetPlayerPos()
    {
        if(m_PlayerObject != null) return m_PlayerObject.transform.position;
        else return Vector3.zero;
    }
}
