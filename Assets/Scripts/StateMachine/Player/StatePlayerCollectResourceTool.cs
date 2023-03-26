using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerCollectResourceTool : StateRessource
{
    private DataTool m_DataTool;
    private GameObject m_Object;
    private Transform m_RayFirstPos;

    private Animator m_Animator;

    private StatePlayerControllerMovement m_StatePlayerControllerMovement;

    private Coroutine m_CoroutineAttack;

    public StatePlayerCollectResourceTool(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 firstPos = m_RayFirstPos.position;

        Vector2 dir = (mouseWorldPosition - firstPos).normalized;

        float radianAngle = m_DataTool.coneRadius * (Mathf.PI / 180);
        Vector2 dirCone = Vector2.right * m_StatePlayerControllerMovement.GetPlayerDir();

        float cosAngle = Vector2.Dot(dirCone, dir);
        float radAngle = Mathf.Acos(cosAngle);
        float degAngle = radAngle * Mathf.Rad2Deg;

        //pas necessaisaire 
        Vector2 dir1 = new Vector2(dirCone.x * Mathf.Cos(radianAngle) - dirCone.y * Mathf.Sin(radianAngle),
                        dirCone.x * Mathf.Sin(radianAngle) + dirCone.y * Mathf.Cos(radianAngle));

        Vector2 dir2 = new Vector2(dirCone.x * Mathf.Cos(-radianAngle) - dirCone.y * Mathf.Sin(-radianAngle),
                                dirCone.x * Mathf.Sin(-radianAngle) + dirCone.y * Mathf.Cos(-radianAngle));

        Debug.DrawRay(firstPos, dir * m_DataTool.distance);
        Debug.DrawRay(firstPos, dir1 * m_DataTool.distance);
        Debug.DrawRay(firstPos, dir2 * m_DataTool.distance);
        //--
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

        m_CoroutineAttack = null;
    }

    public override void End()
    {
        if (m_CoroutineAttack != null)
        {
            switch (m_DataTool.attackType)
            {
                case DataTool.AttackType.Swipe:
                    m_Animator.SetBool("AttackSwipe", false);
                    break;
                case DataTool.AttackType.Stab:
                    m_Animator.SetBool("AttackStab", false);
                    break;
            }

            m_StateMachine.StopCoroutine(m_CoroutineAttack);
            m_CoroutineAttack = null;
        }

        Pool.m_Instance.RemoveObject(m_Object, m_DataTool.instanceType);
        m_DataTool = null;
    }

    public override void ActionKeyDown()
    {
        if (m_CoroutineAttack != null)
        {
            return;
        }

        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 firstPos = m_RayFirstPos.position;

        Vector2 dir = (mouseWorldPosition - firstPos).normalized;

        float radianAngle = m_DataTool.coneRadius * (Mathf.PI / 180);
        Vector2 dirCone = Vector2.right * m_StatePlayerControllerMovement.GetPlayerDir();

        float cosAngle = Vector2.Dot(dirCone, dir);
        float radAngle = Mathf.Acos(cosAngle);
        float degAngle = radAngle * Mathf.Rad2Deg;

        if (degAngle > m_DataTool.coneRadius)
        {
            return;
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(firstPos, dir, m_DataTool.distance);

        m_CoroutineAttack = m_StateMachine.StartCoroutine(Attack(hits));
    }

    private IEnumerator Attack(RaycastHit2D[] hits)
    {
        m_Animator.SetFloat("AttackSpeed", 1 / m_DataTool.intervalAttack);

        switch (m_DataTool.attackType)
        {
            case DataTool.AttackType.Swipe:
                m_Animator.SetBool("AttackSwipe", true);
                break;
            case DataTool.AttackType.Stab:
                m_Animator.SetBool("AttackStab", true);
                break;
        }

        yield return new WaitForSeconds((1 * m_DataTool.intervalAttack) / 2);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Environement"))
            {
                break;
            }

            if (hit.transform.CompareTag("Collectible"))
            {
                Collectible collectible = hit.transform.GetComponent<Collectible>();
                if(collectible.GetToolsCanInteract().Contains(m_DataTool.dataType))
                {
                    collectible.TakeDamage(m_DataTool.damage);
                }

                break;
            }
        }

        yield return new WaitForSeconds((1 * m_DataTool.intervalAttack) / 2);


        switch (m_DataTool.attackType)
        {
            case DataTool.AttackType.Swipe:
                m_Animator.SetBool("AttackSwipe", false);
                break;
            case DataTool.AttackType.Stab:
                m_Animator.SetBool("AttackStab", false);
                break;
        }

        m_CoroutineAttack = null;
    }
}
