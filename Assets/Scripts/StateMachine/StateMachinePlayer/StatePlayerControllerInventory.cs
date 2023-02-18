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
    }

    DataPlayer m_GlobalDataPlayer;

    Canvas m_Canvas;
    DataStorageManageUI m_DataStorageManageUI;

    MouseItem m_CurrMouseItem;

    private bool m_InventoryUsed;

    DataStoragePlayerEquip m_DataStoragePlayerEquip;
    DataStoragePlayerBackpack m_DataStoragePlayerBackpack;

    public StatePlayerControllerInventory(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        m_DataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        m_DataStoragePlayerBackpack = (DataStoragePlayerBackpack)m_StateMachine.GetDataStorage(EnumStatesPlayer.backpack);

        m_Canvas = GameObject.FindObjectOfType<Canvas>();

        m_DataStorageManageUI = (DataStorageManageUI)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageUI);
        m_DataStorageManageUI.AddCurrDataStorageUI(EnumStatesUI.mouseUI);    
    }

    public override void End()
    {
        
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            DataResource pickaxe = (DataResource)Pool.m_Instance.GetData(EnumTools.pickaxe);
            m_DataStoragePlayerEquip.AddRessource(pickaxe);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DataResource earth = (DataResource)Pool.m_Instance.GetData(EnumBlocks.earth);
            m_DataStoragePlayerEquip.AddRessource(earth);
        }

        //va chercher la position de la souris selon le canvas
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.transform as RectTransform, mousePosition, m_Canvas.worldCamera, out localPoint);
        
        //si le joueur appui sur la touche principal our secondaire
        if(Input.GetKeyDown(m_GlobalDataPlayer.primarySlotKey) || Input.GetKeyDown(m_GlobalDataPlayer.secondarySlotKey))
        {
            //regarde si il a appuiyer sur l'inventaire
            m_CurrMouseItem = GetMouseInInventoryUI(localPoint);

            if (m_CurrMouseItem.setRoot != null && m_CurrMouseItem.inventoryCase.currNb != 0)
            {
                m_InventoryUsed = true;

                m_DataStorageManageUI.SetValueMouseContent(m_CurrMouseItem.inventoryCase);
                m_DataStorageManageUI.SetActiveMouseContent(true);
            }

            //si il nest pas sur l'inventaire interagie avec les slot en question
            if (!m_InventoryUsed)
            {
                if(Input.GetKeyDown(m_GlobalDataPlayer.primarySlotKey))
                {
                    m_DataStoragePlayerEquip.Equip(0);
                }
                else if(Input.GetKeyDown(m_GlobalDataPlayer.secondarySlotKey))
                {
                    m_DataStoragePlayerEquip.Equip(1);
                }
            }
        }
        //sinon si le joueur tien une touche secondaire enfoncer
        else if(Input.GetKey(m_GlobalDataPlayer.primarySlotKey) || Input.GetKey(m_GlobalDataPlayer.secondarySlotKey))
        {
            //si l'inventaire n'est pas utiliser interagie avec la slot en question
            if (!m_InventoryUsed)
            {
                m_DataStoragePlayerEquip.ActionOldKey();
            }
            //sinon update le content que le joueur deplace
            else
            {
                m_DataStorageManageUI.UpdateMouseContentPos(localPoint);
            }
        }
        //sinon si le joueur relache une touche pricipal
        else if(Input.GetKeyUp(m_GlobalDataPlayer.primarySlotKey) || Input.GetKeyUp(m_GlobalDataPlayer.secondarySlotKey))
        {
            //si l'inventaire est utiliser
            if(m_InventoryUsed)
            {
                MouseItem mouseItem = GetMouseInInventoryUI(localPoint);
                if(mouseItem.setRoot != null)
                {
                    m_CurrMouseItem.setRoot(mouseItem.inventoryCase);
                    mouseItem.setRoot(m_CurrMouseItem.inventoryCase);
                }
                else
                {
                    m_CurrMouseItem.setRoot(m_CurrMouseItem.inventoryCase);
                }
            }

            m_InventoryUsed = false;

            m_DataStorageManageUI.SetActiveMouseContent(false);
        }
    }

    //public void UpdateContent(Vector2 localPoint)
    //{
    //    RectTransform rectTransform = m_ContentMouse.GetComponent<RectTransform>();
    //    rectTransform.position = new Vector3(localPoint.x, localPoint.y, 0);
    //}

    //retourne les valeur de souris sur l'inventaire
    private MouseItem GetMouseInInventoryUI(Vector2 localPoint)
    {
        MouseItem mouseItem = GetMouseInEquipUI(localPoint);
        if(m_CurrMouseItem.setRoot != null && m_CurrMouseItem.inventoryCase.currNb != 0)
        {
            return mouseItem;
        }

        return mouseItem;
    }

    //retourne les valeur si la souris est sur l'inventaire equiper du joueur
    public MouseItem GetMouseInEquipUI(Vector2 mousePoint)
    {
        //prend le transform de tout les slot de l'inventaire equip
        List<Transform> slotsEquip = m_DataStorageManageUI.GetAllSlotInventoryEquip();

        // initialise les valeurs par default
        InventoryCase inventoryCase = new InventoryCase();
        inventoryCase.resource = (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none);
        inventoryCase.currNb = 0;

        MouseItem mouseItem = new MouseItem();
        mouseItem.inventoryCase = inventoryCase;
        mouseItem.setRoot = null;

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

                mouseItem.inventoryCase = m_DataStoragePlayerEquip.PopCase(index);
                mouseItem.setRoot = setCaseAction;
                return mouseItem;
            }
            index++;
        }

        return mouseItem;
    }

    public void CollectRessource()
    {

    }
}
