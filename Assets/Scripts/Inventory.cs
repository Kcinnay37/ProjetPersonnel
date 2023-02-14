using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryCase
{
    public DataResource resource;
    public int currNb;
}

public class Inventory : MonoBehaviour
{
    private List<InventoryCase> m_Inventory;

    public Inventory(int initialSize)
    {
        m_Inventory = new List<InventoryCase>();
        for(int i = 0; i < initialSize; i++)
        {
            AddNewCase();
        }
    }

    //ajoute une nouvelle case a la list
    public void AddNewCase()
    {
        m_Inventory.Add(new InventoryCase());
        InventoryCase temp = m_Inventory[m_Inventory.Count - 1];
        temp.resource = (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none);
        temp.currNb = 0;
        m_Inventory[m_Inventory.Count - 1] = temp;
    }

    //Pop une case de la list
    public void PopCase()
    {
        m_Inventory.RemoveAt(m_Inventory.Count - 1);
    }

    // retourne la valeur d'une case
    public InventoryCase GetCase(int index)
    {
        return m_Inventory[index];
    }

    // Set les value d'une case de l'inventaire
    public void SetCase(int index, InventoryCase inventoryCase)
    {
        m_Inventory[index] = inventoryCase;
    }

    // ajoute une ressource dans l'inventaire
    public void AddRessource(DataResource dataResource)
    {
        for(int i = 0; i < GetInventorySize(); i++)
        {
            if(m_Inventory[i].resource == dataResource)
            {
                if(m_Inventory[i].currNb < dataResource.maxStack)
                {
                    InventoryCase temp = m_Inventory[i];
                    temp.currNb++;
                    m_Inventory[i] = temp;
                    return;
                }
            }
        }

        for (int i = 0; i < GetInventorySize(); i++)
        {
            if (m_Inventory[i].resource == (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none))
            {
                InventoryCase temp = m_Inventory[i];
                temp.resource = dataResource;
                temp.currNb++;
                m_Inventory[i] = temp;
                return;
            }
        }
    }

    //remet une case a ses stat initial
    public void ClearCase(int index)
    {
        InventoryCase temp = m_Inventory[index];
        temp.resource = (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none);
        temp.currNb = 0;
        m_Inventory[m_Inventory.Count - 1] = temp;
    }

    public int GetInventorySize()
    {
        return m_Inventory.Count;
    }
}
