using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorageUIWorld : DataStorage
{
    GameObject cellUI;

    public DataStorageUIWorld(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        cellUI = Pool.m_Instance.GetObject(EnumUI.cellUI);
    }

    public override void End()
    {
        
    }

    
}
