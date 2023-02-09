using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineManager : StateMachine
{
    [SerializeField] EnumManagers m_Data;

    public static StateMachineManager m_Instance;

    public override void AddInitialsStates()
    {
        m_Instance = this;

        AddCurrState(EnumStatesManager.manageMap);
    }

    public override void InitAllStates()
    {
        AddNewState(EnumStatesManager.manageMap, new StateManagerManageMap(this));
        AddNewState(EnumStatesManager.managePlayer, new StateManagerManagePlayer(this));
        AddNewState(EnumStatesManager.manageUI, new StateManagerManageUI(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
