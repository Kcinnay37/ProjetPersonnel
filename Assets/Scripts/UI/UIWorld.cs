using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorld
{
    private UI m_UI;

    private GameObject m_BlockUI;

    private GameObject m_WorldUI;

    public UIWorld(UI ui)
    {
        m_UI = ui;

        m_WorldUI = GameObject.Find("WorldUI");
    }

    public void InitBlockUI(Vector3Int localPos)
    {

        if(m_BlockUI == null)
        {
            m_BlockUI = Pool.m_Instance.GetObject(EnumUI.blockUI);
            m_BlockUI.transform.SetParent(m_WorldUI.transform);
        }

        RectTransform rectTransform = m_BlockUI.GetComponent<RectTransform>();

        Vector3 worldPosition = new Vector3(localPos.x + 0.5f, localPos.y + 0.5f, -2);
        rectTransform.localPosition = worldPosition;

        SetSlider(1, 1);

        m_BlockUI.SetActive(true);
    }

    public void SetSlider(float value, float maxValue)
    {
        if (m_BlockUI == null)
        {
            m_BlockUI = Pool.m_Instance.GetObject(EnumUI.blockUI);
            m_BlockUI.transform.SetParent(m_WorldUI.transform);
        }

        Slider slider = m_BlockUI.GetComponentInChildren<Slider>();

        slider.value = value / maxValue;
    }

    public void EndBlockUI()
    {
        if (m_BlockUI == null) return;
        m_BlockUI.SetActive(false);
    }
}
