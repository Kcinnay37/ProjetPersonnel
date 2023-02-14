using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class StatePlayerManageInventory : State
{
    Canvas m_Canvas;

    public StatePlayerManageInventory(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        
    }

    public override void End()
    {
        
    }

    public override void Update()
    {
        Transform slot = CheckRayUI();
        if(slot)
        {
            Debug.Log("test");
        }
    }

    public Transform CheckRayUI()
    {
        StateManagerManageUI stateManagerManagerUI = (StateManagerManageUI)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageUI);
        List<Transform> slotsEquip = stateManagerManagerUI.GetAllSlotInventoryEquip();

        if(m_Canvas == null)
        {
            m_Canvas = GameObject.FindObjectOfType<Canvas>();
        }

        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.transform as RectTransform, mousePosition, m_Canvas.worldCamera, out localPoint);

        foreach (Transform currSlot in slotsEquip)
        {
            RectTransform currTransform = currSlot.GetComponent<RectTransform>();
            Vector2 localPosition = currTransform.localPosition;
            Rect localRect = new Rect(localPosition.x - (currTransform.rect.width / 2), localPosition.y - (currTransform.rect.height / 2), currTransform.rect.width, currTransform.rect.height);
            if (localRect.Contains(localPoint))
            {
                return currTransform;
            }
        }

        return null;
    }
}
