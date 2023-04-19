using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop
{
    public struct UIItem
    {
        public DataResource item;
        public DataShop.Recipe recipe;
    }

    public struct UIItemInRecipe
    {
        public DataResource item;
        public int nb;
    }

    private UI m_UI;
    private GameObject m_UIView;

    List<Transform> m_SlotsShop;
    List<Transform> m_SlotsRecipe;

    Dictionary<Transform, UIItem> m_DictItemInShop;

    private UIItem m_SelectedItem;
    private List<UIItemInRecipe> m_CurrRecipe;

    private bool m_ShopIsOpen;

    public UIShop(UI ui)
    {
        m_UI = ui;
        m_ShopIsOpen = false;
    }

    public void InitUI(List<UIItem> UIItems)
    {
        if(m_UIView == null)
        {
            m_UIView = m_UI.transform.Find("UIScreen").Find("UIShop").gameObject;
        }
        m_UIView.SetActive(true);

        m_ShopIsOpen = true;

        m_SlotsShop = new List<Transform>();
        m_SlotsRecipe = new List<Transform>();
        m_CurrRecipe = new List<UIItemInRecipe>();

        Transform slotShop = m_UIView.transform.Find("SlotShop");
        for (int i = 0; i < slotShop.childCount; i++)
        {
            m_SlotsShop.Add(slotShop.GetChild(i));
            slotShop.GetChild(i).gameObject.SetActive(false);
        }

        Transform slotRecipe = m_UIView.transform.Find("SlotRecipe");
        for (int i = 0; i < slotRecipe.childCount; i++)
        {
            m_SlotsRecipe.Add(slotRecipe.GetChild(i));
            slotRecipe.GetChild(i).gameObject.SetActive(false);
        }

        m_SelectedItem = new UIItem();
        m_SelectedItem.item = null;

        InitItemInShop(UIItems);
    }

    public void CloseUI()
    {
        if (m_UIView == null)
        {
            m_UIView = m_UI.transform.Find("UIScreen").Find("UIShop").gameObject;
        }
        m_UIView.SetActive(false);

        m_ShopIsOpen = false;

        m_SlotsShop.Clear();
        m_SlotsRecipe.Clear();
        m_DictItemInShop.Clear();
        m_CurrRecipe.Clear();
    }

    private void InitItemInShop(List<UIItem> UIItems)
    {
        m_DictItemInShop = new Dictionary<Transform, UIItem>();

        for(int i = 0; i < UIItems.Count; i++)
        {
            m_DictItemInShop.Add(m_SlotsShop[i], UIItems[i]);
        }

        foreach(KeyValuePair<Transform, UIItem> keyValue in m_DictItemInShop)
        {
            keyValue.Key.gameObject.SetActive(true);
            keyValue.Key.GetComponent<Image>().sprite = keyValue.Value.item.image;
        }
    }

    public void CheckClick()
    {
        foreach (KeyValuePair<Transform, UIItem> keyValue in m_DictItemInShop)
        {
            RectTransform currTransform = keyValue.Key.GetComponent<RectTransform>();
            Vector2 localPosition = currTransform.localPosition;
            Rect localRect = new Rect(localPosition.x - (currTransform.rect.width / 2), localPosition.y - (currTransform.rect.height / 2), currTransform.rect.width, currTransform.rect.height);

            Canvas canvas = m_UIView.transform.parent.GetComponent<Canvas>();
            Vector2 mousePosition = Input.mousePosition;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out localPoint);

            if (localRect.Contains(localPoint))
            {
                SelectItem(keyValue.Value);
                break;
            }
        }
    }

    private void SelectItem(UIItem item)
    {
        m_SelectedItem = item;
        m_CurrRecipe.Clear();

        foreach(Transform slot in m_SlotsRecipe)
        {
            slot.gameObject.SetActive(false);
        }

        foreach (DataShop.ConsumableInRecipe value in item.recipe.consumableInRecipe)
        {
            UIItemInRecipe currItemInRecipe = new UIItemInRecipe();
            currItemInRecipe.item = (DataResource)Pool.m_Instance.GetData(value.type);
            currItemInRecipe.nb = value.nb;
            m_CurrRecipe.Add(currItemInRecipe);
        }

        foreach (DataShop.EquipementInRecipe value in item.recipe.equipementInRecipe)
        {
            UIItemInRecipe currItemInRecipe = new UIItemInRecipe();
            currItemInRecipe.item = (DataResource)Pool.m_Instance.GetData(value.type);
            currItemInRecipe.nb = value.nb;
            m_CurrRecipe.Add(currItemInRecipe);
        }

        foreach (DataShop.MaterialInRecipe value in item.recipe.materialInRecipe)
        {
            UIItemInRecipe currItemInRecipe = new UIItemInRecipe();
            currItemInRecipe.item = (DataResource)Pool.m_Instance.GetData(value.type);
            currItemInRecipe.nb = value.nb;
            m_CurrRecipe.Add(currItemInRecipe);
        }

        foreach (DataShop.ToolInRecipe value in item.recipe.toolInRecipe)
        {
            UIItemInRecipe currItemInRecipe = new UIItemInRecipe();
            currItemInRecipe.item = (DataResource)Pool.m_Instance.GetData(value.type);
            currItemInRecipe.nb = value.nb;
            m_CurrRecipe.Add(currItemInRecipe);
        }

        foreach (DataShop.MountInRecipe value in item.recipe.mountInRecipe)
        {
            UIItemInRecipe currItemInRecipe = new UIItemInRecipe();
            currItemInRecipe.item = (DataResource)Pool.m_Instance.GetData(value.type);
            currItemInRecipe.nb = value.nb;
            m_CurrRecipe.Add(currItemInRecipe);
        }

        foreach (DataShop.BlockInRecipe value in item.recipe.blockInRecipe)
        {
            UIItemInRecipe currItemInRecipe = new UIItemInRecipe();
            currItemInRecipe.item = (DataResource)Pool.m_Instance.GetData(value.type);
            currItemInRecipe.nb = value.nb;
            m_CurrRecipe.Add(currItemInRecipe);
        }

        int count = 0;
        foreach(UIItemInRecipe itemInRecipe in m_CurrRecipe)
        {
            m_SlotsRecipe[count].gameObject.SetActive(true);
            m_SlotsRecipe[count].GetComponent<Image>().sprite = itemInRecipe.item.image;
            m_SlotsRecipe[count].GetChild(0).GetChild(0).GetComponent<Text>().text = itemInRecipe.nb.ToString();
            count++;
        }
    }

    public void Buy()
    {
        if(m_SelectedItem.item != null)
        {
            StatePlayerControllerInventory statePlayerControllerInventory = (StatePlayerControllerInventory)PlayerManager.m_Instance.CurrPlayerGetState(EnumStatesPlayer.controllerInventory);
            if (statePlayerControllerInventory != null)
            {
                foreach(UIItemInRecipe itemInRecipe in m_CurrRecipe)
                {
                    if(!statePlayerControllerInventory.ContainResource(itemInRecipe.item.GetDataType(), itemInRecipe.nb))
                    {
                        return;
                    }
                }

                if(PlayerManager.m_Instance.CurrPlayerCollectResource(m_SelectedItem.item.GetDataType()))
                {
                    foreach (UIItemInRecipe itemInRecipe in m_CurrRecipe)
                    {
                        statePlayerControllerInventory.RemoveResource(itemInRecipe.item.GetDataType(), itemInRecipe.nb);
                    }
                }
            }
        }
    } 

    public List<Transform> GetAllSlots()
    {
        return m_SlotsShop;
    }

    public bool GetShopIsOpen()
    {
        return m_ShopIsOpen;
    }

    public object GetItemAt(Transform slot)
    {
        if(m_DictItemInShop.ContainsKey(slot))
        {
            return m_DictItemInShop[slot].item.GetDataType();
        }
        return null;
    }
}
