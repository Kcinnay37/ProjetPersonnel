using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnumPlayers m_Player;
    [SerializeField] private float m_TimeCheckResourceInWorld;

    private GameObject m_CurrPlayer;

    private Coroutine m_CoroutineCheckResource = null;

    public static GameManager m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        Map.m_Instance.GenerateMap();

        m_CurrPlayer = Pool.m_Instance.GetObject(m_Player);
        m_CurrPlayer.SetActive(true);

        m_CoroutineCheckResource = StartCoroutine(CheckResourceInGround());
    }

    public Vector3 GetCurrPlayerPos()
    {
        if (m_CurrPlayer == null) return Vector3.zero;
        return m_CurrPlayer.transform.position;
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
    }

    public bool CurrPlayerCollectResource(object resource)
    {
        StateMachinePlayer stateMachinePlayer = m_CurrPlayer.GetComponent<StateMachinePlayer>();
        StatePlayerControllerInventory statePlayerControllerInventory = (StatePlayerControllerInventory)stateMachinePlayer.GetState(EnumStatesPlayer.controllerInventory);
        return statePlayerControllerInventory.CollectResource(resource);
    }

    private IEnumerator CheckResourceInGround()
    {
        Dictionary<EnumBlocks, EnumBlocks> m_BackGroundBlockDictionary = new Dictionary<EnumBlocks, EnumBlocks>();
        DataMap dataMap = Map.m_Instance.GetData();

        foreach(DataMap.Biome biome in dataMap.mapBiomes)
        {
            DataBiome dataBiome = (DataBiome)Pool.m_Instance.GetData(biome.dataBiome);

            if(!m_BackGroundBlockDictionary.ContainsKey(dataBiome.biomeBlocks[dataBiome.biomeBlocks.Count - 1].block))
            {
                m_BackGroundBlockDictionary.Add(dataBiome.biomeBlocks[dataBiome.biomeBlocks.Count - 1].block, dataBiome.biomeBlocks[dataBiome.biomeBlocks.Count - 1].block);
            }
        }

        while(true)
        {
            yield return m_TimeCheckResourceInWorld;

            EventManager.TriggerEvent("CheckResourceInGround", new Dictionary<string, object> { { "dictBlockBackGround", m_BackGroundBlockDictionary} });
        }
    }
}
