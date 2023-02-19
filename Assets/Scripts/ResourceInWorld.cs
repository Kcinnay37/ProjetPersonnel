using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInWorld : MonoBehaviour
{
    [SerializeField] private float m_TimeCanCollect;
    [SerializeField] private float m_TimeToLive;

    private bool m_Equip;

    private bool m_CanCollect;
    private Coroutine m_CoroutineCanCollect;
    private Coroutine m_CoroutineLive;

    private Vector2 m_InitialVelocityForce;

    private object m_InstanceType;
    private object m_DataType;

    public void InitResource(bool equip, Vector2 velocity, object dataType, object instanceType)
    {
        m_Equip = equip;
        m_InitialVelocityForce = velocity;

        m_InstanceType = instanceType;
        m_DataType = dataType;
    }

    private void OnEnable()
    {
        if (!m_Equip)
        {
            GetComponent<BoxCollider2D>().isTrigger = false;

            GetComponent<Rigidbody2D>().AddForce(m_InitialVelocityForce);
            m_CoroutineCanCollect = StartCoroutine(CoroutineSpawn());
            m_CoroutineLive = StartCoroutine(CoroutineLive());

        }
        else
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        if(m_DataType != null)
        {
            DataResource dataResource = (DataResource)Pool.m_Instance.GetData(m_DataType);
            GetComponent<SpriteRenderer>().sprite = dataResource.image;
        }
        
        m_CanCollect = false;
    }

    private void OnDisable()
    {
        m_CanCollect = false;
        m_InitialVelocityForce = Vector2.zero;

        if(m_CoroutineCanCollect != null)
        {
            StopCoroutine(m_CoroutineCanCollect);
        }
        if (m_CoroutineLive != null)
        {
            StopCoroutine(m_CoroutineLive);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player") && m_CanCollect)
        {
            DataStorageManagePlayer dataStorageManagePlayer = (DataStorageManagePlayer)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.managePlayer);
            if(dataStorageManagePlayer.CollectResource(m_DataType))
            {
                Pool.m_Instance?.RemoveObject(gameObject, m_InstanceType);
            }
        }
    }

    IEnumerator CoroutineSpawn()
    {
        yield return new WaitForSeconds(m_TimeCanCollect);
        m_CanCollect = true;
    }

    IEnumerator CoroutineLive()
    {
        yield return new WaitForSeconds(m_TimeToLive);
        Pool.m_Instance?.RemoveObject(gameObject, m_InstanceType);
    }
}
