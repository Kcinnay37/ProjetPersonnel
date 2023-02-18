using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoragePlayerEquip : DataStorage
{
    private DataStorageManageUI m_DataStorageManageUI;
    private DataStoragePlayerStat m_DataStoragePlayerStat;

    private Inventory m_InventoryEquip;
    private Inventory m_InventoryEquipSecondary;

    private int m_IndexEquip;

    public DataStoragePlayerEquip(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        // va chercher le data local du player
        m_DataStoragePlayerStat = (DataStoragePlayerStat)m_StateMachine.GetDataStorage(EnumStatesPlayer.stat);

        //initialise les inventaire avec la bonne grandeur
        m_InventoryEquip = new Inventory(2);
        m_InventoryEquipSecondary = new Inventory(m_DataStoragePlayerStat.GetSizeInventoryEquip());

        m_IndexEquip = -1;

        //ajoute la state du UI player equip
        m_DataStorageManageUI = (DataStorageManageUI)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageUI);
        m_DataStorageManageUI?.AddCurrDataStorageUI(EnumStatesUI.playerEquipUI);
    }

    public override void End()
    {
        m_DataStorageManageUI?.PopCurrStateUI(EnumStatesUI.playerEquipUI);
    }

    public void InitUI()
    {   
        for(int i = 0; i < m_InventoryEquipSecondary.GetInventorySize() - 1; i++)
        {
            m_DataStorageManageUI.AddSlotInventoryEquip();
        }

        for (int i = 0; i < m_InventoryEquip.GetInventorySize(); i++)
        {
            InventoryCase temp = m_InventoryEquip.GetCase(i);
            m_DataStorageManageUI.UpdateCaseAtInventoryEquip(i, temp);
        }

        for (int i = 0; i < m_InventoryEquipSecondary.GetInventorySize(); i++)
        {
            InventoryCase temp = m_InventoryEquipSecondary.GetCase(i);
            m_DataStorageManageUI.UpdateCaseAtInventoryEquip(i + 2, temp);
        }
    }

    public void SetCase(int index, InventoryCase newCase)
    {
        if(index < 2)
        {
            if(m_IndexEquip == index)
            {
                UnEquip();
            }

            m_InventoryEquip.SetCase(index, newCase);
        }
        else
        {
            m_InventoryEquipSecondary.SetCase(index - 2, newCase);
        }

        m_DataStorageManageUI.UpdateCaseAtInventoryEquip(index, newCase);
    }

    public InventoryCase PopCase(int index)
    {
        InventoryCase emptyCase = new InventoryCase();
        emptyCase.resource = (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none);
        emptyCase.currNb = 0;

        InventoryCase currCase = GetCase(index);
        SetCase(index, emptyCase);
        return currCase;
    }

    public InventoryCase GetCase(int index)
    {
        if (index < 2)
        {
            return m_InventoryEquip.GetCase(index);
        }
        else
        {
            return m_InventoryEquipSecondary.GetCase(index - 2);
        }
    }

    public InventoryCase GetEquipCase()
    {
        return m_InventoryEquip.GetCase(m_IndexEquip);
    }

    public bool AddRessource(DataResource resource)
    {
        int index = m_InventoryEquipSecondary.AddRessource(resource);

        if(index != -1)
        {
            m_DataStorageManageUI.UpdateCaseAtInventoryEquip(index + 2, m_InventoryEquipSecondary.GetCase(index));
            return true;
        }
        return false;
    }

    private void UnEquip()
    {
        if (m_IndexEquip == -1)
        {
            return;
        }

        InventoryCase inventoryCase = m_InventoryEquip.GetCase(m_IndexEquip);
        m_StateMachine.PopCurrState(inventoryCase.resource.state);
        m_IndexEquip = -1;
    }

    public void Equip(int index)
    {
        if(m_IndexEquip == index)
        {
            ActionKeyDown();
        }
        else
        {
            UnEquip();

            m_IndexEquip = index;

            InventoryCase currCase = m_InventoryEquip.GetCase(index);
            m_StateMachine.AddCurrState(currCase.resource.state);
        }
    }

    public void ActionKeyDown()
    {
        InventoryCase currCase = m_InventoryEquip.GetCase(m_IndexEquip);
        StateRessource stateRessource = (StateRessource)m_StateMachine.GetState(currCase.resource.state);
        stateRessource?.ActionKeyDown();
    }

    public void ActionOldKey()
    {
        InventoryCase currCase = m_InventoryEquip.GetCase(m_IndexEquip);
        StateRessource stateRessource = (StateRessource)m_StateMachine.GetState(currCase.resource.state);
        stateRessource?.ActionOldKey();
    }
}
