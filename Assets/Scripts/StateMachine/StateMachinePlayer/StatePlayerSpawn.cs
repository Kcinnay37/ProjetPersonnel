using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerSpawn : State
{
    Transform m_Transfrom;

    const float offset = 0.5f;

    public StatePlayerSpawn(StateMachine stateMachine) : base(stateMachine)
    {
        m_Transfrom = m_StateMachine.transform;
    }

    public override void OnInit()
    {
        Vector2 position = MapGenerator.FindPlayerSpawnPoint();
        Vector3 worldPos = new Vector3(position.x + offset, position.y, -1);

        m_Transfrom.position = worldPos;

        m_StateMachine.PopCurrState(EnumState.spawn);
    }

    public override void End()
    {
        m_StateMachine.AddCurrState(EnumState.move);
        m_StateMachine.AddCurrState(EnumState.jump);
        m_StateMachine.AddCurrState(EnumState.inventory);
    }
}
