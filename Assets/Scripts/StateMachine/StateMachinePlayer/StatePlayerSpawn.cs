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
        m_StateMachine.AddCurrDataStorage(EnumStatesPlayer.stat);
        m_StateMachine.AddCurrDataStorage(EnumStatesPlayer.equip);
        m_StateMachine.AddCurrDataStorage(EnumStatesPlayer.backpack);

        m_StateMachine.AddCurrState(EnumStatesPlayer.controllerMovement);
        m_StateMachine.AddCurrState(EnumStatesPlayer.controllerInventory);
    }
}
