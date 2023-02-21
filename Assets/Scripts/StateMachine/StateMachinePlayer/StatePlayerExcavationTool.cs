using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerExcavationTool : StateRessource
{
    private DataTool m_DataTool;
    private GameObject m_Object;
    private Transform m_RayFirstPos;

    private Vector3Int m_CurrCellPoint;
    private Coroutine m_CoroutineDestroyBlock;
    private Vector3 m_HitPoint;

    private bool m_DestoyBlock;

    private Animator m_Animator;

    StatePlayerControllerMovement m_StatePlayerControllerMovement;

    public StatePlayerExcavationTool(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_Animator = m_StateMachine.GetComponent<Animator>();

        m_RayFirstPos = GameObject.Find("PlayerRaycastPoint").transform;

        m_StatePlayerControllerMovement = (StatePlayerControllerMovement)m_StateMachine.GetState(EnumStatesPlayer.controllerMovement);

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
        m_Animator.SetTrigger("Attack");
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
            Map.m_Instance.GetGrid().PopBlockAt(m_HitPoint);

            ResetValue();
        }

        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 firstPos = m_RayFirstPos.position;

        Vector2 dir = (mouseWorldPosition - firstPos).normalized;


        Debug.DrawRay(firstPos, dir * m_DataTool.distance);

        if(dir.x * m_StatePlayerControllerMovement.GetPlayerDir() < 0)
        {
            if (m_CoroutineDestroyBlock != null)
            {
                m_StateMachine.StopCoroutine(m_CoroutineDestroyBlock);
            }
            ResetValue();
            return;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(firstPos, dir, m_DataTool.distance);

        

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform.CompareTag("Environement"))
            {
                Vector3Int cellPoint = Map.m_Instance.GetGrid().ConvertWorldToCell(hit.point + (new Vector2(0.01f, 0.01f) * dir));
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
            ResetValue();
        }
    }

    public override void ActionKeyUp()
    {
        if (m_CoroutineDestroyBlock != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineDestroyBlock);
        }

        ResetValue();
    }

    private void ResetValue()
    {
        m_CurrCellPoint = Vector3Int.zero;
        m_HitPoint = Vector3.zero;
        m_DestoyBlock = false;
        UI.m_Instance.GetUIWorld().EndBlockUI();

        m_Animator.SetBool("Attack", false);
    }

    private IEnumerator CoroutineDestroyBlock()
    {
        DataBlock block = Map.m_Instance.GetGrid().GetBlockAt(m_CurrCellPoint.x, m_CurrCellPoint.y);
        int initialHealth = block.health;

        UI.m_Instance.GetUIWorld().InitBlockUI(m_CurrCellPoint);
        int currHealth = initialHealth;

        m_Animator.SetFloat("AttackSpeed", 1 / m_DataTool.intervalAttack);
        m_Animator.SetBool("Attack", true);

        while (currHealth > 0)
        {
            
            yield return new WaitForSeconds(m_DataTool.intervalAttack);
            currHealth -= m_DataTool.damage;
            UI.m_Instance.GetUIWorld().SetSlider(currHealth, initialHealth);
        }
        
        m_DestoyBlock = true;
    }
}