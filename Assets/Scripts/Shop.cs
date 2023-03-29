using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private EnumAddInWorld m_Data;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            DataShop dataShop = (DataShop)Pool.m_Instance.GetData(m_Data);
            UI.m_Instance.GetUIShop().InitUI(dataShop.itemShop);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            UI.m_Instance.GetUIShop().CloseUI();
        }
    }
}
