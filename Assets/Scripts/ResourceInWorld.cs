using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInWorld : MonoBehaviour
{
    private DataResourceInWorld m_Data;

    private bool m_Equip;

    private Coroutine m_CoroutineCanCollect;

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
        m_Data = (DataResourceInWorld)Pool.m_Instance.GetData(EnumSpecialResources.resourceInWorld);

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

            EventManager.StartListening("CheckResourceInGround", CheckResourceInGround);
            EventManager.StartListening("CheckResourceDistanceToPlayer", CheckDistanceToPlayer);

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
            UpdateCollider();
        }
        gameObject.layer = LayerMask.NameToLayer("ResourceCantTake");
    }

    private void OnDisable()
    {
        gameObject.layer = LayerMask.NameToLayer("ResourceCantTake");
        m_InitialVelocityForce = Vector2.zero;

        EventManager.StopListening("CheckResourceInGround", CheckResourceInGround);
        EventManager.StopListening("CheckResourceDistanceToPlayer", CheckDistanceToPlayer);

        if (m_CoroutineCanCollect != null)
        {
            StopCoroutine(m_CoroutineCanCollect);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            if(PlayerManager.m_Instance.CurrPlayerCollectResource(m_DataType))
            {
                ResourceManager.m_Instance.RemoveResource(this);
            }
        }
    }

    private void UpdateCollider()
    {
        DataResource dataResource = (DataResource)Pool.m_Instance.GetData(m_DataType);

        Vector2[] pointsDuContour = new Vector2[dataResource.image.vertices.Length];
        for (int i = 0; i < dataResource.image.vertices.Length; i++)
        {
            pointsDuContour[i] = dataResource.image.vertices[i];
        }
        PolygonCollider2D polygoneCollider = GetComponent<PolygonCollider2D>();

        if(polygoneCollider != null)
        {
            polygoneCollider.SetPath(0, pointsDuContour);
        }
    }

    IEnumerator CoroutineSpawn()
    {
        yield return new WaitForSeconds(m_Data.timeCanCollect);
        gameObject.layer = LayerMask.NameToLayer("ResourceCanTake");
    }

    object CheckResourceInGround(Dictionary<string, object> parametres)
    {
        Dictionary<EnumBlocks, EnumBlocks> backGroundBlockDictionary = (Dictionary<EnumBlocks, EnumBlocks>)parametres["dictBlockBackGround"];

        Vector2Int currPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(transform.position);
        EnumBlocks currBlock = Map.m_Instance.GetGrid().GetGrid()[currPos.x, currPos.y];

        if(!backGroundBlockDictionary.ContainsKey(currBlock))
        {
            ResourceManager.m_Instance.RemoveResource(this);
        }

        return null;
    }

    object CheckDistanceToPlayer(Dictionary<string, object> parametres)
    {
        Vector3 playerPos = (Vector3)parametres["playerPos"];
        float maxDist = (float)parametres["maxDist"];

        float distance = Vector3.Distance(transform.position, playerPos);
        if (distance >= maxDist)
        {
            ResourceManager.m_Instance.UnActiveResource(this);
        }

        return null;
    }

    public object GetInstanceType()
    {
        return m_InstanceType;
    }
}
