using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerExcavationTool : StateRessource
{
    private DataTool m_DataTool;
    private GameObject m_Object;

    private Vector3Int m_CurrCellPoint;
    private Coroutine m_CoroutineDestroyBlock;
    private Vector3 m_HitPoint;

    private bool m_DestoyBlock;

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
        m_Object.GetComponent<ResourceInWorld>().InitResource(true, Vector2.zero, caseEquip.resource, m_DataTool.instanceType);

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

        m_CurrCellPoint = Vector3Int.zero;
        m_HitPoint = Vector3.zero;
        m_CoroutineDestroyBlock = null;
        m_DestoyBlock = false;
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
        
    }

    public override void ActionOldKey()
    {
        if (m_DestoyBlock)
        {
            DataStorageManageMap dataStorageManageMap = (DataStorageManageMap)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageMap);
            dataStorageManageMap.PopBlockAt(m_HitPoint);

            m_CurrCellPoint = Vector3Int.zero;
            m_HitPoint = Vector3.zero;
            m_DestoyBlock = false;
        }

        Vector3 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 parentPos = m_Object.GetComponentInParent<Transform>().position;

        Vector2 dir = (mouseWorldPosition - parentPos).normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(parentPos, dir, m_DataTool.distance);

        Debug.DrawRay(parentPos, dir * m_DataTool.distance);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform.CompareTag("Environement"))
            {
                DataStorageManageMap dataStorageManageMap = (DataStorageManageMap)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.manageMap);

                Vector3Int cellPoint = dataStorageManageMap.GetWorldToCell(hit.point + (new Vector2(0.01f, 0.01f) * dir));
                if(!cellPoint.Equals(m_CurrCellPoint))
                {
                    if (m_CoroutineDestroyBlock != null)
                    {
                        m_StateMachine.StopCoroutine(m_CoroutineDestroyBlock);
                    }
                    m_CurrCellPoint = cellPoint;
                    m_CoroutineDestroyBlock = m_StateMachine.StartCoroutine(CoroutineDestroyBlock());
                }
                m_HitPoint = hit.point + (new Vector2(0.01f, 0.01f) * dir);
                return;
            }
        }

        if (m_CoroutineDestroyBlock != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineDestroyBlock);
            m_CurrCellPoint = Vector3Int.zero;
            m_HitPoint = Vector3.zero;
            m_DestoyBlock = false;
        }
    }

    public override void ActionKeyUp()
    {
        if (m_CoroutineDestroyBlock != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineDestroyBlock);
        }

        m_CurrCellPoint = Vector3Int.zero;
        m_HitPoint = Vector3.zero;
        m_DestoyBlock = false;
    }

    private IEnumerator CoroutineDestroyBlock()
    {
        yield return new WaitForSeconds(m_DataTool.intervalAttack);
        m_DestoyBlock = true;
    }
}