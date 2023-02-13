using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerManageUI : State
{
    private GameObject m_UIGameObject;
    private StateMachineUI m_StateMachineUI;

    public StateManagerManageUI(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_UIGameObject = Pool.m_Instance.GetObject(EnumUI.UIGame);

        m_StateMachineUI = m_UIGameObject.GetComponent<StateMachineUI>();

        m_UIGameObject.SetActive(true);
    }

    //ajoute une state dans la state machine de UI courrant
    public void AddCurrUIState(EnumStatesUI state)
    {
        m_StateMachineUI.AddCurrState(state);
    }

    //retire une state dans la state machine de UI courrant
    public void PopCurrUIState(EnumStatesUI state)
    {
        m_StateMachineUI.PopCurrState(state);
    }

    //manipulation inventory equip -------------------------------------------
    public void AddSlotInventoryEquip()
    {
        StateUIPlayerEquip stateUIPlayerEquip = (StateUIPlayerEquip)m_StateMachineUI.GetState(EnumStatesUI.playerEquipUI);
        stateUIPlayerEquip.AddSlot();
    }

    public void UpdateCaseAtInventoryEquip(int index, InventoryCase inventoryCase)
    {
        StateUIPlayerEquip stateUIPlayerEquip = (StateUIPlayerEquip)m_StateMachineUI.GetState(EnumStatesUI.playerEquipUI);
        stateUIPlayerEquip.UpdateSlotAt(index, inventoryCase);
    }

    public List<Transform> GetAllSlotInventoryEquip()
    {
        StateUIPlayerEquip stateUIPlayerEquip = (StateUIPlayerEquip)m_StateMachineUI.GetState(EnumStatesUI.playerEquipUI);
        return stateUIPlayerEquip.GetAllSlots();
    }
    // --------------------------------------------------------------------------
}
