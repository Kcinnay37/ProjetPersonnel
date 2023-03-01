using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private float m_TimeCheckResourceInGround = 0.1f;
    [SerializeField] private float m_TimeCheckDistanceToPlayer = 0.2f;
    [SerializeField] private float m_DistanceToPlayerStopPhysique = 20f;
    [SerializeField] private float m_TimeToLive = 60f;

    Dictionary<GameObject, ResourceInWorld> m_InstanciateResource;
    Dictionary<GameObject, ResourceInWorld> m_UnInstanciateResource;

    private Coroutine m_CoroutineCheckResourceInGround = null;
    private Coroutine m_CoroutineCheckDistanceInstanciateResource = null;
    private Coroutine m_CoroutineCheckDistanceUnInstanciateResource = null;

    private Dictionary<ResourceInWorld, Coroutine> m_CoroutineTimeToLive;

    public static ResourceManager m_Instance;

    private void Awake()
    {
        m_Instance = this;

        m_InstanciateResource = new Dictionary<GameObject, ResourceInWorld>();
        m_UnInstanciateResource = new Dictionary<GameObject, ResourceInWorld>();
        m_CoroutineTimeToLive = new Dictionary<ResourceInWorld, Coroutine>();
    }

    public void Start()
    {
        m_CoroutineCheckResourceInGround = StartCoroutine(CheckResourceInGround());
        m_CoroutineCheckDistanceInstanciateResource = StartCoroutine(CoroutineCheckDistanceInstanciateResource());
        m_CoroutineCheckDistanceUnInstanciateResource = StartCoroutine(CoroutineCheckDistanceUnInstanciateResource());
    }

    private IEnumerator CheckResourceInGround()
    {
        Dictionary<EnumBlocks, EnumBlocks> backGroundBlockDictionary = Map.m_Instance.GetGrid().GetBackGroundDict();

        while (true)
        {
            yield return m_TimeCheckResourceInGround;

            EventManager.TriggerEvent("CheckResourceInGround", new Dictionary<string, object> { { "dictBlockBackGround", backGroundBlockDictionary } });
        }
    }

    IEnumerator CoroutineCheckDistanceInstanciateResource()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_TimeCheckDistanceToPlayer);
            Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();

            EventManager.TriggerEvent("CheckResourceDistanceToPlayer", new Dictionary<string, object> { { "playerPos", playerPos }, { "maxDist", m_DistanceToPlayerStopPhysique } });
        }
    }

    IEnumerator CoroutineCheckDistanceUnInstanciateResource()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_TimeCheckDistanceToPlayer);
            Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();

            List<ResourceInWorld> resourceToAdd = new List<ResourceInWorld>();

            foreach(KeyValuePair<GameObject, ResourceInWorld> resource in m_UnInstanciateResource)
            {
                float distance = Vector3.Distance(resource.Key.transform.position, playerPos);
                if (distance < m_DistanceToPlayerStopPhysique)
                {
                    resourceToAdd.Add(resource.Value);
                }
            }

            foreach(ResourceInWorld resource in resourceToAdd)
            {
                ActiveResource(resource);
            }
        }
    }

    IEnumerator CoroutineLive(ResourceInWorld resource)
    {
        yield return new WaitForSeconds(m_TimeToLive);
        m_CoroutineTimeToLive[resource] = null;
        RemoveResource(resource);
    }

    public void InstanciateResourceInWorldAt(object dataResource, Vector3 pos, Vector2 velo, int dir)
    {
        DataResource data = (DataResource)Pool.m_Instance.GetData(dataResource);

        GameObject objectResource = Pool.m_Instance.GetObject(data.GetInstanceType());
        objectResource.GetComponent<ResourceInWorld>().InitResource(false, velo, dataResource, data.GetInstanceType());
        objectResource.transform.rotation = Quaternion.identity;
        objectResource.transform.position = pos;

        Vector3 scale = objectResource.transform.localScale;
        if ((dir < 0 && scale.x > 0) || (dir > 0 && scale.x < 0))
        {
            scale.x = -scale.x;
        }
        objectResource.transform.localScale = scale;

        objectResource.SetActive(true);

        m_InstanciateResource.Add(objectResource, objectResource.GetComponent<ResourceInWorld>());
    }

    public void RemoveResource(ResourceInWorld resource)
    {
        if (m_InstanciateResource.ContainsKey(resource.gameObject))
        {
            m_InstanciateResource.Remove(resource.gameObject);
        }
        if(m_UnInstanciateResource.ContainsKey(resource.gameObject))
        {
            m_UnInstanciateResource.Remove(resource.gameObject);
        }
        if(m_CoroutineTimeToLive.ContainsKey(resource))
        {
            if(m_CoroutineTimeToLive[resource] != null)
            {
                StopCoroutine(m_CoroutineTimeToLive[resource]);
            }
            m_CoroutineTimeToLive.Remove(resource);
        }

        Pool.m_Instance.RemoveObject(resource.gameObject, resource.GetInstanceType());
    }

    public void UnActiveResource(ResourceInWorld resource)
    {
        m_InstanciateResource.Remove(resource.gameObject);
        m_UnInstanciateResource.Add(resource.gameObject, resource.GetComponent<ResourceInWorld>());

        m_CoroutineTimeToLive.Add(resource, StartCoroutine(CoroutineLive(resource)));

        resource.gameObject.SetActive(false);
    }

    public void ActiveResource(ResourceInWorld resource)
    {
        m_UnInstanciateResource.Remove(resource.gameObject);
        m_InstanciateResource.Add(resource.gameObject, resource.GetComponent<ResourceInWorld>());

        if (m_CoroutineTimeToLive.ContainsKey(resource))
        {
            if (m_CoroutineTimeToLive[resource] != null)
            {
                StopCoroutine(m_CoroutineTimeToLive[resource]);
            }
            m_CoroutineTimeToLive.Remove(resource);
        }

        resource.gameObject.SetActive(true);
    }
}
