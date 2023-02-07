using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineManager : StateMachine
{
    [SerializeField] EnumManagers m_Data;

    public override void AddInitialsStates()
    {
        AddCurrState(EnumStatesManager.initGame);
    }

    public override void InitAllStates()
    {
        AddNewStateData(EnumStatesManager.data, new StateManagerData(this));
        AddNewState(EnumStatesManager.initGame, new StateManagerInitGame(this));
        AddNewState(EnumStatesManager.spawn, new StateManagerSpawn(this));
    }

    public override ScriptableObject GetData()
    {
        return Pool.m_Instance.GetData(m_Data);
    }
}
