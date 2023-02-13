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
        AddNewStateData(EnumStatesPlayer.data, new StatePlayerData(this));

        AddNewState(EnumStatesPlayer.spawn, new StatePlayerSpawn(this));

        AddNewState(EnumStatesPlayer.controller, new StatePlayerController(this));

        AddNewState(EnumStatesPlayer.excavation, new StatePlayerExcavationTool(this));

        AddNewState(EnumStatesPlayer.equip, new StatePlayerEquip(this));
        AddNewState(EnumStatesPlayer.backpack, new StatePlayerBackpack(this));
        AddNewState(EnumStatesPlayer.manageInventory, new StatePlayerManageInventory(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
