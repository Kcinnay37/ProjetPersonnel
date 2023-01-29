using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachinePlayer : StateMachine
{
    public override void AddInitialsStates()
    {
        AddCurrState(EnumState.playerSpawn);
    }

    public override void InitAllStates()
    {
        // movement
        m_States.Add(EnumState.playerMove, new StatePlayerMove(this));
        m_States.Add(EnumState.playerJump, new StatePlayerJump(this));

        //spawn
        m_States.Add(EnumState.playerSpawn, new StatePlayerSpawn(this));

        //inventory
        //m_States.Add(EnumState.playerEquipInventory, new StatePlayerEquipInventory(this));

        //recource
        //m_States.Add(EnumState.playerPickaxe, new StatePlayerPickaxe(this));
    }
}
