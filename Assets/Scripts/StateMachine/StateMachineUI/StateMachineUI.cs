using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineUI : StateMachine
{
    public override void AddInitialsStates()
    {
        AddCurrState(EnumState.UIEquipInventory);
    }

    public override void InitAllStates()
    {
        //m_States.Add(EnumState.UIEquipInventory, new StateUIEquipInventory(this));
    }
}
