using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerSpawn : State
{


    const float offset = 0.5f;

    public StatePlayerSpawn(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {

        StateManagerManageMap map = (StateManagerManageMap)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageMap);

        Vector3 pos = map.GetPointToWorld();
        pos.z = -1;
        pos.x += offset;

        m_StateMachine.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        m_StateMachine.transform.position = pos;

        //Vector2 position = Vector2.zero;//MapGenerator.GetInitialPoint();
        //Vector3 worldPos = new Vector3(position.x + offset, position.y, -1);

        //m_Transfrom.position = worldPos;

        //m_StateMachine.PopCurrState(EnumStatesPlayer.playerSpawn);
    }

    public override void End()
    {
        //m_StateMachine.AddCurrState(EnumStatesPlayer.playerMove);
        //m_StateMachine.AddCurrState(EnumStatesPlayer.playerJump);
        //m_StateMachine.AddCurrState(EnumStatesPlayer.playerEquipInventory);
    }
}
