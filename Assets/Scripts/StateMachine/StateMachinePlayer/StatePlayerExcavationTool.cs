using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerExcavationTool : StateRessource
{
    private DataTool m_DataTool;
    private GameObject m_Object;

    public StatePlayerExcavationTool(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        InventoryCase caseEquip = dataStoragePlayerEquip.GetEquipCase();
        m_DataTool = (DataTool)Pool.m_Instance.GetData(caseEquip.resource);

        //Instanci l'outil
        m_Object = Pool.m_Instance.GetObject(m_DataTool.instanceType);

        Vector3 scale = m_Object.transform.localScale;
        if ((m_StateMachine.transform.localScale.x < 0 && scale.x > 0) || (m_StateMachine.transform.localScale.x > 0 && scale.x < 0))
        {
            scale.x = -scale.x;
        }
        m_Object.transform.localScale = scale;

        m_Object.transform.parent = GameObject.Find("PlayerWeaponSlot").transform;
        m_Object.SetActive(true);

        m_Object.transform.localPosition = Vector3.zero;

        m_Object.transform.localRotation = Quaternion.identity;
    }

    public override void End()
    {
        Pool.m_Instance.RemoveObject(m_Object, m_DataTool.instanceType);
        m_DataTool = null;
    }

    public override void Update()
    {
        
    }

    public override void ActionKeyDown()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

        DataStorageManageMap dataStorageManageMap = (DataStorageManageMap)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageMap);

        dataStorageManageMap.PopBlockAt(mouseWorldPosition);
    }

    public override void ActionOldKey()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

        DataStorageManageMap dataStorageManageMap = (DataStorageManageMap)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageMap);

        dataStorageManageMap.PopBlockAt(mouseWorldPosition);
    }
}