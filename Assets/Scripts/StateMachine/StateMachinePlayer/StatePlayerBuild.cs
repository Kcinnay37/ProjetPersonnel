using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerBuild : StateRessource
{
    private DataPlayer m_GlobalDataPlayer;

    private DataBlock m_DataBlock;
    private EnumBlocks m_BlockType;
    private GameObject m_Object;

    private Transform m_RaycastPoint;

    public StatePlayerBuild(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_RaycastPoint = GameObject.Find("PlayerRaycastPoint").transform;

        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

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

    }

    public override void ActionOldKey()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 firstPos = m_RaycastPoint.position;

        Vector2 dir = (mouseWorldPosition - firstPos).normalized;

        //regarde si la souris est trop loin
        if (Vector2.Distance(m_RaycastPoint.position, mouseWorldPosition) > m_GlobalDataPlayer.blockDropDistance)
        {
            return;
        }

        Vector3Int cornerUpLeft = Map.m_Instance.GetGrid().ConvertWorldToCell(new Vector3(firstPos.x - 0.4f, firstPos.y + 0.9f, 0.0f));
        Vector3Int cornerUpRight = Map.m_Instance.GetGrid().ConvertWorldToCell(new Vector3(firstPos.x + 0.4f, firstPos.y + 0.9f, 0.0f));
        Vector3Int cornerBotLeft = Map.m_Instance.GetGrid().ConvertWorldToCell(new Vector3(firstPos.x - 0.4f, firstPos.y - 0.9f, 0.0f));
        Vector3Int cornerBotRight = Map.m_Instance.GetGrid().ConvertWorldToCell(new Vector3(firstPos.x + 0.4f, firstPos.y - 0.9f, 0.0f));

        Vector3Int cellMousePos = Map.m_Instance.GetGrid().ConvertWorldToCell(new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0.0f));

        if(cellMousePos == cornerUpLeft || cellMousePos == cornerUpRight || cellMousePos == cornerBotLeft || cellMousePos == cornerBotRight)
        {
            return;
        }


        float dist = Vector2.Distance(firstPos, mouseWorldPosition);

        Debug.DrawRay(firstPos, dir * dist);

        //regarde si il a de l'environement dans les jambe
        RaycastHit2D[] hits = Physics2D.RaycastAll(firstPos, dir, dist);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Environement"))
            {
                return;
            }
        }

        if (Map.m_Instance.GetGrid().AddBlockAt(mouseWorldPosition, m_BlockType))
        {
            DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
            dataStoragePlayerEquip.DropOneAtCaseEquip();
        }
    }
}
