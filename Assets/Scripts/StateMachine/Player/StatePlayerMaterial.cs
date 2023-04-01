using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerMaterial : StateRessource
{
    private DataMaterial m_DataMaterial;
    private GameObject m_Object;

    public StatePlayerMaterial(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        InventoryCase caseEquip = dataStoragePlayerEquip.GetEquipCase();
        
        m_DataMaterial = (DataMaterial)Pool.m_Instance.GetData(caseEquip.resource);

        m_Object = Pool.m_Instance.GetObject(m_DataMaterial.instanceType);
        m_Object.GetComponent<ResourceInWorld>().InitResource(true, Vector2.zero, caseEquip.resource, m_DataMaterial.instanceType);

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
        Pool.m_Instance.RemoveObject(m_Object, m_DataMaterial.instanceType);
        m_DataMaterial = null;
    }
}
