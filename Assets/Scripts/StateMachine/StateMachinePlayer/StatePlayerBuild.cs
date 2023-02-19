using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerBuild : StateRessource
{
    private DataBlock m_DataBlock;
    private EnumBlocks m_BlockType;
    private GameObject m_Object;

    public StatePlayerBuild(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        InventoryCase caseEquip = dataStoragePlayerEquip.GetEquipCase();
        m_DataBlock = (DataBlock)Pool.m_Instance.GetData(caseEquip.resource);
        m_BlockType = (EnumBlocks)caseEquip.resource;

        //Instanci l'outil
        m_Object = Pool.m_Instance.GetObject(m_DataBlock.instanceType);
        m_Object.GetComponent<ResourceInWorld>().InitResource(true, Vector2.zero, caseEquip.resource, EnumBlocks.block);
        

        Vector3 scale = m_Object.transform.localScale;
        
        if((m_StateMachine.transform.localScale.x < 0 && scale.x > 0) || (m_StateMachine.transform.localScale.x > 0 && scale.x < 0))
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
        m_Object.GetComponent<SpriteRenderer>().sprite = null;
        Pool.m_Instance.RemoveObject(m_Object, m_DataBlock.instanceType);
        m_DataBlock = null;
    }

    public override void ActionKeyDown()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

        DataStorageManageMap dataStorageManageMap = (DataStorageManageMap)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageMap);

        dataStorageManageMap.AddBlockAt(mouseWorldPosition, m_BlockType);
    }

    public override void ActionOldKey()
    {

    }
}
