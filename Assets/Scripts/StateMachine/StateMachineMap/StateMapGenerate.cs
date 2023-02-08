using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMapGenerate : StateData
{
    private StateMapManager m_StateMapManager;

    private float m_Scale = 1;
    private int m_Octaves = 1;
    private float m_Persistence = 1;
    private float m_Lacunarity = 1;
    private int m_Seed = 0;

    private AnimationCurve m_HeightCurve;

    private Vector2 m_Offset;

    private DataMap m_DataMap;
    private DataBiome m_DataCurrBiome;

    private int m_CurrGridChunkX = 0;
    private int m_CurrGridChunkY = 0;

    private Dictionary<int, List<DataMap.Biome>> m_DictDepthChunk;

    public StateMapGenerate(StateMachine stateMachine) : base(stateMachine)
    {

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

                SetCurrBiome();
                InitValueBiome();
                GenerateCave();
            }
        }
    }

    private void InitValueMap()
    {
        m_StateMapManager = (StateMapManager)m_StateMachine.GetState(EnumStatesMap.manager);
        m_DataMap = (DataMap)m_StateMachine.GetData();

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

        // algorithme de bruit de Perlin pour g�n�rer un bruit de Perlin 2D
        //float[,] noiseMap = Procedural.GenerateNoiseMap(m_DataMap.chunkWidth, m_DataMap.chunkHeight, m_Scale, m_Octaves, m_Persistence, m_Lacunarity, rand.Next());
        float[,] noiseMap = Procedural.GenerateNoiseMap(m_DataMap.chunkWidth, m_DataMap.chunkHeight, m_Scale, m_Octaves, m_Persistence, m_Lacunarity, rand.Next(), m_Offset, m_HeightCurve);

        // Parcour tout les element de la grid et defini il est de quelle type selon le bruit de perlin et les valeur de la cave
        for (int x = 0; x < m_DataMap.chunkWidth; x++)
        {
            for (int y = 0; y < m_DataMap.chunkHeight; y++)
            {
                foreach (DataBiome.Block block in m_DataCurrBiome.biomeBlocks)
                {
                    if (noiseMap[x, y] >= block.minValue && noiseMap[x, y] <= block.maxValue)
                    {
                        m_StateMapManager.GetGrid()[x + (m_DataMap.chunkWidth * m_CurrGridChunkX), y + (m_DataMap.chunkHeight * m_CurrGridChunkY)] = block.block;

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

    private void SetCurrBiome()
    {
        int index = Random.Range(0, m_DictDepthChunk[m_CurrGridChunkY].Count);
        DataMap.Biome biome = m_DictDepthChunk[m_CurrGridChunkY][index];
        m_DataCurrBiome = (DataBiome)Pool.m_Instance.GetData(biome.dataBiome);

        m_StateMapManager.SetGridBiomeAt(new Vector2Int(m_CurrGridChunkX, m_CurrGridChunkY), biome.dataBiome);

        int checkRarity = Random.Range(1, 101);
        if (checkRarity > biome.rarity)
        {
            SetCurrBiome();
        }
    }

    private void InitValueBiome()
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

    public void GenerateRandomValues()
    {
        // G�n�ration al�atoire de la valeur de scale entre minScale et maxScale
        if (m_DataCurrBiome.useRandomScale)
        {
            m_Scale = Random.Range(m_DataCurrBiome.minScale, m_DataCurrBiome.maxScale + 1);
        }

        // G�n�ration al�atoire de la valeur de octaves entre minOctaves et maxOctaves
        if (m_DataCurrBiome.useRandomOctave)
        {
            m_Octaves = Random.Range(m_DataCurrBiome.minOctave, m_DataCurrBiome.maxOctave + 1);
        }

        // G�n�ration al�atoire de la valeur de persistence entre minPersistence et maxPersistence
        if (m_DataCurrBiome.useRandomPersistence)
        {
            m_Persistence = Random.Range(m_DataCurrBiome.minPersistence, m_DataCurrBiome.maxPersistence + 1);
        }

        // G�n�ration al�atoire de la valeur de lacunarity entre minLacunarity et maxLacunarity
        if (m_DataCurrBiome.useRandomLacunarity)
        {
            m_Lacunarity = Random.Range(m_DataCurrBiome.minLacunarity, m_DataCurrBiome.maxLacunarity + 1);
        }

        // G�n�ration al�atoire de la valeur de seed entre minSeed et maxSeed
        if (m_DataCurrBiome.useRandomSeed)
        {
            m_Seed = Random.Range(m_DataCurrBiome.minSeed, m_DataCurrBiome.maxSeed + 1);
        }
    }
}
