//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class StatePlayerEquipInventory : State
//{
//    DataPlayer m_Data;

//    List<EnumData> m_Inventory;
//    List<GameObject> m_Cases;

//    int m_CaseSelected;

//    public StatePlayerEquipInventory(StateMachine stateMachine) : base(stateMachine)
//    {
//        m_Data = (DataPlayer)m_StateMachine.GetData();

//        m_Inventory = new List<EnumData>();
//        m_Cases = new List<GameObject>();

//        m_CaseSelected = 0;
//    }

//    public override void OnInit()
//    {
//        InitInventory();
//        AddInInventory(EnumData.pickaxe);
//        EventManager.TriggerEvent("DrawEquipInventory", new Dictionary<string, object> { { "inventoryEquip", m_Inventory } });
//    }

//    public override void Update()
//    {
//        CheckInput();
//    }

//    // a retravailler
//    private void InitInventory()
//    {
//        //aller lire les data dans la save game...

//        m_Inventory.Clear();

//        //aller inserer les data de la save game...

//        for (int i = 0; i < m_Data.sizeInventoryEquip; i++)
//        {
//            m_Inventory.Add(EnumData.none);
//            m_Cases.Add(null);
//        }
//    }

//    // a retravailler
//    private void AddInInventory(EnumData data)
//    {
//        for (int i = 0; i < m_Data.sizeInventoryEquip; i++)
//        {
//            if (m_Inventory[i] == EnumData.none)
//            {
//                m_Inventory[i] = data;
//                break;
//            }
//        }
//    }

//    //private void DrawInventory()
//    //{
//    //    for (int i = 0; i < m_Data.sizeInventoryEquip; i++)
//    //    {
//    //        if (m_Inventory[i] != EnumData.none)
//    //        {
//    //            GameObject currCase = m_Cases[i];
//    //            if (currCase == null)
//    //            {
//    //                currCase = GameObject.Find("Case" + i.ToString());
//    //            }

//    //            DataResource data = (DataResource)Pool.m_Instance.GetDataResource(m_Inventory[i]);

//    //            currCase.GetComponent<Image>().sprite = data.image;
//    //        }
//    //    }
//    //}

//    private void CheckInput()
//    {
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            m_CaseSelected = 0;
//            SelectCase();
//        }
//    }

//    private void SelectCase()
//    {
//        m_StateMachine.PopCurrState(EnumState.playerPickaxe);

//        DataResource data = (DataResource)Pool.m_Instance.GetData(m_Inventory[m_CaseSelected]);

//        m_StateMachine.AddCurrState(data.state);
//    }

//    public EnumData GetCurrCase()
//    {
//        return m_Inventory[m_CaseSelected];
//    }
//}
