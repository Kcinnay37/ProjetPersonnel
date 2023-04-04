using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoragePlayerEquip : DataStorage
{
    private DataStoragePlayerStat m_DataStoragePlayerStat;
    private DataPlayer m_DataPlayer;

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
        m_DataPlayer = (DataPlayer)m_StateMachine.GetData();

        //initialise les inventaire avec la bonne grandeur
        m_InventoryEquip = new Inventory(2);
        m_InventoryEquipSecondary = new Inventory(m_DataStoragePlayerStat.GetSizeInventoryEquip());

        m_IndexEquip = -1;

        InitUI();

        UpdateEquipement();
    }

    public override void End()
    {
        UI.m_Instance.GetUIPlayerEquip()?.CloseUI();
    }

    public void InitUI()
    {
        UI.m_Instance.GetUIPlayerEquip().InitUI();

        for (int i = 0; i < m_InventoryEquipSecondary.GetInventorySize() - 1; i++)
        {
            UI.m_Instance.GetUIPlayerEquip().AddSlot();
        }

        for (int i = 0; i < m_InventoryEquip.GetInventorySize(); i++)
        {
            InventoryCase temp = m_InventoryEquip.GetCase(i);
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(i, temp);
        }

        for (int i = 0; i < m_InventoryEquipSecondary.GetInventorySize(); i++)
        {
            InventoryCase temp = m_InventoryEquipSecondary.GetCase(i);
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(i + 2, temp);
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

        UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index, newCase);
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

        if(slotCase.currNb > 1)
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
        if(m_IndexEquip == -1)
        {
            InventoryCase emptyCase = new InventoryCase();
            emptyCase.resource = EnumSpecialResources.none;
            emptyCase.currNb = 0;
            return emptyCase;
        }

        return m_InventoryEquip.GetCase(m_IndexEquip);
    }

    public object DropOneAtCaseEquip()
    {
        if (m_IndexEquip == -1)
        {
            return null;
        }
        
        InventoryCase inventoryCase = m_InventoryEquip.GetCase(m_IndexEquip);

        if (inventoryCase.currNb == 0)
        {
            return null;
        }

        object currResource = inventoryCase.resource;

        inventoryCase.currNb--;
        if (inventoryCase.currNb <= 0)
        {
            inventoryCase.resource = EnumSpecialResources.none;
            SetCase(m_IndexEquip, inventoryCase);
            return currResource;
        }
        m_InventoryEquip.SetCase(m_IndexEquip, inventoryCase);
        UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(m_IndexEquip, inventoryCase);
        return currResource;
    }

    public bool AddRessource(object resource)
    {
        int index = m_InventoryEquip.IncrementResource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index, m_InventoryEquip.GetCase(index));
            return true;
        }

        index = m_InventoryEquipSecondary.AddRessource(resource);

        if(index != -1)
        {
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index + 2, m_InventoryEquipSecondary.GetCase(index));
            return true;
        }
        return false;
    }

    public bool IncrementRessource(object resource)
    {
        int index = m_InventoryEquip.IncrementResource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index, m_InventoryEquip.GetCase(index));
            return true;
        }

        index = m_InventoryEquipSecondary.IncrementResource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index + 2, m_InventoryEquipSecondary.GetCase(index));
            return true;
        }
        return false;
    }

    public bool DecrementRessource(object resource)
    {
        int index = m_InventoryEquip.DecrementResource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index, m_InventoryEquip.GetCase(index));
            return true;
        }

        index = m_InventoryEquipSecondary.DecrementResource(resource);

        if (index != -1)
        {
            UI.m_Instance.GetUIPlayerEquip().UpdateSlotAt(index + 2, m_InventoryEquipSecondary.GetCase(index));
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

        DataResource resource = (DataResource)Pool.m_Instance.GetData(inventoryCase.resource);

        m_StateMachine.PopCurrState(resource.state);
        m_IndexEquip = -1;
    }

    public void Equip(int index)
    {
        if (UI.m_Instance.GetUIShop().GetShopIsOpen()) return;

        if (m_IndexEquip == index)
        {
            ActionKeyDown();
        }
        else
        {
            UnEquip();

            m_IndexEquip = index;

            InventoryCase currCase = m_InventoryEquip.GetCase(index);
            DataResource resource = (DataResource)Pool.m_Instance.GetData(currCase.resource);
            m_StateMachine.AddCurrState(resource.state);
        }
    }

    public void ActionKeyDown()
    {
        if (m_IndexEquip == -1) return;
        if (UI.m_Instance.GetUIShop().GetShopIsOpen()) return;

        InventoryCase currCase = m_InventoryEquip.GetCase(m_IndexEquip);
        DataResource resource = (DataResource)Pool.m_Instance.GetData(currCase.resource);
        StateRessource stateRessource = (StateRessource)m_StateMachine.GetState(resource.state);
        stateRessource?.ActionKeyDown();
    }

    public void ActionOldKey()
    {
        if (m_IndexEquip == -1) return;
        if (UI.m_Instance.GetUIShop().GetShopIsOpen()) return;

        InventoryCase currCase = m_InventoryEquip.GetCase(m_IndexEquip);
        DataResource resource = (DataResource)Pool.m_Instance.GetData(currCase.resource);
        StateRessource stateRessource = (StateRessource)m_StateMachine.GetState(resource.state);
        stateRessource?.ActionOldKey();
    }

    public void ActionKeyUp()
    {
        if (m_IndexEquip == -1) return;
        if (UI.m_Instance.GetUIShop().GetShopIsOpen()) return;

        InventoryCase currCase = m_InventoryEquip.GetCase(m_IndexEquip);
        DataResource resource = (DataResource)Pool.m_Instance.GetData(currCase.resource);
        StateRessource stateRessource = (StateRessource)m_StateMachine.GetState(resource.state);
        stateRessource?.ActionKeyUp();
    }

    public bool CheckHasEquipementType(object resource)
    {
        if(resource is EnumEquipements)
        {
            EnumEquipements equipement = (EnumEquipements)resource;

            foreach(InventoryCase currCase in m_InventoryEquip.GetInventory())
            {
                if(currCase.resource is EnumEquipements)
                {
                    EnumEquipements currEquipement = (EnumEquipements)currCase.resource;

                    DataEquipement equipement1 = (DataEquipement)Pool.m_Instance.GetData(equipement);
                    DataEquipement equipement2 = (DataEquipement)Pool.m_Instance.GetData(currEquipement);

                    if(equipement1.typeEquipement == equipement2.typeEquipement)
                    {
                        return true;
                    }
                }
            }
            foreach (InventoryCase currCase in m_InventoryEquipSecondary.GetInventory())
            {
                if (currCase.resource is EnumEquipements)
                {
                    EnumEquipements currEquipement = (EnumEquipements)currCase.resource;

                    DataEquipement equipement1 = (DataEquipement)Pool.m_Instance.GetData(equipement);
                    DataEquipement equipement2 = (DataEquipement)Pool.m_Instance.GetData(currEquipement);

                    if (equipement1.typeEquipement == equipement2.typeEquipement)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void UpdateEquipement()
    {
        ResetEquipement();

        foreach (InventoryCase currCase in m_InventoryEquip.GetInventory())
        {
            if (currCase.resource is EnumEquipements)
            {
                EnumEquipements currEquipement = (EnumEquipements)currCase.resource;
                DataEquipement equipement = (DataEquipement)Pool.m_Instance.GetData(currEquipement);
                EquipEquipement(equipement);
            }
        }
        foreach (InventoryCase currCase in m_InventoryEquipSecondary.GetInventory())
        {
            if (currCase.resource is EnumEquipements)
            {
                EnumEquipements currEquipement = (EnumEquipements)currCase.resource;
                DataEquipement equipement = (DataEquipement)Pool.m_Instance.GetData(currEquipement);
                EquipEquipement(equipement);
            }
        }
    }

    private void EquipEquipement(DataEquipement equipement)
    {
        Transform component = m_StateMachine.transform.Find("Component");
        SkinnedMeshRenderer renderer = component.Find(equipement.componentForMaterial).GetComponent<SkinnedMeshRenderer>();
        renderer.material = equipement.playerMaterial;

        if(equipement.typeEquipement == EnumTypeEquipement.hat && equipement.playerMaterial == m_DataPlayer.hatNone)
        {
            component.Find("Hair").gameObject.SetActive(true);
        }
        else if(equipement.typeEquipement == EnumTypeEquipement.hat && equipement.playerMaterial != m_DataPlayer.hatNone)
        {
            component.Find("Hair").gameObject.SetActive(false);
        }

        m_DataStoragePlayerStat.AddStats(equipement.bonusStat);
    }

    private void ResetEquipement()
    {
        Transform component = m_StateMachine.transform.Find("Component");

        if(component != null)
        {
            SkinnedMeshRenderer clothRenderer = component.Find("Cloth").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer pantsRenderer = component.Find("Pants").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer hatRenderer = component.Find("Hat").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer shoesRenderer = component.Find("Shoes").GetComponent<SkinnedMeshRenderer>();

            clothRenderer.material = m_DataPlayer.baseCloth;
            pantsRenderer.material = m_DataPlayer.basePants;
            hatRenderer.material = m_DataPlayer.baseHat;
            shoesRenderer.material = m_DataPlayer.baseShoes;

            component.Find("Hair").gameObject.SetActive(true);
        }

        m_DataStoragePlayerStat.ResetMaxStats();
    }

    public bool ContainResource(object type, int nb)
    {
        if(m_InventoryEquip.ContainResource(type, nb))
        {
            return true;
        }
        if(m_InventoryEquipSecondary.ContainResource(type, nb))
        {
            return true;
        }

        return false;
    }
}
