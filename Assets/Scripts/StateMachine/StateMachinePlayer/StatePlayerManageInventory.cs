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
        RayUI();
    }

    public void RayUI()
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
            RectTransform transform = currSlot.GetComponent<RectTransform>();
            Vector2 localPosition = transform.localPosition;
            Rect localRect = new Rect(localPosition.x - (transform.rect.width / 2), localPosition.y - (transform.rect.height / 2), transform.rect.width, transform.rect.height);
            if (localRect.Contains(localPoint))
            {
                Debug.Log("test");
            }
            else
            {

            }
        }
    }

    public void RayWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null)
        {
            
        }
    }
}
