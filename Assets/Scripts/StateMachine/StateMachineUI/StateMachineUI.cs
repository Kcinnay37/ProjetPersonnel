using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineUI : StateMachine
{
    [SerializeField] EnumUI m_Data;

    public override void AddInitialsStates()
    {
        AddCurrState(EnumStatesPlayer.UIEquipInventory);
    }

    public override void InitAllStates()
    {
        //m_States.Add(EnumState.UIEquipInventory, new StateUIEquipInventory(this));
    }

    public override ScriptableObject GetData()
    {
        return (DataUI)Pool.m_Instance.GetData(m_Data);
    }
}
