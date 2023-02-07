using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerManagePlayer : State
{
    DataManager data;

    GameObject m_PlayerObject;
    StateMachinePlayer m_StateMachinePlayer;

    public StateManagerManagePlayer(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        // fais spawner le player
        DataManager data = (DataManager)m_StateMachine.GetData();
        m_PlayerObject = Pool.m_Instance.GetObject(data.player);
        m_PlayerObject.SetActive(true);
    }
}
