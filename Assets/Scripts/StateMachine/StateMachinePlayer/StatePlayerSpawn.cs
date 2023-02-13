using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerSpawn : State
{
    public StatePlayerSpawn(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        CinemachineVirtualCamera virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = GameObject.Find("FollowCam").transform;

        m_StateMachine.PopCurrState(EnumStatesPlayer.spawn);
    }

    public override void End()
    {
        m_StateMachine.AddCurrState(EnumStatesPlayer.controller);

        m_StateMachine.AddCurrState(EnumStatesPlayer.equip);
        m_StateMachine.AddCurrState(EnumStatesPlayer.backpack);
        m_StateMachine.AddCurrState(EnumStatesPlayer.manageInventory);
    }
}
