using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerManageUI : State
{
    GameObject m_UIGameObject;
    StateMachineUI m_StateMachineUI;

    public StateManagerManageUI(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_UIGameObject = Pool.m_Instance.GetObject(EnumUI.UIGame);

        m_StateMachineUI = m_UIGameObject.GetComponent<StateMachineUI>();

        m_UIGameObject.SetActive(true);
    }

    public void AddCurrUIState(EnumStatesUI state)
    {
        m_StateMachineUI.AddCurrState(state);
    }

    public void PopCurrUIState(EnumStatesUI state)
    {
        m_StateMachineUI.PopCurrState(state);
    }

    public void AddSlot()
    {
        StateUIPlayerEquip stateUIPlayerEquip = (StateUIPlayerEquip)m_StateMachineUI.GetState(EnumStatesUI.playerEquipUI);
        stateUIPlayerEquip.AddSlot();
    }

    public void ChangeImageAt(int index, Sprite image)
    {
        StateUIPlayerEquip stateUIPlayerEquip = (StateUIPlayerEquip)m_StateMachineUI.GetState(EnumStatesUI.playerEquipUI);
        stateUIPlayerEquip.ChangeImageAt(index, image);
    }
}
