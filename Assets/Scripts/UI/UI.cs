using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private EnumUI m_DataUI;

    private UIWorld m_UIWorld;
    private UIMouse m_UIMouse;
    private UIPlayerEquip m_UIPlayerEquip;
    private UIBackpack m_UIBackpack;
    private UIShop m_UIShop;
    private UIPlayerStat m_UIPlayerStat;

    public static UI m_Instance;

    private void Awake()
    {
        m_Instance = this;

        m_UIWorld = new UIWorld(this);
        m_UIMouse = new UIMouse(this);
        m_UIPlayerEquip = new UIPlayerEquip(this);
        m_UIBackpack = new UIBackpack(this);
        m_UIShop = new UIShop(this);
        m_UIPlayerStat = new UIPlayerStat(this);
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

    public UIBackpack GetUIBackpack()
    {
        return m_UIBackpack;
    }

    public UIShop GetUIShop()
    {
        return m_UIShop;
    }

    public UIPlayerStat GetUIPlayerStat()
    {
        return m_UIPlayerStat;
    }

    public void Buy()
    {
        if(m_UIShop != null)
        {
            m_UIShop.Buy();
        }
    }

    public void CheckClick()
    {
        if(m_UIShop != null)
        {
            m_UIShop.CheckClick();
        }
    }

    public void ReloadScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
