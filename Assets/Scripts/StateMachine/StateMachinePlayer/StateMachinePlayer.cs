using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachinePlayer : StateMachine
{
    public override void AddInitialsStates()
    {
        AddCurrState(EnumState.spawn);
    }

    public override void InitAllStates()
    {
        // movement
        m_States.Add(EnumState.move, new StatePlayerMove(this));
        m_States.Add(EnumState.jump, new StatePlayerJump(this));

        //spawn
        m_States.Add(EnumState.spawn, new StatePlayerSpawn(this));

        //inventory
        m_States.Add(EnumState.inventory, new StatePlayerEquip(this));

        //recource
        m_States.Add(EnumState.pickaxe, new StatePlayerPickaxe(this));
    }
}
