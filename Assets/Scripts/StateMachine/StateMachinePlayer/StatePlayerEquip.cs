using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerEquip : State
{
    StateManagerManageUI m_StateManagerManageUI;

    public StatePlayerEquip(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_StateManagerManageUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);
        m_StateManagerManageUI.AddCurrUIState(EnumStatesUI.playerEquipUI);
    }

    public override void End()
    {
        m_StateManagerManageUI.PopCurrUIState(EnumStatesUI.playerEquipUI);
    }

    public void InitUI()
    {
        
    }
}
