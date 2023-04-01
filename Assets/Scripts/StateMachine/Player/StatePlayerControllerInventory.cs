using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePlayerControllerInventory : State
{
    public struct MouseItem
    {
        public InventoryCase inventoryCase;
        public Action<InventoryCase> setRoot;
        public Func<InventoryCase> getRoot;
        public bool onInventoryEquip;
    }

    DataPlayer m_GlobalDataPlayer;

    Canvas m_Canvas;

    MouseItem m_CurrMouseItem;

    private bool m_InventoryUsed;

    DataStoragePlayerEquip m_DataStoragePlayerEquip;
    DataStoragePlayerBackpack m_DataStoragePlayerBackpack;

    bool m_PrimaryKeyUse;
    bool m_SecondaryKeyUse;

    Vector2 localPoint;

    public StatePlayerControllerInventory(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        m_DataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        m_DataStoragePlayerBackpack = (DataStoragePlayerBackpack)m_StateMachine.GetDataStorage(EnumStatesPlayer.backpack);

        GameObject uiScreen = GameObject.Find("UIScreen");
        m_Canvas = uiScreen.GetComponent<Canvas>();

        m_PrimaryKeyUse = false;
        m_SecondaryKeyUse = false;
    }

    public override void End()
    {
        
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CollectResource(EnumTools.pickaxe);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CollectResource(EnumTools.sword);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CollectResource(EnumTools.axe);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CollectResource(EnumTools.hammer);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CollectResource(EnumEquipements.firstHat);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            CollectResource(EnumMount.broom);
        }

        if (Input.GetKeyDown(m_GlobalDataPlayer.openBackpackKey))
        {
            UI.m_Instance.GetUIBackpack().ChangeUIState();
        }

        if (Input.GetKeyDown(m_GlobalDataPlayer.dropResourceEquipKey))
        {
            DropEquipResource();
        }

        //va chercher la position de la souris selon le canvas
        Vector2 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.transform as RectTransform, mousePosition, m_Canvas.worldCamera, out localPoint);

        if(m_PrimaryKeyUse == false && m_SecondaryKeyUse == false)
        {
            if (Input.GetKeyDown(m_GlobalDataPlayer.primarySlotKey))
            {
                m_PrimaryKeyUse = true;
            }
            else if (Input.GetKeyDown(m_GlobalDataPlayer.secondarySlotKey))
            {
                m_SecondaryKeyUse = true;
            }
        }

        if (m_PrimaryKeyUse || m_SecondaryKeyUse)
        {
            ActionKey();
        }

        if(m_PrimaryKeyUse == true && Input.GetKeyUp(m_GlobalDataPlayer.primarySlotKey))
        {
            m_PrimaryKeyUse = false;
        }
        if (m_SecondaryKeyUse == true && Input.GetKeyUp(m_GlobalDataPlayer.secondarySlotKey))
        {
            m_SecondaryKeyUse = false;
        }
    }

    private void ActionKey()
    {
        //si l'inventaire est pas utiliser
        if (!m_InventoryUsed)
        {
            //si la primary key est appuyer et elle est actif
            if (Input.GetKeyDown(m_GlobalDataPlayer.primarySlotKey) && m_PrimaryKeyUse)
            {
                //prend ce qui ce trouve sous la souris
                m_CurrMouseItem = GetMouseClickInInventoryUI(localPoint, true, true);

                //si il avait quelque chose sur la souris
                if (m_CurrMouseItem.setRoot != null && m_CurrMouseItem.inventoryCase.currNb != 0)
                {
                    m_InventoryUsed = true;
                    UI.m_Instance.GetUIMouse().SetValueMouseContent(m_CurrMouseItem.inventoryCase);
                    UI.m_Instance.GetUIMouse().SetActiveMouseContent(true);
                    return;
                }

                //equipe la slot 0 ou joue l'action key down si elle est deja equiper
                m_DataStoragePlayerEquip.Equip(0);
            }
            //si la primary key est appuyer et elle est actif
            if (Input.GetKeyDown(m_GlobalDataPlayer.secondarySlotKey) && m_SecondaryKeyUse)
            {
                //prend la moitier de ce qu'il a sous la souris
                m_CurrMouseItem = GetMouseClickInInventoryUI(localPoint, false, true);

                //si il a quelque chose sous la souris
                if (m_CurrMouseItem.setRoot != null && m_CurrMouseItem.inventoryCase.currNb != 0)
                {
                    m_InventoryUsed = true;

                    UI.m_Instance.GetUIMouse().SetValueMouseContent(m_CurrMouseItem.inventoryCase);
                    UI.m_Instance.GetUIMouse().SetActiveMouseContent(true);
                    return;
                }

                //equipe la slot 1 ou joue laction key down si elle est utilisé
                m_DataStoragePlayerEquip.Equip(1);
            }

            //si la key actif est maintenue joue l'action key down
            if ((Input.GetKey(m_GlobalDataPlayer.primarySlotKey) && m_PrimaryKeyUse) || (Input.GetKey(m_GlobalDataPlayer.secondarySlotKey) && m_SecondaryKeyUse))
            {
                m_DataStoragePlayerEquip.ActionOldKey();
            }

            if ((Input.GetKeyUp(m_GlobalDataPlayer.primarySlotKey) && m_PrimaryKeyUse) || (Input.GetKeyUp(m_GlobalDataPlayer.secondarySlotKey) && m_SecondaryKeyUse))
            {
                m_DataStoragePlayerEquip.ActionKeyUp();
            }
        }
        else
        {
            //si le joueur appui sur l'autre touche que la touche actif dans l'inventaire drop une ressource
            if ((Input.GetKeyDown(m_GlobalDataPlayer.primarySlotKey) && m_SecondaryKeyUse) || (Input.GetKeyDown(m_GlobalDataPlayer.secondarySlotKey) && m_PrimaryKeyUse))
            {
                MouseItem mouseItem = GetMouseClickInInventoryUI(localPoint, true, false);
                if(mouseItem.setRoot != null)
                {
                    DropCurrMouseItemInSlot(mouseItem, true);
                }
                else
                {
                    DropCurrMouseItemInWorld(true);
                }
            }

            //si le joueur continue de tenir appuyer la key actif fait suivre le content au UI de la souris
            if((Input.GetKey(m_GlobalDataPlayer.primarySlotKey) && m_PrimaryKeyUse) || (Input.GetKey(m_GlobalDataPlayer.secondarySlotKey) && m_SecondaryKeyUse))
            {
                UI.m_Instance.GetUIMouse().UpdateMouseContentPos(localPoint);
            }

            //si le joueur arret d'appuyer sur la key actif drop tout les resource de la souris
            if((Input.GetKeyUp(m_GlobalDataPlayer.primarySlotKey) && m_PrimaryKeyUse) || (Input.GetKeyUp(m_GlobalDataPlayer.secondarySlotKey) && m_SecondaryKeyUse))
            {
                MouseItem mouseItem = GetMouseClickInInventoryUI(localPoint, true, false);
                if (mouseItem.setRoot != null)
                {
                    DropCurrMouseItemInSlot(mouseItem, false);
                }
                else
                {
                    DropCurrMouseItemInWorld(false);
                }

                m_InventoryUsed = false;

                UI.m_Instance.GetUIMouse().SetActiveMouseContent(false);
            }
        }
    }

    private void DropCurrMouseItemInSlot(MouseItem slot, bool justOne)
    {
        if(m_CurrMouseItem.inventoryCase.resource.Equals(EnumSpecialResources.none))
        {
            return;
        }

        DataResource currMouseResource = (DataResource)Pool.m_Instance.GetData(m_CurrMouseItem.inventoryCase.resource);
        DataResource currSlotResource = (DataResource)Pool.m_Instance.GetData(slot.inventoryCase.resource);

        if(m_DataStoragePlayerEquip.CheckHasEquipementType(m_CurrMouseItem.inventoryCase.resource) && slot.onInventoryEquip)
        {
            SetCurrMouseItemRoot(m_CurrMouseItem.inventoryCase);
            UI.m_Instance.GetUIMouse().SetActiveMouseContent(false);
            return;
        }


        if (justOne)
        {
            if ((m_CurrMouseItem.inventoryCase.currNb > 0) &&
                ((slot.inventoryCase.resource.Equals(m_CurrMouseItem.inventoryCase.resource) && currMouseResource.maxStack > slot.inventoryCase.currNb) ||
                (slot.inventoryCase.resource.Equals(EnumSpecialResources.none))))
            {
                InventoryCase inventoryCase = new InventoryCase();
                inventoryCase.resource = m_CurrMouseItem.inventoryCase.resource;
                inventoryCase.currNb = slot.inventoryCase.currNb + 1;
                slot.setRoot(inventoryCase);

                m_CurrMouseItem.inventoryCase.currNb--;
                UI.m_Instance.GetUIMouse().SetValueMouseContent(m_CurrMouseItem.inventoryCase);
            }

            if (m_CurrMouseItem.inventoryCase.currNb <= 0)
            {
                m_CurrMouseItem.inventoryCase.resource = EnumSpecialResources.none;
                UI.m_Instance.GetUIMouse().SetActiveMouseContent(false);
            }
        }
        else
        {
            if (m_CurrMouseItem.inventoryCase.currNb > 0 && 
                (slot.inventoryCase.resource.Equals(m_CurrMouseItem.inventoryCase.resource) && currMouseResource.maxStack > slot.inventoryCase.currNb))
            {
                InventoryCase inventoryCase = new InventoryCase();
                inventoryCase.resource = m_CurrMouseItem.inventoryCase.resource;
                inventoryCase.currNb = slot.inventoryCase.currNb + m_CurrMouseItem.inventoryCase.currNb;

                int surplus = inventoryCase.currNb - currMouseResource.maxStack;
                if(surplus > 0)
                {
                    inventoryCase.currNb = currMouseResource.maxStack;
                    m_CurrMouseItem.inventoryCase.currNb = surplus;

                    slot.setRoot(inventoryCase);
                    SetCurrMouseItemRoot(m_CurrMouseItem.inventoryCase);
                }
                else
                {
                    slot.setRoot(inventoryCase);
                }
            }
            else
            {
                slot.setRoot(m_CurrMouseItem.inventoryCase);
                SetCurrMouseItemRoot(slot.inventoryCase);  
            }
            UI.m_Instance.GetUIMouse().SetActiveMouseContent(false);
        }

        if(m_CurrMouseItem.inventoryCase.resource is EnumEquipements || slot.inventoryCase.resource is EnumEquipements)
        {
            m_DataStoragePlayerEquip.UpdateEquipement();
        }
    }

    private void SetCurrMouseItemRoot(InventoryCase inventoryCase)
    {
        InventoryCase rootCase = m_CurrMouseItem.getRoot();
        if(rootCase.resource.Equals(EnumSpecialResources.none))
        {
            m_CurrMouseItem.setRoot(inventoryCase);
        }
        else
        {
            for(int i = 0; i < inventoryCase.currNb; i++)
            {
                if(!CollectResource(inventoryCase.resource))
                {
                    DropResource(inventoryCase.resource);
                }
            }
        }
    }

    private void DropCurrMouseItemInWorld(bool justOne)
    {
        if (m_CurrMouseItem.inventoryCase.resource.Equals(EnumSpecialResources.none))
        {
            return;
        }

        if (justOne)
        {
            if(m_CurrMouseItem.inventoryCase.currNb > 0)
            {
                m_CurrMouseItem.inventoryCase.currNb--;
                DropResource(m_CurrMouseItem.inventoryCase.resource);

                UI.m_Instance.GetUIMouse().SetValueMouseContent(m_CurrMouseItem.inventoryCase);
            }

            if (m_CurrMouseItem.inventoryCase.currNb <= 0)
            {
                m_CurrMouseItem.inventoryCase.resource = EnumSpecialResources.none;
                UI.m_Instance.GetUIMouse().SetActiveMouseContent(false);
            }
        }
        else
        {
            for(int i = 0; i < m_CurrMouseItem.inventoryCase.currNb; i++)
            {
                DropResource(m_CurrMouseItem.inventoryCase.resource);
            }
            m_CurrMouseItem.inventoryCase.resource = EnumSpecialResources.none;
            m_CurrMouseItem.inventoryCase.currNb = 0;
            UI.m_Instance.GetUIMouse().SetActiveMouseContent(false);
        }
    }

    //retourne les valeur de souris sur l'inventaire
    private MouseItem GetMouseClickInInventoryUI(Vector2 localPoint, bool primaryKey, bool pop)
    {
        MouseItem mouseItem = GetMouseClickInEquipUI(localPoint, primaryKey, pop);
        if (mouseItem.setRoot != null || mouseItem.inventoryCase.currNb != 0)
        {
            return mouseItem;
        }

        mouseItem = GetMouseClickInBackpackUI(localPoint, primaryKey, pop);
        if (mouseItem.setRoot != null || mouseItem.inventoryCase.currNb != 0)
        {
            return mouseItem;
        }

        return mouseItem;
    }

    //retourne les valeur si la souris est sur l'inventaire equiper du joueur
    private MouseItem GetMouseClickInEquipUI(Vector2 mousePoint, bool primaryKey, bool pop)
    {
        //prend le transform de tout les slot de l'inventaire equip
        List<Transform> slotsEquip = UI.m_Instance.GetUIPlayerEquip().GetAllSlots();

        // initialise les valeurs par default
        InventoryCase inventoryCase = new InventoryCase();
        inventoryCase.resource = EnumSpecialResources.none;
        inventoryCase.currNb = 0;

        MouseItem mouseItem = new MouseItem();
        mouseItem.inventoryCase = inventoryCase;
        mouseItem.setRoot = null;
        mouseItem.onInventoryEquip = true;

        //regarde la position sur l'inventaire equip
        int index = 0;
        foreach (Transform currSlot in slotsEquip)
        {
            RectTransform currTransform = currSlot.GetComponent<RectTransform>();
            Vector2 localPosition = currTransform.localPosition;
            Rect localRect = new Rect(localPosition.x - (currTransform.rect.width / 2), localPosition.y - (currTransform.rect.height / 2), currTransform.rect.width, currTransform.rect.height);
            if (localRect.Contains(mousePoint))
            {
                int paramIndex = index;
                Action<InventoryCase> setCaseAction = (caseItem) => m_DataStoragePlayerEquip.SetCase(paramIndex, caseItem);
                Func<InventoryCase> getCaseFunc = () => m_DataStoragePlayerEquip.GetCase(paramIndex);
                if (pop)
                {
                    if (primaryKey)
                    {
                        mouseItem.inventoryCase = m_DataStoragePlayerEquip.PopCase(index);
                    }
                    else
                    {
                        mouseItem.inventoryCase = m_DataStoragePlayerEquip.PopHalfCase(index);
                    }
                }
                else
                {
                    mouseItem.inventoryCase = m_DataStoragePlayerEquip.GetCase(index);
                }

                
                mouseItem.setRoot = setCaseAction;
                mouseItem.getRoot = getCaseFunc;
                return mouseItem;
            }
            index++;
        }

        return mouseItem;
    }

    private MouseItem GetMouseClickInBackpackUI(Vector2 mousePoint, bool primaryKey, bool pop)
    {
        //prend le transform de tout les slot de l'inventaire equip
        List<Transform> slotsEquip = UI.m_Instance.GetUIBackpack().GetAllSlots();

        // initialise les valeurs par default
        InventoryCase inventoryCase = new InventoryCase();
        inventoryCase.resource = EnumSpecialResources.none;
        inventoryCase.currNb = 0;

        MouseItem mouseItem = new MouseItem();
        mouseItem.inventoryCase = inventoryCase;
        mouseItem.setRoot = null;
        mouseItem.onInventoryEquip = false;

        if (!UI.m_Instance.GetUIBackpack().GetBackpackActive())
        {
            return mouseItem;
        }

        //regarde la position sur l'inventaire equip
        int index = 0;
        foreach (Transform currSlot in slotsEquip)
        {
            RectTransform currTransform = currSlot.GetComponent<RectTransform>();
            Vector2 localPosition = currTransform.localPosition;
            Rect localRect = new Rect(localPosition.x - (currTransform.rect.width / 2), localPosition.y - (currTransform.rect.height / 2), currTransform.rect.width, currTransform.rect.height);
            if (localRect.Contains(mousePoint))
            {
                int paramIndex = index;
                Action<InventoryCase> setCaseAction = (caseItem) => m_DataStoragePlayerBackpack.SetCase(paramIndex, caseItem);
                Func<InventoryCase> getCaseFunc = () => m_DataStoragePlayerBackpack.GetCase(paramIndex);
                if (pop)
                {
                    if (primaryKey)
                    {
                        mouseItem.inventoryCase = m_DataStoragePlayerBackpack.PopCase(index);
                    }
                    else
                    {
                        mouseItem.inventoryCase = m_DataStoragePlayerBackpack.PopHalfCase(index);
                    }
                }
                else
                {
                    mouseItem.inventoryCase = m_DataStoragePlayerBackpack.GetCase(index);
                }


                mouseItem.setRoot = setCaseAction;
                mouseItem.getRoot = getCaseFunc;
                return mouseItem;
            }
            index++;
        }

        return mouseItem;
    }

    private void DropResource(object dataResource)
    {
        StatePlayerControllerMovement statePlayerControllerMovement = (StatePlayerControllerMovement)m_StateMachine.GetState(EnumStatesPlayer.controllerMovement);
        int dirPlayer = statePlayerControllerMovement.GetPlayerDir();

        Vector3 pos = m_StateMachine.transform.position;
        pos.y += 1;
        pos.z -= 1;

        ResourceManager.m_Instance.InstanciateResourceInWorldAt(dataResource, pos, Vector2.right * dirPlayer * 100, dirPlayer);

        m_DataStoragePlayerEquip.UpdateEquipement();
    }

    public bool CollectResource(object resource)
    {
        if (m_DataStoragePlayerEquip.IncrementRessource(resource))
        {
            return true;
        }

        if (m_DataStoragePlayerBackpack.IncrementRessource(resource))
        {

            return true;
        }

        if(!m_DataStoragePlayerEquip.CheckHasEquipementType(resource))
        {
            if (m_DataStoragePlayerEquip.AddRessource(resource))
            {
                m_DataStoragePlayerEquip.UpdateEquipement();
                return true;
            }
        }
        
        if (m_DataStoragePlayerBackpack.AddRessource(resource))
        {
            
            return true;
        }

        return false;
    }

    public void DropEquipResource()
    {
        object resource = m_DataStoragePlayerEquip.DropOneAtCaseEquip();
        if(resource != null)
        {
            DropResource(resource);
        }
    }

    public bool ContainResource(object type, int nb)
    {
        if(m_DataStoragePlayerEquip.ContainResource(type, nb))
        {
            return true;
        }
        if(m_DataStoragePlayerBackpack.ContainResource(type, nb))
        {
            return true;
        }

        return false;
    }

    public void RemoveResource(object type, int nb)
    {
        for(int i = 0; i < nb; i++)
        {
            if(m_DataStoragePlayerEquip.DecrementRessource(type))
            {
                continue;
            }
            if(m_DataStoragePlayerBackpack.DecrementRessource(type))
            {
                continue;
            }
        }
    }
}
