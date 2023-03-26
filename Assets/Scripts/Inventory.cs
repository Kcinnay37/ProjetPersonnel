using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryCase
{
    public object resource;
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

    public List<InventoryCase> GetInventory()
    {
        return m_Inventory;
    }

    //ajoute une nouvelle case a la list
    public void AddNewCase()
    {
        m_Inventory.Add(new InventoryCase());
        InventoryCase temp = m_Inventory[m_Inventory.Count - 1];
        temp.resource = EnumSpecialResources.none;
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
    public int AddRessource(object dataResource)
    {
        for(int i = 0; i < GetInventorySize(); i++)
        {
            if (m_Inventory[i].resource.Equals(dataResource))
            {
                DataResource resource = (DataResource)Pool.m_Instance.GetData(dataResource);
                if (m_Inventory[i].currNb < resource.maxStack)
                {
                    InventoryCase temp = m_Inventory[i];
                    temp.currNb++;
                    m_Inventory[i] = temp;
                    return i;
                }
            }
        }

        for (int i = 0; i < GetInventorySize(); i++)
        {
            if(m_Inventory[i].resource is EnumSpecialResources)
            {
                EnumSpecialResources resource = (EnumSpecialResources)m_Inventory[i].resource;
                if (resource == EnumSpecialResources.none)
                {
                    InventoryCase temp = m_Inventory[i];
                    temp.resource = dataResource;
                    temp.currNb++;
                    m_Inventory[i] = temp;
                    return i;
                }
            }
        }

        return -1;
    }

    public int IncrementResource(object dataResource)
    {
        for (int i = 0; i < GetInventorySize(); i++)
        {
            if (m_Inventory[i].resource.Equals(dataResource))
            {
                DataResource resource = (DataResource)Pool.m_Instance.GetData(dataResource);
                if (m_Inventory[i].currNb < resource.maxStack)
                {
                    InventoryCase temp = m_Inventory[i];
                    temp.currNb++;
                    m_Inventory[i] = temp;
                    return i;
                }
            }
        }

        return -1;
    }

    public bool IncrementCountAt(int index)
    {
        InventoryCase temp = m_Inventory[index];

        DataResource resource = (DataResource)Pool.m_Instance.GetData(temp.resource);
        if(temp.currNb < resource.maxStack)
        {
            temp.currNb++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool DecrementCountAt(int index)
    {
        InventoryCase temp = m_Inventory[index];

        DataResource resource = (DataResource)Pool.m_Instance.GetData(temp.resource);
        if (temp.currNb > 1)
        {
            temp.currNb--;
            return true;
        }
        else
        {
            ClearCase(index);
            return false;
        }
    }

    //remet une case a ses stat initial
    public void ClearCase(int index)
    {
        InventoryCase temp = m_Inventory[index];
        temp.resource = EnumSpecialResources.none;
        temp.currNb = 0;
        m_Inventory[index] = temp;
    }

    public int GetInventorySize()
    {
        return m_Inventory.Count;
    }
}
