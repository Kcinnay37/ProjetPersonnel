using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    [System.Serializable]
    public struct MonsterSpawn
    {
        public EnumMonster monster;
        public TypeSpawn typeSpawn;
        public int strength;
        [Range(1, 1000)]
        public int rarity;
    }

    [System.Serializable]
    public struct BiomeDifficulties
    {
        public EnumBiomes biome;
        public int maxStrength;
        public int offsetStrength;
        public List<MonsterSpawn> monsterSpawn;
    }

    public struct BiomeDictValue
    {
        public int maxStrength;
        public int offsetStrength;
        public List<MonsterSpawn> monsterSpawn;
    }

    public enum TypeSpawn
    {
        spawnGround,
        spawnAir,
        bigSpawnGround,
        bigSpawnAir
    }

    [SerializeField] List<BiomeDifficulties> biomeDifficulties = new List<BiomeDifficulties>();

    [SerializeField] private int m_DistanceSpawn;
    [SerializeField] private int m_OffsetViewPlayer;
    [SerializeField] private float m_TimeCheckForSpawn;

    [SerializeField] private float m_TimeToCheckDistanceToPlayer;
    [SerializeField] private float m_DistanceToPlayerStopPhysique = 20f;
    [SerializeField] private float m_TimeToLiveDisabled = 120;

    [SerializeField] private float m_TimeWaitToStart;

    Dictionary<GameObject, MonsterSpawn> m_EnableMonster;
    Dictionary<GameObject, MonsterSpawn> m_DisableMonster;

    private Dictionary<EnumBiomes, BiomeDictValue> m_DictBiome;

    private Dictionary<GameObject, Coroutine> m_CoroutineTimeToLive;

    private int m_CurrStrength;

    private Coroutine m_CoroutineCheckSpawn;
    private Coroutine m_CoroutineCheckDistanceToEnable;
    private Coroutine m_CoroutineCheckDistanceToDisable;

    public static MonsterManager m_Instance;

    private void Awake()
    {
        m_Instance = this;

        m_EnableMonster = new Dictionary<GameObject, MonsterSpawn>();
        m_DisableMonster = new Dictionary<GameObject, MonsterSpawn>();

        m_CoroutineTimeToLive = new Dictionary<GameObject, Coroutine>();

        m_DictBiome = new Dictionary<EnumBiomes, BiomeDictValue>();
        foreach(BiomeDifficulties currBiome in biomeDifficulties)
        {
            EnumBiomes biome = currBiome.biome;
            BiomeDictValue biomeDictValue = new BiomeDictValue();
            biomeDictValue.maxStrength = currBiome.maxStrength;
            biomeDictValue.monsterSpawn = currBiome.monsterSpawn;
            biomeDictValue.offsetStrength = currBiome.offsetStrength;

            m_DictBiome.Add(biome, biomeDictValue);
        }

        m_CurrStrength = 0;
        
    }

    public void Start()
    {
        StartCoroutine(WaitToStart());
    }

    private IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(m_TimeWaitToStart);
        m_CoroutineCheckSpawn = StartCoroutine(SpawnEnemie());
        m_CoroutineCheckDistanceToEnable = StartCoroutine(CoroutineCheckDistanceToEnable());
        m_CoroutineCheckDistanceToDisable = StartCoroutine(CoroutineCheckDistanceToDisable());
    }

    private IEnumerator SpawnEnemie()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeCheckForSpawn);

            EnumBiomes currBiomes = Map.m_Instance.GetGrid().GetBiomeAtCurrPoint();

            if(!m_DictBiome.ContainsKey(currBiomes))
            {
                continue;
            }    

            if(m_CurrStrength >= m_DictBiome[currBiomes].maxStrength)
            {
                continue;
            }

            foreach(MonsterSpawn monsterSpawn in m_DictBiome[currBiomes].monsterSpawn)
            {
                int rarity = Random.Range(0, 1001);
                if(rarity <= monsterSpawn.rarity)
                {
                    SpawnMonster(monsterSpawn);
                    break;
                }
            }
        }
    }

    private IEnumerator CoroutineCheckDistanceToEnable()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeToCheckDistanceToPlayer);

            Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();

            List<GameObject> monsterToAdd = new List<GameObject>();

            foreach (KeyValuePair<GameObject, MonsterSpawn> resource in m_DisableMonster)
            {
                float distance = Vector3.Distance(resource.Key.transform.position, playerPos);
                if (distance < m_DistanceToPlayerStopPhysique)
                {
                    monsterToAdd.Add(resource.Key);
                }
            }

            foreach (GameObject resource in monsterToAdd)
            {
                EnableMonster(resource);
            }
        }
    }

    private IEnumerator CoroutineCheckDistanceToDisable()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_TimeToCheckDistanceToPlayer);
            Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();

            List<GameObject> monsterToPop = new List<GameObject>();

            foreach (KeyValuePair<GameObject, MonsterSpawn> resource in m_EnableMonster)
            {
                float distance = Vector3.Distance(resource.Key.transform.position, playerPos);
                if (distance >= m_DistanceToPlayerStopPhysique)
                {
                    monsterToPop.Add(resource.Key);
                }
            }

            foreach (GameObject resource in monsterToPop)
            {
                DisableMonster(resource);
            }
        }
    }

    private IEnumerator CoroutineTimeToLive(GameObject monster)
    {
        yield return new WaitForSeconds(m_TimeToLiveDisabled);
        m_CoroutineTimeToLive[monster] = null;
        DispawnMonster(monster);
    }

    private void SpawnMonster(MonsterSpawn monsterSpawn)
    {
        Dictionary<TypeSpawn, List<Vector2Int>> spawnPoint = GetSpawnPoints();
        if(spawnPoint[monsterSpawn.typeSpawn].Count == 0)
        {
            return;
        }

        GameObject objectMonster = Pool.m_Instance.GetObject(monsterSpawn.monster);

        int index = Random.Range(0, spawnPoint[monsterSpawn.typeSpawn].Count);
        Vector2Int localPos = spawnPoint[monsterSpawn.typeSpawn][index];
        Vector3 worldPos = new Vector3(localPos.x + 0.5f, localPos.y, -3.0f);
        objectMonster.transform.position = worldPos;

        m_EnableMonster.Add(objectMonster, monsterSpawn);

        m_CurrStrength += monsterSpawn.strength;

        objectMonster.SetActive(true);
    }

    public void DispawnMonster(GameObject monster)
    {
        EnumMonster type = EnumMonster.zombie;
        bool typeIsInit = false;
        if(m_EnableMonster.ContainsKey(monster))
        {
            type = m_EnableMonster[monster].monster;
            m_CurrStrength -= m_EnableMonster[monster].strength;
            typeIsInit = true;
            m_EnableMonster.Remove(monster);
        }
        if(m_DisableMonster.ContainsKey(monster))
        {
            type = m_DisableMonster[monster].monster;
            typeIsInit = true;
            m_DisableMonster.Remove(monster);
        }
        if (m_CoroutineTimeToLive.ContainsKey(monster))
        {
            if (m_CoroutineTimeToLive[monster] != null)
            {
                StopCoroutine(m_CoroutineTimeToLive[monster]);
            }
            m_CoroutineTimeToLive.Remove(monster);
        }

        if(typeIsInit)
        {
            Pool.m_Instance.RemoveObject(monster, type);
        }
    }

    private void EnableMonster(GameObject monster)
    {
        EnumBiomes currBiomes = Map.m_Instance.GetGrid().GetBiomeAtCurrPoint();

        if (m_CurrStrength >= m_DictBiome[currBiomes].maxStrength + m_DictBiome[currBiomes].offsetStrength)
        {
            DispawnMonster(monster);
            return;
        }

        m_EnableMonster.Add(monster, m_DisableMonster[monster]);
        m_DisableMonster.Remove(monster);
        
        if(m_CoroutineTimeToLive.ContainsKey(monster))
        {
            if (m_CoroutineTimeToLive[monster] != null)
            {
                StopCoroutine(m_CoroutineTimeToLive[monster]);
            }
            m_CoroutineTimeToLive.Remove(monster);
        }

        m_CurrStrength += m_EnableMonster[monster].strength;

        monster.SetActive(true);
    }

    private void DisableMonster(GameObject monster)
    {
        m_DisableMonster.Add(monster, m_EnableMonster[monster]);
        m_EnableMonster.Remove(monster);
        
        m_CoroutineTimeToLive.Add(monster, StartCoroutine(CoroutineTimeToLive(monster)));

        m_CurrStrength -= m_DisableMonster[monster].strength;

        monster.SetActive(false);
    }

    public Dictionary<TypeSpawn, List<Vector2Int>> GetSpawnPoints()
    {
        Dictionary<TypeSpawn, List<Vector2Int>> spawnPoints = new Dictionary<TypeSpawn, List<Vector2Int>>();
        spawnPoints.Add(TypeSpawn.spawnGround, new List<Vector2Int>());
        spawnPoints.Add(TypeSpawn.spawnAir, new List<Vector2Int>());
        spawnPoints.Add(TypeSpawn.bigSpawnGround, new List<Vector2Int>());
        spawnPoints.Add(TypeSpawn.bigSpawnAir, new List<Vector2Int>());

        Vector2Int midPoint = Map.m_Instance.GetGrid().GetPoint();
        EnumBlocks[,] grid = Map.m_Instance.GetGrid().GetGrid();
        Dictionary<EnumBlocks, EnumBlocks> backGroundBlocks = Map.m_Instance.GetGrid().GetBackGroundDict();

        for (int x = midPoint.x - m_DistanceSpawn; x <= midPoint.x + m_DistanceSpawn; x++)
        {
            if(x < 2 || x >= grid.GetLength(0) - 2)
            {
                continue;
            }

            for (int y = midPoint.y - m_DistanceSpawn; y <= midPoint.y + m_DistanceSpawn; y++)
            {
                if (y < 2 || y >= grid.GetLength(1) - 2)
                {
                    continue;
                }

                if (x >= midPoint.x - m_OffsetViewPlayer && x <= midPoint.x + m_OffsetViewPlayer
                    && y >= midPoint.y - m_OffsetViewPlayer && y <= midPoint.y + m_OffsetViewPlayer)
                {
                    y += (m_OffsetViewPlayer * 2) + 1;
                    continue;
                }

                bool bigSpawn = true;

                //regarde si c'est un gros spawn
                for (int i = x - 1; i <= x + 1; i++)
                {
                    for (int j = y; j <= y + 2; j++)
                    {
                        if (!backGroundBlocks.ContainsKey(grid[i, j]))
                        {
                            bigSpawn = false;
                        }
                    }
                }

                if (bigSpawn == true)
                {
                    bool ground = true;

                    for (int a = x - 1; a <= x + 1; a++)
                    {
                        if (backGroundBlocks.ContainsKey(grid[a, y - 1]))
                        {
                            ground = false;
                            break;
                        }
                    }

                    if (ground)
                    {
                        spawnPoints[TypeSpawn.bigSpawnGround].Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        spawnPoints[TypeSpawn.bigSpawnAir].Add(new Vector2Int(x, y));
                    }
                    continue;
                }

                //regarde si c'est un petit spawn
                if (backGroundBlocks.ContainsKey(grid[x, y]) && backGroundBlocks.ContainsKey(grid[x, y + 1]))
                {
                    if(backGroundBlocks.ContainsKey(grid[x, y - 1]))
                    {
                        spawnPoints[TypeSpawn.spawnAir].Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        spawnPoints[TypeSpawn.spawnGround].Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        return spawnPoints;
    }
}
