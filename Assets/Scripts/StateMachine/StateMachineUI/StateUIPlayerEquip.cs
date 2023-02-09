using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateUIPlayerEquip : State
{
    GameObject m_UIPlayerEquip;

    public StateUIPlayerEquip(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        Canvas canvas = m_StateMachine.GetComponentInChildren<Canvas>();
        for(int i = 0; i < canvas.transform.childCount; i++)
        {
            if(canvas.transform.GetChild(i).name == "UIPlayerEquip")
            {
                m_UIPlayerEquip = canvas.transform.GetChild(i).gameObject;
                break;
            }
        }
        m_UIPlayerEquip?.SetActive(true);
    }

    public override void End()
    {
        m_UIPlayerEquip?.SetActive(false);
    }

    public void AddSlot()
    {
        GameObject slot = Pool.m_Instance.GetObject(EnumUI.slot);
        slot.transform.SetParent(m_UIPlayerEquip.transform);

        Vector3 position = m_UIPlayerEquip.transform.GetChild(m_UIPlayerEquip.transform.childCount - 2).transform.position;
        position.x += 100;
        slot.transform.position = position;
        slot.SetActive(true);
    }

    public void ChangeImageAt(int index, Sprite image)
    {
        GameObject slot = m_UIPlayerEquip.transform.GetChild(index).gameObject;
        slot.transform.GetChild(0).GetComponent<Image>().sprite = image;
    }
}
