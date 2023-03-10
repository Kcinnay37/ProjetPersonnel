using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private EnumUI m_DataUI;

    private UIWorld m_UIWorld;
    private UIMouse m_UIMouse;
    private UIPlayerEquip m_UIPlayerEquip;

    public static UI m_Instance;

    private void Awake()
    {
        m_Instance = this;

        m_UIWorld = new UIWorld(this);
        m_UIMouse = new UIMouse(this);
        m_UIPlayerEquip = new UIPlayerEquip(this);
    }

    public DataUI GetData()
    {
        return (DataUI)Pool.m_Instance.GetData(m_DataUI);
    }

    public UIWorld GetUIWorld()
    {
        return m_UIWorld;
    }

    public UIMouse GetUIMouse()
    {
        return m_UIMouse;
    }

    public UIPlayerEquip GetUIPlayerEquip()
    {
        return m_UIPlayerEquip;
    }
}
