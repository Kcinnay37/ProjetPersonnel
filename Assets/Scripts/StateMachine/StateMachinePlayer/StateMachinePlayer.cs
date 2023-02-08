using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachinePlayer : StateMachine
{
    [SerializeField] EnumPlayers m_Data;

    public override void AddInitialsStates()
    {
        AddCurrState(EnumStatesPlayer.spawn);
    }

    public override void InitAllStates()
    {
        m_States.Add(EnumStatesPlayer.spawn, new StatePlayerSpawn(this));
        m_States.Add(EnumStatesPlayer.controller, new StatePlayerController(this));
        m_States.Add(EnumStatesPlayer.pickaxe, new StatePlayerPickaxe(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
