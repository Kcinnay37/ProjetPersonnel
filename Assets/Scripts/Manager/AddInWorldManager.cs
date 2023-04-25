using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddInWorldManager : MonoBehaviour
{
    [SerializeField] private float m_TimeToStart;
    [SerializeField] private float m_DelayCheckSpawn;
    [SerializeField] private float m_DelayCheckDispawn;
    [SerializeField] private float m_DelayCheckBelow;
    [SerializeField] private float m_DistanceSpawn;
    [SerializeField] private float m_DistanceDispawn;

    private Coroutine m_CoroutineCheckSpawn;
    private Coroutine m_CoroutineCheckDispawn;
    private Coroutine m_CoroutineCheckBelow;

    private Dictionary<Vector2Int, GameObject> m_ActiveCollectiblesPosKey;
    private Dictionary<GameObject, Vector2Int> m_ActiveCollectiblesObjectKey;

    public static AddInWorldManager m_Instance;


    private void Awake()
    {
        m_Instance = this;

        m_ActiveCollectiblesPosKey = new Dictionary<Vector2Int, GameObject>();
        m_ActiveCollectiblesObjectKey = new Dictionary<GameObject, Vector2Int>();
    }

    private void Start()
    {
        StartCoroutine(StartSpawnCollectible());
    }

    private IEnumerator StartSpawnCollectible()
    {
        yield return new WaitForSeconds(m_TimeToStart);

        m_CoroutineCheckSpawn = StartCoroutine(CheckSpawn());
        m_CoroutineCheckDispawn = StartCoroutine(CheckDispawn());
        m_CoroutineCheckBelow = StartCoroutine(CheckBelow());
    }

    private IEnumerator CheckSpawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeToStart);

            Dictionary<Vector2Int, EnumAddInWorld> dictCollectibles = Map.m_Instance.GetGrid().GetCollectible();
            Vector2Int point = Map.m_Instance.GetGrid().GetPoint();

            for(int x = point.x - (int)m_DistanceSpawn; x <= point.x + (int)m_DistanceSpawn; x++)
            {
                for (int y = point.y - (int)m_DistanceSpawn; y <= point.y + (int)m_DistanceSpawn; y++)
                {
                    Vector2Int currPos = new Vector2Int(x, y);
                    if(dictCollectibles.ContainsKey(currPos))
                    {
                        SpawnCollectible(currPos);
                    }
                }
            }
        }
    }

    private IEnumerator CheckDispawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeToStart);

            Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();

            List<Vector2Int> listToDisable = new List<Vector2Int>();

            foreach(KeyValuePair<Vector2Int, GameObject> value in m_ActiveCollectiblesPosKey)
            {
                float distance = Vector2.Distance(value.Key, playerPos);
                if(distance > m_DistanceDispawn)
                {
                    listToDisable.Add(value.Key);
                }
            }

            foreach(Vector2Int pos in listToDisable)
            {
                DisableCollectible(pos);
            }
        }
    }

    private IEnumerator CheckBelow()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_DelayCheckBelow);

            EnumBlocks[,] grid = Map.m_Instance.GetGrid().GetGrid();
            Dictionary<EnumBlocks, EnumBlocks> dictBackGround = Map.m_Instance.GetGrid().GetBackGroundDict();

            List<GameObject> listToDisable = new List<GameObject>();

            foreach (KeyValuePair<Vector2Int, GameObject> value in m_ActiveCollectiblesPosKey)
            {
                DataAddInWorld dataValue = (DataAddInWorld)Pool.m_Instance.GetData(Map.m_Instance.GetGrid().GetCollectible()[value.Key]);

                for (int x = 0; x <= dataValue.gridSpawnSideAndHeightSize.x; x++)
                {
                    Vector2Int belowPos1 = new Vector2Int(value.Key.x + x, value.Key.y - 1);
                    Vector2Int belowPos2 = new Vector2Int(value.Key.x - x, value.Key.y - 1);

                    if ((dictBackGround.ContainsKey(grid[belowPos1.x, belowPos1.y]) && dataValue.gridSpawn[dataValue.gridSpawnSideAndHeightSize.x + x, dataValue.gridSpawnSideAndHeightSize.y])
                        || (dictBackGround.ContainsKey(grid[belowPos2.x, belowPos2.y]) && dataValue.gridSpawn[dataValue.gridSpawnSideAndHeightSize.x - x, dataValue.gridSpawnSideAndHeightSize.y]))
                    {
                        listToDisable.Add(value.Value);
                    }
                }
            }

            foreach (GameObject collectible in listToDisable)
            {
                DestroyCollectible(collectible);
            }
        }
    }

    private void SpawnCollectible(Vector2Int pos)
    {
        if(m_ActiveCollectiblesPosKey.ContainsKey(pos))
        {
            return;
        }

        Dictionary<Vector2Int, EnumAddInWorld> dictCollectibles = Map.m_Instance.GetGrid().GetCollectible();

        GameObject collectible = Pool.m_Instance.GetObject(dictCollectibles[pos]);
        DataAddInWorld dataCollectible = (DataAddInWorld)Pool.m_Instance.GetData(dictCollectibles[pos]);

        m_ActiveCollectiblesPosKey.Add(pos, collectible);
        m_ActiveCollectiblesObjectKey.Add(collectible, pos);

        collectible.transform.position = new Vector3((float)pos.x + dataCollectible.offsetSpawn.x, (float)pos.y + dataCollectible.offsetSpawn.y, -1f);
        collectible.SetActive(true);
    }

    private void DisableCollectible(Vector2Int pos)
    {
        if (!m_ActiveCollectiblesPosKey.ContainsKey(pos))
        {
            return;
        }

        Dictionary<Vector2Int, EnumAddInWorld> dictCollectibles = Map.m_Instance.GetGrid().GetCollectible();

        GameObject collectible = m_ActiveCollectiblesPosKey[pos];
        m_ActiveCollectiblesPosKey.Remove(pos);
        m_ActiveCollectiblesObjectKey.Remove(collectible);

        Pool.m_Instance.RemoveObject(collectible, dictCollectibles[pos]);
    }

    public void DestroyCollectible(GameObject collectible)
    {
        if(!m_ActiveCollectiblesObjectKey.ContainsKey(collectible))
        {
            return;
        }

        Dictionary<Vector2Int, EnumAddInWorld> dictCollectibles = Map.m_Instance.GetGrid().GetCollectible();

        Vector2Int pos = m_ActiveCollectiblesObjectKey[collectible];
        m_ActiveCollectiblesPosKey.Remove(pos);
        m_ActiveCollectiblesObjectKey.Remove(collectible);

        Pool.m_Instance.RemoveObject(collectible, dictCollectibles[pos]);

        Map.m_Instance.GetGrid().GetCollectible().Remove(pos);
    }

    public Dictionary<Vector2Int, GameObject> GetAllResourceEnable()
    {
        return m_ActiveCollectiblesPosKey;
    }
}
