using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerPickaxe : State
{
    DataOutil m_Data;
    GameObject m_Object;

    public StatePlayerPickaxe(StateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override void OnInit()
    {
        StatePlayerEquip inventoryState = (StatePlayerEquip)m_StateMachine.GetState(EnumState.inventory);
        EnumData dataName = inventoryState.GetCurrCase();

        m_Data = (DataOutil)Pool.m_Instance.GetDataResource(dataName);
        m_Object = Pool.m_Instance.GetGameObject(m_Data.gameObject);

        m_Object.transform.parent = GameObject.Find("PlayerWeaponSlot").transform;
        m_Object.SetActive(true);
        m_Object.transform.localPosition = Vector3.zero;
    }

    public override void End()
    {
        Pool.m_Instance.RemoveGameObject(m_Object, m_Data.gameObject);
    }
}
