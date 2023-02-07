using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachinePlayer : StateMachine
{
    [SerializeField] EnumPlayers m_Data;

    public override void AddInitialsStates()
    {
        AddCurrState(EnumStatesPlayer.playerSpawn);
    }

    public override void InitAllStates()
    {
        m_States.Add(EnumStatesPlayer.playerSpawn, new StatePlayerSpawn(this));
       // m_States.Add(EnumStatesPlayer.playerMove, new StatePlayerMove(this));
        //m_States.Add(EnumStatesPlayer.playerJump, new StatePlayerJump(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
