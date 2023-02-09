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
        AddNewState(EnumStatesPlayer.spawn, new StatePlayerSpawn(this));
        AddNewState(EnumStatesPlayer.controller, new StatePlayerController(this));
        AddNewState(EnumStatesPlayer.pickaxe, new StatePlayerPickaxe(this));
        AddNewState(EnumStatesPlayer.equip, new StatePlayerEquip(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
