using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackpack
{
    private UI m_UI;

    private GameObject m_UIBackpack;

    private List<Transform> m_Slots;

    private bool m_BackpackActive;

    private Coroutine m_Coroutine;

    public UIBackpack(UI ui)
    {
        m_UI = ui;

        m_UIBackpack = GameObject.Find("UIBackpack");
        m_Slots = new List<Transform>();
        UpdateListSlot();

        m_Coroutine = m_UI.StartCoroutine(CoroutineCloseUI());
    }

    public void ChangeUIState()
    {
        if(m_Coroutine != null)
        {
            return;
        }

        if(m_BackpackActive)
        {
            m_Coroutine = m_UI.StartCoroutine(CoroutineCloseUI());
        }
        else
        {
            m_Coroutine = m_UI.StartCoroutine(CoroutineOpenUI());
        }
    }

    private IEnumerator CoroutineOpenUI()
    {
        m_UIBackpack.SetActive(true);

        yield return null;

        m_BackpackActive = true;
        m_Coroutine = null;
    }

    private IEnumerator CoroutineCloseUI()
    {
        m_BackpackActive = false;

        yield return null;
        
        m_UIBackpack.SetActive(false);
        m_Coroutine = null;
    }

    //Update la list de tout les transforms des slots
    private void UpdateListSlot()
    {
        m_Slots.Clear();

        Transform slots = m_UIBackpack.transform.Find("Slots");

        for (int i = 0; i < slots.childCount; i++)
        {
            m_Slots.Add(slots.GetChild(i));
        }
    }

    public void UpdateSlotAt(int index, InventoryCase inventoryCase)
    {
        DataResource resource = (DataResource)Pool.m_Instance.GetData(inventoryCase.resource);

        m_Slots[index].GetChild(0).GetComponent<Image>().sprite = resource.image;
        m_Slots[index].GetChild(0).GetChild(1).GetComponent<Text>().text = inventoryCase.currNb.ToString();

        if (resource.image == null || inventoryCase.currNb == 0)
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

    public bool GetBackpackActive()
    {
        return m_BackpackActive;
    }
}
