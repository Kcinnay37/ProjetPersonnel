using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoragePlayerBackpack : DataStorage
{
    private Inventory m_Inventory;

    public DataStoragePlayerBackpack(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        //initialise l'inventaire avec la bonne grandeur
        m_Inventory = new Inventory(12);

        InitUI();
    }

    public override void End()
    {
        
    }

    public void InitUI()
    {
        for (int i = 0; i < m_Inventory.GetInventorySize(); i++)
        {
            InventoryCase temp = m_Inventory.GetCase(i);
            UI.m_Instance.GetUIBackpack().UpdateSlotAt(i, temp);
        }
    }

    public void SetCase(int index, InventoryCase newCase)
    {
        m_Inventory.SetCase(index, newCase);

        UI.m_Instance.GetUIBackpack().UpdateSlotAt(index, newCase);
    }

    public InventoryCase PopCase(int index)
    {
        InventoryCase emptyCase = new InventoryCase();
        emptyCase.resource = EnumSpecialResources.none;
        emptyCase.currNb = 0;

        InventoryCase currCase = GetCase(index);
        SetCase(index, emptyCase);
        return currCase;
    }

    public InventoryCase PopHalfCase(int index)
    {
        InventoryCase slotCase = GetCase(index);
        InventoryCase currCase = new InventoryCase();
        currCase.resource = EnumSpecialResources.none;
        currCase.currNb = 0;

        if (slotCase.currNb > 1)
        {
            int half = slotCase.currNb / 2;
            slotCase.currNb = slotCase.currNb - half;
            currCase.currNb = half;

            currCase.resource = slotCase.resource;
        }

        SetCase(index, slotCase);
        return currCase;
    }

    public InventoryCase GetCase(int index)
    {
        return m_Inventory.GetCase(index);
    }

    public bool AddRessource(object resource)
    {
        int index = m_Inventory.AddRessource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIBackpack().UpdateSlotAt(index, m_Inventory.GetCase(index));
            return true;
        }

        return false;
    }

    public bool IncrementRessource(object resource)
    {
        int index = m_Inventory.IncrementResource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIBackpack().UpdateSlotAt(index, m_Inventory.GetCase(index));
            return true;
        }

        return false;
    }
}
