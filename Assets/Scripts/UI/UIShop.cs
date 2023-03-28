using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShop
{
    private UI m_UI;
    private GameObject m_UIView;

    public UIShop(UI ui)
    {
        m_UI = ui;
    }

    public void InitUI()
    {
        if(m_UIView == null)
        {
            m_UIView = m_UI.transform.Find("UIScreen").Find("UIShop").gameObject;
        }
        m_UIView.SetActive(true);
    }

    public void CloseUI()
    {
        if (m_UIView == null)
        {
            m_UIView = m_UI.transform.Find("UIScreen").Find("UIShop").gameObject;
        }
        m_UIView.SetActive(false);
    }
}
