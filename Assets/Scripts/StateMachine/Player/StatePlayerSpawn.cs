using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerSpawn : State
{
    private const float offset = 0.5f;

    public StatePlayerSpawn(StateMachine stateMachine) : base(stateMachine)
    {
        Vector3 pos = Map.m_Instance.GetGrid().GetPointToWorld();
        pos.z = -1;
        pos.x += offset;
        pos.y += 0.1f;

        m_StateMachine.transform.position = pos;
    }

    public override void OnInit()
    {
        CinemachineVirtualCamera virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = GameObject.Find("FollowCam").transform;
        
        Vector3 pos = Map.m_Instance.GetGrid().GetPointToWorld();
        pos.z = -1;
        pos.x += offset;
        pos.y += 0.1f;

        m_StateMachine.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        m_StateMachine.transform.position = pos;


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
