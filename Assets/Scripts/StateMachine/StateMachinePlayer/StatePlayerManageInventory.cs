using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePlayerManageInventory : State
{
    public struct MouseItem
    {
        public InventoryCase inventoryCase;
        public Transform rootSlot;
    }

    DataPlayer m_DataPlayer;

    Canvas m_Canvas;
    StateManagerManageUI m_StateManagerManagerUI;

    GameObject m_ContentMouse;
    MouseItem m_CurrMouseItem;

    private bool m_InventoryUsed;

    StatePlayerEquip m_StatePlayerEquip;

    public StatePlayerManageInventory(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_DataPlayer = (DataPlayer)m_StateMachine.GetData();

        m_StatePlayerEquip = (StatePlayerEquip)m_StateMachine.GetState(EnumStatesPlayer.equip);

        m_Canvas = GameObject.FindObjectOfType<Canvas>();

        m_ContentMouse = Pool.m_Instance.GetObject(EnumUI.content);
    }

    public override void End()
    {
        
    }

    public override void Update()
    {
        //va chercher la position de la souris selon le canvas
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.transform as RectTransform, mousePosition, m_Canvas.worldCamera, out localPoint);

        if(Input.GetKeyDown(m_DataPlayer.primarySlotKey) || Input.GetKeyDown(m_DataPlayer.secondarySlotKey))
        {
            CheckMouseInventoryUI(localPoint);

            if (!m_InventoryUsed)
            {
                if(Input.GetKeyDown(m_DataPlayer.primarySlotKey))
                {
                    m_StatePlayerEquip.Equip(0);
                }
                else if(Input.GetKeyDown(m_DataPlayer.secondarySlotKey))
                {
                    m_StatePlayerEquip.Equip(1);
                }
            }
        }
        else if(Input.GetKey(m_DataPlayer.primarySlotKey) || Input.GetKey(m_DataPlayer.secondarySlotKey))
        {
            if (!m_InventoryUsed)
            {
                m_StatePlayerEquip.ActionOldKey();
            }
            else
            {
                UpdateContent(localPoint);
            }
        }
        else if(Input.GetKeyUp(m_DataPlayer.primarySlotKey) || Input.GetKeyUp(m_DataPlayer.secondarySlotKey))
        {
            m_InventoryUsed = false;
            m_ContentMouse.SetActive(false);
        }
    }

    public void UpdateContent(Vector2 localPoint)
    {
        RectTransform rectTransform = m_ContentMouse.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(localPoint.x, localPoint.y, 0);
    }

    private void CheckMouseInventoryUI(Vector2 localPoint)
    {
        m_StateManagerManagerUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);

        MouseItem mouseItem = GetUIEquip(localPoint);
        if(mouseItem.rootSlot != null)
        {
            m_InventoryUsed = true;

            m_ContentMouse.GetComponent<Image>().sprite = mouseItem.inventoryCase.resource.image;
            m_ContentMouse.SetActive(true);
        }
    }

    public MouseItem GetUIEquip(Vector2 mousePoint)
    {
        //prend le transform de tout les slot de l'inventaire equip
        List<Transform> slotsEquip = m_StateManagerManagerUI.GetAllSlotInventoryEquip();

        StatePlayerEquip statePlayerEquip = (StatePlayerEquip)m_StateMachine.GetState(EnumStatesPlayer.equip);

        // initialise les valeurs par default
        InventoryCase inventoryCase = new InventoryCase();
        inventoryCase.resource = (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none);
        inventoryCase.currNb = 0;

        MouseItem mouseItem = new MouseItem();
        mouseItem.inventoryCase = inventoryCase;
        mouseItem.rootSlot = null;

        //regarde la position sur l'inventaire equip
        int index = 0;
        foreach (Transform currSlot in slotsEquip)
        {
            RectTransform currTransform = currSlot.GetComponent<RectTransform>();
            Vector2 localPosition = currTransform.localPosition;
            Rect localRect = new Rect(localPosition.x - (currTransform.rect.width / 2), localPosition.y - (currTransform.rect.height / 2), currTransform.rect.width, currTransform.rect.height);
            if (localRect.Contains(mousePoint))
            {
                mouseItem.inventoryCase = statePlayerEquip.PopCase(index);
                mouseItem.rootSlot = currSlot;
                return mouseItem;
            }
            index++;
        }

        return mouseItem;
    }
}
