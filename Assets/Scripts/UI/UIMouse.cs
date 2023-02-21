using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMouse
{
    private UI m_UI;

    private GameObject m_ContentMouse;

    public UIMouse(UI ui)
    {
        m_UI = ui;
    }

    public void SetActiveMouseContent(bool value)
    {
        if(m_ContentMouse == null)
        {
            m_ContentMouse = Pool.m_Instance.GetObject(EnumUI.content);
            m_ContentMouse.transform.SetParent(GameObject.Find("MouseContent").transform);
        }

        m_ContentMouse.SetActive(value);
    }

    public void SetValueMouseContent(InventoryCase inventoryCase)
    {
        if (m_ContentMouse == null)
        {
            m_ContentMouse = Pool.m_Instance.GetObject(EnumUI.content);
            m_ContentMouse.transform.SetParent(GameObject.Find("MouseContent").transform);
        }

        DataResource resource = (DataResource)Pool.m_Instance.GetData(inventoryCase.resource);

        m_ContentMouse.GetComponent<Image>().sprite = resource.image;
        m_ContentMouse.GetComponentInChildren<Text>().text = inventoryCase.currNb.ToString();
    }

    public void UpdateMouseContentPos(Vector2 pos)
    {
        if (m_ContentMouse == null)
        {
            m_ContentMouse = Pool.m_Instance.GetObject(EnumUI.content);
            m_ContentMouse.transform.SetParent(GameObject.Find("MouseContent").transform);
        }

        RectTransform rectTransform = m_ContentMouse.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(pos.x, pos.y, 0);
    }
}
