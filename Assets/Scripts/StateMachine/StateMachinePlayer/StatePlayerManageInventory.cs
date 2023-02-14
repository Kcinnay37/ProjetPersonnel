using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatePlayerManageInventory : State
{
    struct MouseItem
    {
        InventoryCase inventoryCase;
        Transform rootSlot;
    }

    Canvas m_Canvas;
    StateManagerManageUI m_StateManagerManagerUI;

    GameObject m_ContentMouse;

    public StatePlayerManageInventory(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_Canvas = GameObject.FindObjectOfType<Canvas>();

        m_ContentMouse = Pool.m_Instance.GetObject(EnumUI.content);
        m_ContentMouse.SetActive(true);
    }

    public override void End()
    {
        
    }

    public override void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.transform as RectTransform, mousePosition, m_Canvas.worldCamera, out localPoint);

        CheckMouseInventoryUI(localPoint);

        UpdateContent(localPoint);
    }

    public void UpdateContent(Vector2 localPoint)
    {
        RectTransform rectTransform = m_ContentMouse.GetComponent<RectTransform>();
        rectTransform.position = new Vector3(localPoint.x, localPoint.y, 0);
    }

    private void CheckMouseInventoryUI(Vector2 localPoint)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_StateManagerManagerUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);

            InventoryCase inventoryCase = GetUIEquip(localPoint);
            if(inventoryCase.currNb != 0)
            {
                Debug.Log(inventoryCase.resource);
            }
        }
    }

    public InventoryCase GetUIEquip(Vector2 mousePoint)
    {
        //prend le transform de tout les slot de l'inventaire equip
        List<Transform> slotsEquip = m_StateManagerManagerUI.GetAllSlotInventoryEquip();

        StatePlayerEquip statePlayerEquip = (StatePlayerEquip)m_StateMachine.GetState(EnumStatesPlayer.equip);

        InventoryCase inventoryCase = new InventoryCase();
        inventoryCase.resource = (DataResource)Pool.m_Instance.GetData(EnumSpecialResources.none);
        inventoryCase.currNb = 0;

        //regarde la position sur l'inventaire equip
        int index = 0;
        foreach (Transform currSlot in slotsEquip)
        {
            RectTransform currTransform = currSlot.GetComponent<RectTransform>();
            Vector2 localPosition = currTransform.localPosition;
            Rect localRect = new Rect(localPosition.x - (currTransform.rect.width / 2), localPosition.y - (currTransform.rect.height / 2), currTransform.rect.width, currTransform.rect.height);
            if (localRect.Contains(mousePoint))
            {
                inventoryCase = statePlayerEquip.PopCase(index);
                return inventoryCase;
            }
            index++;
        }
        
        return inventoryCase;
    }
}
