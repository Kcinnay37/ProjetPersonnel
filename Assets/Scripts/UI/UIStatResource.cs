using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatResource : MonoBehaviour
{
    private UI m_UI;
    private GameObject m_ObjectUIStatPlayer;

    private bool m_UIIsActive;

    public UIStatResource(UI ui)
    {
        m_UI = ui;

        m_ObjectUIStatPlayer = m_UI.transform.Find("UIScreen").Find("UIStatResource").gameObject;
        m_ObjectUIStatPlayer.SetActive(false);
        m_UIIsActive = false;
    }

    public void ShowResource(object resource)
    {
        if (m_UIIsActive) return;

        m_ObjectUIStatPlayer.SetActive(true);

        Text text = m_ObjectUIStatPlayer.transform.Find("Image").Find("Text").GetComponent<Text>();

        DataResource dataResource = (DataResource)Pool.m_Instance.GetData(resource);

        text.text = "";

        text.text = "name: " + dataResource.GetDataType().ToString() + "\n\n";

        text.text += dataResource.GetText();

        m_UIIsActive = true;
    }

    public void UnshowResource()
    {
        m_ObjectUIStatPlayer.transform.Find("Image").Find("Text").GetComponent<Text>().text = "";

        m_ObjectUIStatPlayer.SetActive(false);

        m_UIIsActive = false;
    }
}
