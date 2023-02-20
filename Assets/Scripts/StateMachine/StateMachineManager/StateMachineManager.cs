using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineManager : StateMachine
{
    [SerializeField] EnumManagers m_Data;

    public static StateMachineManager m_Instance;

    public override void AddInitialsStatesAndData()
    {
        m_Instance = this;

        AddCurrDataStorage(EnumStatesManager.manageMap);
    }

    public override void InitAllStatesAndData()
    {
        AddNewDataStorage(EnumStatesManager.manageMap, new DataStorageManageMap(this));
        AddNewDataStorage(EnumStatesManager.managePlayer, new DataStorageManagePlayer(this));
        AddNewDataStorage(EnumStatesManager.manageUI, new DataStorageManageUI(this));
        AddNewDataStorage(EnumStatesManager.manageResource, new DataStorageManageResource(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
