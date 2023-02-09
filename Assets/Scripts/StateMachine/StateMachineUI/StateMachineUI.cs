using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineUI : StateMachine
{
    [SerializeField] EnumUI m_Data;

    public override void AddInitialsStates()
    {
        
    }

    public override void InitAllStates()
    {
        AddNewState(EnumStatesUI.playerEquipUI, new StateUIPlayerEquip(this));
    }

    public override ScriptableObject GetData()
    {
        return (DataUI)Pool.m_Instance.GetData(m_Data);
    }
}
