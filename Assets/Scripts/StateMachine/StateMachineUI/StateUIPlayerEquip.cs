using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateUIPlayerEquip : State
{
    private GameObject m_UIPlayerEquip;

    private List<Transform> m_Slots;

    public StateUIPlayerEquip(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_Slots = new List<Transform>();

        Canvas canvas = m_StateMachine.GetComponentInChildren<Canvas>();
        for(int i = 0; i < canvas.transform.childCount; i++)
        {
            if(canvas.transform.GetChild(i).name == "UIPlayerEquip")
            {
                m_UIPlayerEquip = canvas.transform.GetChild(i).gameObject;
                break;
            }
        }

        UpdateListSlot();

        StateManagerManagePlayer stateManagerManagePlayer = (StateManagerManagePlayer)StateMachineManager.m_Instance.GetState(EnumStatesManager.managePlayer);
        stateManagerManagePlayer.InitEquipUI();

        m_UIPlayerEquip?.SetActive(true);
    }

    public override void End()
    {
        m_UIPlayerEquip?.SetActive(false);
    }
    
    //ajoute une slot dans le UI
    public void AddSlot()
    {
        GameObject slot = Pool.m_Instance.GetObject(EnumUI.slot);
        slot.transform.SetParent(m_UIPlayerEquip.transform);

        Vector3 position = m_UIPlayerEquip.transform.GetChild(m_UIPlayerEquip.transform.childCount - 2).transform.position;
        position.x += 100;
        slot.transform.position = position;
        slot.SetActive(true);

        UpdateListSlot();
    }

    //Update la list de tout les transforms des slots
    private void UpdateListSlot()
    {
        m_Slots.Clear();
        for(int i = 0; i < m_UIPlayerEquip.transform.childCount; i++)
        {
            m_Slots.Add(m_UIPlayerEquip.transform.GetChild(i));
        }
    }

    public void UpdateSlotAt(int index, InventoryCase inventoryCase)
    {
        m_Slots[index].GetChild(0).GetComponent<Image>().sprite = inventoryCase.resource.image;
        m_Slots[index].GetChild(0).GetChild(0).GetComponent<Text>().text = inventoryCase.currNb.ToString();

        if(inventoryCase.resource.image == null || inventoryCase.currNb == 0)
        {
            m_Slots[index].GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            m_Slots[index].GetChild(0).gameObject.SetActive(true);
        }
    }

    public void ChangeImageAt(int index, Sprite image)
    {
        //GameObject slot = m_UIPlayerEquip.transform.GetChild(index).gameObject;
        m_Slots[index].GetChild(0).GetComponent<Image>().sprite = image;
        
        if(image == null)
        {
            m_Slots[index].GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            m_Slots[index].GetChild(0).gameObject.SetActive(true);
        }
    }

    //Retourne la list de tout les transform des slot dans le UI
    public List<Transform> GetAllSlots()
    {
        return m_Slots;
    }
}
