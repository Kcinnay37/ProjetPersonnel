using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerControllerMount : StateRessource
{
    private bool m_IsEquip;
    private DataMount m_DataMount;
    private GameObject m_Object;
    InventoryCase caseEquip;

    Vector3 scale;

    Animator m_Animator;

    Rigidbody2D m_RigidBody;

    DataPlayer m_GlobalDataPlayer;

    public StatePlayerControllerMount(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_IsEquip = false;

        m_RigidBody = m_StateMachine.GetComponent<Rigidbody2D>();

        m_Animator = m_StateMachine.GetComponent<Animator>();

        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        InventoryCase caseEquip = dataStoragePlayerEquip.GetEquipCase();

        m_DataMount = (DataMount)Pool.m_Instance.GetData(caseEquip.resource);

        m_Object = Pool.m_Instance.GetObject(m_DataMount.instanceType);
        m_Object.GetComponent<ResourceInWorld>().InitResource(true, Vector2.zero, caseEquip.resource, m_DataMount.instanceType);

        scale = m_Object.transform.localScale;
        if ((m_StateMachine.transform.localScale.x < 0 && scale.x > 0) || (m_StateMachine.transform.localScale.x > 0 && scale.x < 0))
        {
            scale.x = -scale.x;
        }
        m_Object.transform.localScale = scale;

        EquipOnHand();
    }

    public override void End()
    {
        Pool.m_Instance.RemoveObject(m_Object, m_DataMount.instanceType);
        m_DataMount = null;

        m_Animator.SetBool("UseMount", false);

        if(m_IsEquip)
        {
            m_RigidBody.gravityScale = m_GlobalDataPlayer.gravityScale;
            m_StateMachine.AddCurrState(EnumStatesPlayer.controllerMovement);
        }
    }

    public override void Update()
    {
        if(m_IsEquip)
        {
            UpdateMove();
        }

    }

    private void UpdateMove()
    {
        Vector2 velo = m_RigidBody.velocity;
        velo.y = 0;

        bool checkHorizontal = true;
        bool checkVertical = true;

        //mets la velo a 0 si les deux touche opposé sont enfoncé
        if (Input.GetKey(m_GlobalDataPlayer.leftKey) && Input.GetKey(m_GlobalDataPlayer.rightKey))
        {
            velo.x = 0;
            checkHorizontal = false;
        }
        if (Input.GetKey(m_GlobalDataPlayer.upKey) && Input.GetKey(m_GlobalDataPlayer.downKey))
        {
            velo.y = 0;
            checkVertical = false;
        }

        if (checkHorizontal)
        {
            velo.x = Input.GetAxis("Horizontal") * m_DataMount.mountSpeed;
        }
        if (checkVertical)
        {
            velo.y = Input.GetAxis("Vertical") * m_DataMount.mountSpeed;
        }

        m_RigidBody.velocity = velo;

        //rotationne le player dans la bonne direction
        if (m_RigidBody.velocity.x < 0)
        {
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = -1;
            m_StateMachine.transform.localScale = rota;
        }
        else if (m_RigidBody.velocity.x > 0)
        {
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = 1;
            m_StateMachine.transform.localScale = rota;
        }
    }

    public override void ActionKeyDown()
    {
        if(m_IsEquip)
        {
            m_IsEquip = false;

            m_StateMachine.AddCurrState(EnumStatesPlayer.controllerMovement);

            EquipOnHand();
        }
        else
        {
            m_IsEquip = true;

            m_StateMachine.PopCurrState(EnumStatesPlayer.controllerMovement);

            EquipOnLeg();
        }
    }

    private void EquipOnHand()
    {
        m_Object.transform.parent = GameObject.Find("PlayerWeaponSlot").transform;
        m_Object.SetActive(true);

        m_Object.transform.localPosition = Vector3.zero;

        m_Object.transform.localRotation = Quaternion.identity;

        m_Animator.SetBool("UseMount", false);

        m_RigidBody.gravityScale = m_GlobalDataPlayer.gravityScale;
    }

    private void EquipOnLeg()
    {
        m_Object.transform.parent = GameObject.Find("PlayerMountSlot").transform;
        m_Object.SetActive(true);

        m_Object.transform.localPosition = Vector3.zero;

        m_Object.transform.localRotation = Quaternion.identity;

        m_Animator.SetBool("UseMount", true);

        m_RigidBody.gravityScale = 0;
    }
}
