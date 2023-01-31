using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachinePlayer : StateMachine
{
    public override void AddInitialsStates()
    {
        AddCurrState(EnumStates.playerSpawn);
    }

    public override void InitAllStates()
    {
        m_States.Add(EnumStates.playerSpawn, new StatePlayerSpawn(this));
        m_States.Add(EnumStates.playerMove, new StatePlayerMove(this));
        m_States.Add(EnumStates.playerJump, new StatePlayerJump(this));
    }
}
