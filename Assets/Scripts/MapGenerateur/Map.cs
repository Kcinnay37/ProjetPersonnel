using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] EnumMaps m_EnumDataMap;
    [SerializeField] int m_InitialViewSize;
    [SerializeField] int m_RangeStepView;

    private EnumBlocks[,] m_Grid;
    private bool[,] m_DrawGrid;

    private float m_Scale = 1;
    private int m_Octaves = 1;
    private float m_Persistence = 1;
    private float m_Lacunarity = 1;
    private int m_Seed = 0;
    private AnimationCurve m_HeightCurve;
    private Vector2 m_Offset;
    private DataMap m_DataMap;
    private DataBiome m_DataCurrBiome;

    public static Map m_Instance;

    private int m_CurrGridChunkX = 0;
    private int m_CurrGridChunkY = 0;

    private Dictionary<int, List<DataMap.Biome>> m_DictDepthChunk;

    private Vector2 m_InitialPoint;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        GenerateMap();
    }

    public void SetInitialPoint(int offSet)
    {
        int width = m_Instance.m_Grid.GetLength(0);
        int height = m_Instance.m_Grid.GetLength(1);

        // Recherche d'un point de spawn près du centre de la grille
        for (int y = height / 2 - offSet; y < height / 2 + offSet; y++)
        {
            if (y < 1 || y >= m_Grid.GetLength(1) - 1)
            {
                continue;
            }
            for (int x = width / 2 - offSet; x < width / 2 + offSet; x++)
            {
                if (x < 1 || x >= m_Grid.GetLength(0) - 1)
                {
                    continue;
                }
                // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
                if (m_Instance.m_Grid[x, y] == EnumBlocks.backGroundEarth && m_Instance.m_Grid[x, y + 1] == EnumBlocks.backGroundEarth && m_Instance.m_Grid[x, y - 1] == EnumBlocks.earth)
                {
                    m_InitialPoint = new Vector2(x, y);
                    return;
                }
            }
        }
        m_InitialPoint = new Vector2(m_Instance.m_Grid.GetLength(0) / 2, m_Instance.m_Grid.GetLength(0) / 2);
        //SetInitialPoint(offSet + 10);
    }

    void ResetMapView()
    {
        //pour tout les chunk de la cave
        foreach (DataMap.Biome chunk in m_DataMap.mapBiomes)
        {
            //va chercher le data du chunk actuel
            DataBiome currDataChunk = (DataBiome)Pool.m_Instance.GetData(chunk.dataBiome);

            //pour tout les block du chunk
            foreach (DataBiome.Block block in currDataChunk.biomeBlocks)
            {
                //va chercher le data du block
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(block.block);
                //clear ca tilemap
                dataBlock.map.ClearAllTiles();
            }
        }
    }

    void InitialDraw()
    {
        for(int x = (int)m_InitialPoint.x - (m_InitialViewSize / 2); x < (int)m_InitialPoint.x + (m_InitialViewSize / 2); x++)
        {
            if(x < 0 || x >= m_Grid.GetLength(0))
            {
                continue;
            }

            for (int y = (int)m_InitialPoint.y - (m_InitialViewSize / 2); y < (int)m_InitialPoint.y + (m_InitialViewSize / 2); y++)
            {
                if (y < 0 || y >= m_Grid.GetLength(1))
                {
                    continue;
                }

                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_Grid[x, y]);
                dataBlock.map.SetTile(new Vector3Int(x, y, 0), dataBlock.tile);
            }
        }
    }


    public void GenerateRandomValues()
    {
        // Génération aléatoire de la valeur de scale entre minScale et maxScale
        if (m_DataCurrBiome.useRandomScale)
        {
            m_Scale = Random.Range(m_DataCurrBiome.minScale, m_DataCurrBiome.maxScale + 1);
        }

        // Génération aléatoire de la valeur de octaves entre minOctaves et maxOctaves
        if (m_DataCurrBiome.useRandomOctave)
        {
            m_Octaves = Random.Range(m_DataCurrBiome.minOctave, m_DataCurrBiome.maxOctave + 1);
        }

        // Génération aléatoire de la valeur de persistence entre minPersistence et maxPersistence
        if (m_DataCurrBiome.useRandomPersistence)
        {
            m_Persistence = Random.Range(m_DataCurrBiome.minPersistence, m_DataCurrBiome.maxPersistence + 1);
        }

        // Génération aléatoire de la valeur de lacunarity entre minLacunarity et maxLacunarity
        if (m_DataCurrBiome.useRandomLacunarity)
        {
            m_Lacunarity = Random.Range(m_DataCurrBiome.minLacunarity, m_DataCurrBiome.maxLacunarity + 1);
        }

        // Génération aléatoire de la valeur de seed entre minSeed et maxSeed
        if (m_DataCurrBiome.useRandomSeed)
        {
            m_Seed = Random.Range(m_DataCurrBiome.minSeed, m_DataCurrBiome.maxSeed + 1);
        }
    }

    public void GenerateMap()
    {
        InitValueMap();

        for (int y = 0; y < m_DataMap.nbChunkDown; y++)
        {
            m_CurrGridChunkY = y;

            for (int x = 0; x < m_DataMap.nbChunkRight; x++)
            {
                m_CurrGridChunkX = x;

                SetCurrChunk();
                InitValueChunk();
                GenerateCave();
            }
        }

        SetInitialPoint(25);

        ResetMapView();
        InitialDraw();
    }

    private void InitValueChunk()
    {
        m_Scale = m_DataCurrBiome.scale;

        m_Octaves = m_DataCurrBiome.octaves;

        m_Persistence = m_DataCurrBiome.persistence;

        m_Lacunarity = m_DataCurrBiome.lacunarity;

        m_Seed = m_DataCurrBiome.seed;

        m_HeightCurve = m_DataCurrBiome.heightCurve;

        m_Offset = m_DataCurrBiome.offset;

        GenerateRandomValues();
    }

    private void InitValueMap()
    {
        m_DataMap = (DataMap)Pool.m_Instance.GetData(m_EnumDataMap);
        m_Grid = new EnumBlocks[m_DataMap.nbChunkRight * m_DataMap.chunkWidth, m_DataMap.nbChunkDown * m_DataMap.chunkHeight];

        int drawGridX = m_Grid.GetLength(0) / m_InitialViewSize;
        if (drawGridX % m_InitialViewSize != 0)
        {
            drawGridX += 1;
        }
        int drawGridY = m_Grid.GetLength(1) / m_InitialViewSize;
        if (drawGridY % m_InitialViewSize != 0)
        {
            drawGridY += 1;
        }
        m_DrawGrid = new bool[drawGridX, drawGridY];

        if (m_DictDepthChunk == null)
        {
            m_DictDepthChunk = new Dictionary<int, List<DataMap.Biome>>();
        }
        else
        {
            m_DictDepthChunk.Clear();
        }

        for (int i = 0; i < m_DataMap.nbChunkDown; i++)
        {
            m_DictDepthChunk.Add(i, new List<DataMap.Biome>());
        }

        foreach (DataMap.Biome biome in m_DataMap.mapBiomes)
        {
            for (int i = biome.chunkMinDepth; i <= biome.chunkMaxDepth; i++)
            {
                List<DataMap.Biome> currList;
                if (m_DictDepthChunk.TryGetValue(i, out currList))
                {
                    currList.Add(biome);
                }
            }
        }
    }

    public void GenerateCave()
    {
        System.Random rand = new System.Random(m_Seed);

        // algorithme de bruit de Perlin pour générer un bruit de Perlin 2D
        //float[,] noiseMap = Procedural.GenerateNoiseMap(m_DataMap.chunkWidth, m_DataMap.chunkHeight, m_Scale, m_Octaves, m_Persistence, m_Lacunarity, rand.Next());
        float[,] noiseMap = Procedural.GenerateNoiseMap2(m_DataMap.chunkWidth, m_DataMap.chunkHeight, m_Scale, m_Octaves, m_Persistence, m_Lacunarity, rand.Next(), m_Offset, m_HeightCurve);

        // Parcour tout les element de la grid et defini il est de quelle type selon le bruit de perlin et les valeur de la cave
        for (int x = 0; x < m_DataMap.chunkWidth; x++)
        {
            for (int y = 0; y < m_DataMap.chunkHeight; y++)
            {
                foreach (DataBiome.Block block in m_DataCurrBiome.biomeBlocks)
                {
                    if (noiseMap[x, y] >= block.minValue && noiseMap[x, y] <= block.maxValue)
                    {
                        m_Grid[x + (m_DataMap.chunkWidth * m_CurrGridChunkX), y + (m_DataMap.chunkHeight * m_CurrGridChunkY)] = block.block;
                        int checkRarity = Random.Range(1, 101);
                        if (checkRarity <= block.rarity)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void SetCurrChunk()
    {
        int index = Random.Range(0, m_DictDepthChunk[m_CurrGridChunkY].Count);
        DataMap.Biome biome = m_DictDepthChunk[m_CurrGridChunkY][index];
        m_DataCurrBiome = (DataBiome)Pool.m_Instance.GetData(biome.dataBiome);

        int checkRarity = Random.Range(1, 101);
        if(checkRarity > biome.rarity)
        {
            SetCurrChunk();
        }
    }

    public static Vector2 GetInitialPoint()
    {
        return m_Instance.m_InitialPoint;
    }
}