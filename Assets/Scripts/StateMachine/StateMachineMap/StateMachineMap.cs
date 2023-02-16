using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineMap : StateMachine
{
    [SerializeField] EnumMaps m_Data;

    public override void AddInitialsStatesAndData()
    {
        AddCurrDataStorage(EnumStatesMap.generate);
        AddCurrDataStorage(EnumStatesMap.view);
        AddCurrDataStorage(EnumStatesMap.grid);
    }

    public override void InitAllStatesAndData()
    {
        AddNewDataStorage(EnumStatesMap.generate, new DataStorageMapGenerate(this));
        AddNewDataStorage(EnumStatesMap.view, new DataStorageMapView(this));
        AddNewDataStorage(EnumStatesMap.grid, new DataStorageMapGrid(this));
    }

    public override ScriptableObject GetData()
    {
        return (DataMap)Pool.m_Instance.GetData(m_Data);
    }
}
