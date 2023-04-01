using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachinePlayer : StateMachine
{
    [SerializeField] EnumPlayers m_Data;

    public override void AddInitialsStatesAndData()
    {
        AddCurrState(EnumStatesPlayer.spawn);
    }

    public override void InitAllStatesAndData()
    {
        AddNewDataStorage(EnumStatesPlayer.stat, new DataStoragePlayerStat(this));
        AddNewDataStorage(EnumStatesPlayer.equip, new DataStoragePlayerEquip(this));
        AddNewDataStorage(EnumStatesPlayer.backpack, new DataStoragePlayerBackpack(this));

        AddNewState(EnumStatesPlayer.spawn, new StatePlayerSpawn(this));
        AddNewState(EnumStatesPlayer.controllerMovement, new StatePlayerControllerMovement(this));
        AddNewState(EnumStatesPlayer.excavation, new StatePlayerExcavationTool(this));
        AddNewState(EnumStatesPlayer.build, new StatePlayerBuild(this));
        AddNewState(EnumStatesPlayer.controllerInventory, new StatePlayerControllerInventory(this));
        AddNewState(EnumStatesPlayer.meleeWeapon, new StatePlayerMeleeWeapon(this));
        AddNewState(EnumStatesPlayer.collectResource, new StatePlayerCollectResourceTool(this));
        AddNewState(EnumStatesPlayer.controllerMount, new StatePlayerControllerMount(this));
        AddNewState(EnumStatesPlayer.material, new StatePlayerMaterial(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
