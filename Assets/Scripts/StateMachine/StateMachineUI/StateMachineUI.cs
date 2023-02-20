using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineUI : StateMachine
{
    [SerializeField] EnumUI m_Data;

    public override void AddInitialsStatesAndData()
    {
        AddCurrDataStorage(EnumStatesUI.worldUI);
    }

    public override void InitAllStatesAndData()
    {
        AddNewDataStorage(EnumStatesUI.playerEquipUI, new DataStorageUIPlayerEquip(this));
        AddNewDataStorage(EnumStatesUI.mouseUI, new DataStorageUIMouse(this));
        AddNewDataStorage(EnumStatesUI.worldUI, new DataStorageUIWorld(this));
    }

    public override ScriptableObject GetData()
    {
        return (DataUI)Pool.m_Instance.GetData(m_Data);
    }
}
