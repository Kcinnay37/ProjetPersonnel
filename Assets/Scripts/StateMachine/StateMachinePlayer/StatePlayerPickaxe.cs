using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerPickaxe : State
{
    DataPlayer m_DataPlayer;

    DataOutil m_Data;
    GameObject m_Object;

    public StatePlayerPickaxe(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_DataPlayer = (DataPlayer)m_StateMachine.GetData();

        m_Object = Pool.m_Instance.GetObject(EnumTools.pickaxe);

        m_Object.transform.parent = GameObject.Find("PlayerWeaponSlot").transform;
        m_Object.SetActive(true);
        m_Object.transform.localPosition = Vector3.zero;
    }

    public override void Update()
    {
        if(Input.GetKeyDown(m_DataPlayer.primarySlotKey))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

            StateManagerManageMap manageMap = (StateManagerManageMap)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageMap);

            manageMap.PopBlockAt(mouseWorldPosition);
        }
    }

    public override void End()
    {
        Pool.m_Instance.RemoveObject(m_Object, EnumTools.pickaxe);
    }
}