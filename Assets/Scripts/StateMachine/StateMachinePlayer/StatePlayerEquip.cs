using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerEquip : State
{
    private StateManagerManageUI m_StateManagerManageUI;
    private StatePlayerData m_StatePlayerData;

    private Inventory m_InventoryEquip;
    private Inventory m_InventoryEquipSecondary;

    private int m_IndexEquip;

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

        m_IndexEquip = -1;

        //ajoute la state du UI player equip
        m_StateManagerManageUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);
        m_StateManagerManageUI.AddCurrUIState(EnumStatesUI.playerEquipUI);
    }

    public override void End()
    {
        m_StateManagerManageUI.PopCurrUIState(EnumStatesUI.playerEquipUI);
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            InventoryCase temp = new InventoryCase();
            temp.resource = (DataResource)Pool.m_Instance.GetData(EnumTools.pickaxe);
            temp.currNb = 1;
            SetCase(0, temp);
        }
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
            stateManagerManageUI.UpdateCaseAtInventoryEquip(i + 2, temp);
        }
    }

    public void SetCase(int index, InventoryCase newCase)
    {
        if(index < 2)
        {
            if(m_IndexEquip == index)
            {
                UnEquip(index);
            }

            m_InventoryEquip.SetCase(index, newCase);
            
            if(m_IndexEquip == index)
            {
                Equip(index);
            }
        }
        else
        {
            m_InventoryEquipSecondary.SetCase(index - 2, newCase);
        }

        m_StateManagerManageUI.UpdateCaseAtInventoryEquip(index, newCase);
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

    private void UnEquip(int index)
    {
        InventoryCase inventoryCase = m_InventoryEquip.GetCase(index);
        m_StateMachine.PopCurrState(inventoryCase.resource.state);
    }

    private void Equip(int index)
    {
        m_IndexEquip = index;

        InventoryCase currCase = m_InventoryEquip.GetCase(index);

        if (index == 0)
        {
            UnEquip(1);
        }
        else
        {
            UnEquip(0);
        }
        m_StateMachine.AddCurrState(currCase.resource.state);

    }
}
