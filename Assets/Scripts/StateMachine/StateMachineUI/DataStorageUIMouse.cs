using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataStorageUIMouse : DataStorage
{
    GameObject m_ContentMouse;

    public DataStorageUIMouse(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_ContentMouse = Pool.m_Instance.GetObject(EnumUI.content);
        m_ContentMouse.transform.SetParent(GameObject.Find("MouseContent").transform);
    }

    public void SetActiveMouseContent(bool value)
    {
        m_ContentMouse.SetActive(value);
    }

    public void SetValueMouseContent(InventoryCase inventoryCase)
    {
        DataResource resource = (DataResource)Pool.m_Instance.GetData(inventoryCase.resource);

        m_ContentMouse.GetComponent<Image>().sprite = resource.image;
        m_ContentMouse.GetComponentInChildren<Text>().text = inventoryCase.currNb.ToString();
    }

    public void UpdateMouseContentPos(Vector2 pos)
    {
        RectTransform rectTransform = m_ContentMouse.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(pos.x, pos.y, 0);
    }
}
