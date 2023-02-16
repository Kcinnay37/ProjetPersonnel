using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorageManageUI : DataStorage
{
    private GameObject m_UIGameObject;
    private StateMachineUI m_StateMachineUI;

    public DataStorageManageUI(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_UIGameObject = Pool.m_Instance.GetObject(EnumUI.UIGame);

        m_StateMachineUI = m_UIGameObject.GetComponent<StateMachineUI>();

        m_UIGameObject.SetActive(true);
    }

    //ajoute une state dans la state machine de UI courrant
    public void AddCurrStateUI(EnumStatesUI state)
    {
        m_StateMachineUI.AddCurrState(state);
    }

    //retire une state dans la state machine de UI courrant
    public void PopCurrStateUI(EnumStatesUI state)
    {
        m_StateMachineUI.PopCurrState(state);
    }

    public void AddCurrDataStorageUI(EnumStatesUI dataStorage)
    {
        m_StateMachineUI.AddCurrDataStorage(dataStorage);
    }

    public void PopCurrDataStorageUI(EnumStatesUI dataStorage)
    {
        m_StateMachineUI.PopCurrDataStorage(dataStorage);
    }

    //manipulation inventory equip -------------------------------------------
    public void AddSlotInventoryEquip()
    {
        DataStorageUIPlayerEquip stateUIPlayerEquip = (DataStorageUIPlayerEquip)m_StateMachineUI.GetDataStorage(EnumStatesUI.playerEquipUI);
        stateUIPlayerEquip.AddSlot();
    }

    public void UpdateCaseAtInventoryEquip(int index, InventoryCase inventoryCase)
    {
        DataStorageUIPlayerEquip stateUIPlayerEquip = (DataStorageUIPlayerEquip)m_StateMachineUI.GetDataStorage(EnumStatesUI.playerEquipUI);
        stateUIPlayerEquip.UpdateSlotAt(index, inventoryCase);
    }

    public List<Transform> GetAllSlotInventoryEquip()
    {
        DataStorageUIPlayerEquip stateUIPlayerEquip = (DataStorageUIPlayerEquip)m_StateMachineUI.GetDataStorage(EnumStatesUI.playerEquipUI);
        return stateUIPlayerEquip.GetAllSlots();
    }
    // --------------------------------------------------------------------------
}
