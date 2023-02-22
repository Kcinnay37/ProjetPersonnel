using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInWorld : MonoBehaviour
{
    private const float m_TimeCanCollect = 0.5f;
    private const float m_TimeToLive = 60f;
    private const float m_TimeCheckDistance = 0.2f;
    private const float m_DistanceToDelete = 20f;

    private bool m_Equip;

    private Coroutine m_CoroutineCanCollect;
    private Coroutine m_CoroutineLive;
    private Coroutine m_CoroutineCheckDistance;

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
            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            if(boxCollider2D != null)
            {
                boxCollider2D.isTrigger = false;
            }

            PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();
            if(polygonCollider2D != null)
            {
                polygonCollider2D.isTrigger = false;
            }

            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            GetComponent<Rigidbody2D>().AddForce(m_InitialVelocityForce);
            m_CoroutineCanCollect = StartCoroutine(CoroutineSpawn());
            m_CoroutineLive = StartCoroutine(CoroutineLive());
            m_CoroutineCheckDistance = StartCoroutine(CoroutineCheckDistance());

            EventManager.StartListening("CheckResourceInGround", CheckResourceInGround);

        }
        else
        {
            BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                boxCollider2D.isTrigger = true;
            }

            PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();
            if (polygonCollider2D != null)
            {
                polygonCollider2D.isTrigger = true;
            }

            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        if(m_DataType != null)
        {
            DataResource dataResource = (DataResource)Pool.m_Instance.GetData(m_DataType);
            GetComponent<SpriteRenderer>().sprite = dataResource.image;
        }
        gameObject.layer = LayerMask.NameToLayer("ResourceCantTake");
    }

    private void OnDisable()
    {
        gameObject.layer = LayerMask.NameToLayer("ResourceCantTake");
        m_InitialVelocityForce = Vector2.zero;

        EventManager.StopListening("CheckResourceInGround", CheckResourceInGround);

        if (m_CoroutineCanCollect != null)
        {
            StopCoroutine(m_CoroutineCanCollect);
        }
        if (m_CoroutineLive != null)
        {
            StopCoroutine(m_CoroutineLive);
        }
        if (m_CoroutineCheckDistance != null)
        {
            StopCoroutine(m_CoroutineLive);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            if(GameManager.m_Instance.CurrPlayerCollectResource(m_DataType))
            {
                Pool.m_Instance?.RemoveObject(gameObject, m_InstanceType);
            }
        }
    }

    IEnumerator CoroutineSpawn()
    {
        yield return new WaitForSeconds(m_TimeCanCollect);
        gameObject.layer = LayerMask.NameToLayer("ResourceCanTake");
    }

    IEnumerator CoroutineLive()
    {
        yield return new WaitForSeconds(m_TimeToLive);
        Pool.m_Instance?.RemoveObject(gameObject, m_InstanceType);
    }

    IEnumerator CoroutineCheckDistance()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeCheckDistance);
            Vector3 playerPos = GameManager.m_Instance.GetCurrPlayerPos();
            float distance = Vector3.Distance(transform.position, playerPos);
            if (distance >= m_DistanceToDelete)
            {
                Pool.m_Instance?.RemoveObject(gameObject, m_InstanceType);
            }
        }
    }

    object CheckResourceInGround(Dictionary<string, object> parametres)
    {
        Dictionary<EnumBlocks, EnumBlocks> backGroundBlockDictionary = (Dictionary<EnumBlocks, EnumBlocks>)parametres["dictBlockBackGround"];

        Vector2Int currPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(transform.position);
        EnumBlocks currBlock = Map.m_Instance.GetGrid().GetGrid()[currPos.x, currPos.y];

        if(!backGroundBlockDictionary.ContainsKey(currBlock))
        {
            Pool.m_Instance.RemoveObject(gameObject, m_InstanceType);
        }

        return null;
    }
}
