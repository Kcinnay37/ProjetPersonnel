using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerate
{
    private Map m_Map;

    private float m_Scale = 1;
    private int m_Octaves = 1;
    private float m_Persistence = 1;
    private float m_Lacunarity = 1;
    private int m_Seed = 0;

    private AnimationCurve m_HeightCurve;

    private Vector2 m_Offset;

    private DataBiome m_DataCurrBiome;

    private int m_CurrGridChunkX = 0;
    private int m_CurrGridChunkY = 0;

    private Dictionary<int, List<DataMap.Biome>> m_DictDepthChunk;

    public MapGenerate(Map map)
    {
        m_Map = map;
    }

    public void GenerateMap()
    {
        InitValueMap();

        for (int y = 0; y < m_Map.GetData().nbChunkDown; y++)
        {
            m_CurrGridChunkY = y;

            for (int x = 0; x < m_Map.GetData().nbChunkRight; x++)
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
        if (m_DictDepthChunk == null)
        {
            m_DictDepthChunk = new Dictionary<int, List<DataMap.Biome>>();
        }
        else
        {
            m_DictDepthChunk.Clear();
        }

        for (int i = 0; i < m_Map.GetData().nbChunkDown; i++)
        {
            m_DictDepthChunk.Add(i, new List<DataMap.Biome>());
        }

        foreach (DataMap.Biome biome in m_Map.GetData().mapBiomes)
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
        float[,] noiseMap = Procedural.GenerateNoiseMap(m_Map.GetData().chunkWidth, m_Map.GetData().chunkHeight, m_Scale, m_Octaves, m_Persistence, m_Lacunarity, rand.Next(), m_Offset, m_HeightCurve);

        // Parcour tout les element de la grid et defini il est de quelle type selon le bruit de perlin et les valeur de la cave
        for (int x = 0; x < m_Map.GetData().chunkWidth; x++)
        {
            for (int y = 0; y < m_Map.GetData().chunkHeight; y++)
            {
                foreach (DataBiome.Block block in m_DataCurrBiome.biomeBlocks)
                {
                    if (noiseMap[x, y] >= block.minValue && noiseMap[x, y] <= block.maxValue)
                    {
                        m_Map.GetGrid().GetGrid()[x + (m_Map.GetData().chunkWidth * m_CurrGridChunkX), y + (m_Map.GetData().chunkHeight * m_CurrGridChunkY)] = block.block;

                        int checkRarity = Random.Range(1, 1001);
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

        m_Map.GetGrid().SetGridBiomeAt(new Vector2Int(m_CurrGridChunkX, m_CurrGridChunkY), biome.dataBiome);

        int checkRarity = Random.Range(1, 1001);
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

        m_Seed = m_Map.GetData().seed;

        m_HeightCurve = m_DataCurrBiome.heightCurve;

        m_Offset = m_DataCurrBiome.offset;

        GenerateRandomValues();
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
        if (m_Map.GetData().useRandomSeed)
        {
            m_Seed = Random.Range(m_Map.GetData().minSeed, m_Map.GetData().maxSeed + 1);
        }
    }
}
