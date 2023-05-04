using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private EnumAddInWorld m_Data;

    List<UIShop.UIItem> UIItems;

    private void OnEnable()
    {
        DataShop dataShop = (DataShop)Pool.m_Instance.GetData(m_Data);
        InitItemInShop(dataShop);
    }

    private void OnDisable()
    {
        UIItems.Clear();
    }

    private void InitItemInShop(DataShop dataShop)
    {
        UIItems = new List<UIShop.UIItem>();
        List<UIShop.UIItem> tempUIItems = new List<UIShop.UIItem>();

        foreach (DataShop.ConsumableInShop item in dataShop.itemShop.consumableInShop)
        {
            UIShop.UIItem currItem = new UIShop.UIItem();
            currItem.item = (DataResource)Pool.m_Instance.GetData(item.type);
            currItem.recipe = item.recipe;
            tempUIItems.Add(currItem);
        }

        foreach (DataShop.EquipementInShop item in dataShop.itemShop.equipementInShop)
        {
            UIShop.UIItem currItem = new UIShop.UIItem();
            currItem.item = (DataResource)Pool.m_Instance.GetData(item.type);
            currItem.recipe = item.recipe;
            tempUIItems.Add(currItem);
        }

        foreach (DataShop.MaterialInShop item in dataShop.itemShop.materialInShop)
        {
            UIShop.UIItem currItem = new UIShop.UIItem();
            currItem.item = (DataResource)Pool.m_Instance.GetData(item.type);
            currItem.recipe = item.recipe;
            tempUIItems.Add(currItem);
        }

        foreach (DataShop.ToolInShop item in dataShop.itemShop.toolInShop)
        {
            UIShop.UIItem currItem = new UIShop.UIItem();
            currItem.item = (DataResource)Pool.m_Instance.GetData(item.type);
            currItem.recipe = item.recipe;
            tempUIItems.Add(currItem);
        }

        foreach (DataShop.MountInShop item in dataShop.itemShop.mountInShop)
        {
            UIShop.UIItem currItem = new UIShop.UIItem();
            currItem.item = (DataResource)Pool.m_Instance.GetData(item.type);
            currItem.recipe = item.recipe;
            tempUIItems.Add(currItem);
        }

        for (int i = 0; i < dataShop.nbItem; i++)
        {
            if (tempUIItems.Count == 0)
            {
                break;
            }

            int index = Random.Range(0, tempUIItems.Count);
            UIItems.Add(tempUIItems[index]);
            tempUIItems.RemoveAt(index);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AudioManager.m_Instance.PlaySoundAt(transform.position, EnumAudios.shopOpen);
            UI.m_Instance.GetUIShop().InitUI(UIItems);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            AudioManager.m_Instance.PlaySoundAt(transform.position, EnumAudios.shopClose);
            UI.m_Instance.GetUIShop().CloseUI();
        }
    }
}
