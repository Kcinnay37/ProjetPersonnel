using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerEquip : State
{
    private StateManagerManageUI m_StateManagerManageUI;
    private StatePlayerData m_StatePlayerData;

    private Inventory m_InventoryEquip;
    private Inventory m_InventoryEquipSecondary;

    public StatePlayerEquip(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        // va chercher le data local du player
        m_StatePlayerData = (StatePlayerData)m_StateMachine.GetStateData(EnumStatesPlayer.data);

        //initialise les inventaire avec la bonne grandeur
        m_InventoryEquip = new Inventory(2);
        m_InventoryEquipSecondary = new Inventory(m_StatePlayerData.GetSizeInventoryEquip());

        m_InventoryEquipSecondary.AddRessource((DataResource)Pool.m_Instance.GetData(EnumTools.pickaxe));

        m_InventoryEquip.AddRessource((DataResource)Pool.m_Instance.GetData(EnumTools.pickaxe));

        //ajoute la state du UI player equip
        m_StateManagerManageUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);
        m_StateManagerManageUI.AddCurrUIState(EnumStatesUI.playerEquipUI);
    }

    public override void End()
    {
        m_StateManagerManageUI.PopCurrUIState(EnumStatesUI.playerEquipUI);
    }

    public void InitUI()
    {
        StateManagerManageUI stateManagerManageUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);
        
        for(int i = 0; i < m_InventoryEquipSecondary.GetInventorySize() - 1; i++)
        {
            stateManagerManageUI.AddSlotInventoryEquip();
        }

        for (int i = 0; i < m_InventoryEquip.GetInventorySize(); i++)
        {
            InventoryCase temp = m_InventoryEquip.GetCase(i);
            stateManagerManageUI.UpdateCaseAtInventoryEquip(i, temp);
        }

        for (int i = 0; i < m_InventoryEquipSecondary.GetInventorySize(); i++)
        {
            InventoryCase temp = m_InventoryEquipSecondary.GetCase(i);
            stateManagerManageUI.UpdateCaseAtInventoryEquip(i, temp);
        }
    }
}
