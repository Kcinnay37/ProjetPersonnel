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

        Dictionary<EnumBlocks, EnumBlocks> blockCanGo = m_Map.GetGrid().GetBackGroundDict();

        // Parcour tout les element de la grid et defini il est de quelle type selon le bruit de perlin et les valeur de la cave
        for (int x = 0; x < m_Map.GetData().chunkWidth; x++)
        {
            for (int y = 0; y < m_Map.GetData().chunkHeight; y++)
            {
                //regarde pour placer le block a l'emplacement
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

        Dictionary<Vector2Int, Vector2Int> cantSpawnCase = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, List<Vector2Int>> occupyCase = new Dictionary<Vector2Int, List<Vector2Int>>();

        for (int x = 0; x < m_Map.GetData().chunkWidth; x++)
        {
            for (int y = 0; y < m_Map.GetData().chunkHeight; y++)
            {
                //regarde pour placer un resource à l'emplacement
                foreach (DataBiome.AddInWorld resourceToAdd in m_DataCurrBiome.biomeAdd)
                {
                    if (noiseMap[x, y] >= resourceToAdd.minValue && noiseMap[x, y] <= resourceToAdd.maxValue)
                    {
                        int checkRarity = Random.Range(1, 1001);
                        if (checkRarity <= resourceToAdd.rarity)
                        {
                            Vector2Int pos = new Vector2Int(x + (m_Map.GetData().chunkWidth * m_CurrGridChunkX), y + (m_Map.GetData().chunkHeight * m_CurrGridChunkY));
                            m_Map.GetGrid().GetCollectible().Add(pos, resourceToAdd.collectibles);

                            DataAddInWorld dataCollectible = (DataAddInWorld)Pool.m_Instance.GetData(resourceToAdd.collectibles);
                            if (dataCollectible.gridSpawn == null)
                            {
                                dataCollectible.LoadData();
                            }

                            bool stop = false;

                            //si il a pas de block en dessous
                            EnumBlocks[,] grid = Map.m_Instance.GetGrid().GetGrid();
                            for (int x1 = 0; x1 <= dataCollectible.gridSpawnSideAndHeightSize.x; x1++)
                            {
                                Vector2Int belowPos1 = new Vector2Int(pos.x + x1, pos.y - 1);
                                Vector2Int belowPos2 = new Vector2Int(pos.x - x1, pos.y - 1);

                                //si c'est pas dans la map
                                if (belowPos1.x <= 0 || belowPos1.x >= m_Map.GetGrid().GetGrid().GetLength(0) || belowPos2.x <= 0 || belowPos2.x >= m_Map.GetGrid().GetGrid().GetLength(0))
                                {
                                    m_Map.GetGrid().GetCollectible().Remove(pos);
                                    stop = true;
                                    break;
                                }
                                else if (belowPos1.y <= 0 || belowPos1.y >= m_Map.GetGrid().GetGrid().GetLength(1) || belowPos1.y <= 0 || belowPos1.y >= m_Map.GetGrid().GetGrid().GetLength(1))
                                {
                                    m_Map.GetGrid().GetCollectible().Remove(pos);
                                    stop = true;
                                    break;
                                }


                                if ((blockCanGo.ContainsKey(grid[belowPos1.x, belowPos1.y]) && dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x + x1, dataCollectible.gridSpawnSideAndHeightSize.y])
                                    || (blockCanGo.ContainsKey(grid[belowPos2.x, belowPos2.y]) && dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x - x1, dataCollectible.gridSpawnSideAndHeightSize.y]))
                                {
                                    m_Map.GetGrid().GetCollectible().Remove(pos);
                                    stop = true;
                                    break;
                                }
                            }

                            if(!stop)
                            {
                                for (int x1 = 0; x1 <= dataCollectible.gridSpawnSideAndHeightSize.x; x1++)
                                {
                                    for (int y1 = 0; y1 <= dataCollectible.gridSpawnSideAndHeightSize.y; y1++)
                                    {
                                        //si c'est pas dans la map
                                        if (pos.x - x1 <= 0 || pos.x + x1 >= m_Map.GetGrid().GetGrid().GetLength(0))
                                        {
                                            m_Map.GetGrid().GetCollectible().Remove(pos);
                                            stop = true;
                                            break;
                                        }
                                        else if (pos.y <= 0 || pos.y + y1 >= m_Map.GetGrid().GetGrid().GetLength(1))
                                        {
                                            m_Map.GetGrid().GetCollectible().Remove(pos);
                                            stop = true;
                                            break;
                                        }

                                        //si il a un block dans les jambe
                                        if (!blockCanGo.ContainsKey(m_Map.GetGrid().GetGrid()[pos.x + x1, pos.y + y1]) && dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x + x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                        {
                                            m_Map.GetGrid().GetCollectible().Remove(pos);
                                            stop = true;
                                            break;
                                        }
                                        if (!blockCanGo.ContainsKey(m_Map.GetGrid().GetGrid()[pos.x - x1, pos.y + y1]) && dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x - x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                        {
                                            m_Map.GetGrid().GetCollectible().Remove(pos);
                                            stop = true;
                                            break;
                                        }

                                        if (cantSpawnCase.ContainsKey(new Vector2Int(pos.x + x1, pos.y + y1)) && dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x + x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                        {
                                            m_Map.GetGrid().GetCollectible().Remove(pos);
                                            stop = true;
                                            break;
                                        }
                                        if (cantSpawnCase.ContainsKey(new Vector2Int(pos.x - x1, pos.y + y1)) && dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x - x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                        {
                                            m_Map.GetGrid().GetCollectible().Remove(pos);
                                            stop = true;
                                            break;
                                        }
                                    }
                                    if (stop == true)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (!stop)
                            {
                                for (int x1 = 0; x1 <= dataCollectible.gridSpawnSideAndHeightSize.x; x1++)
                                {
                                    for (int y1 = 0; y1 <= dataCollectible.gridSpawnSideAndHeightSize.y; y1++)
                                    {
                                        if (dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x + x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                        {
                                            if(!occupyCase.ContainsKey(new Vector2Int(pos.x + x1, pos.y + y1)))
                                            {
                                                occupyCase.Add(new Vector2Int(pos.x + x1, pos.y + y1), new List<Vector2Int>());
                                            }
                                            occupyCase[new Vector2Int(pos.x + x1, pos.y + y1)].Add(new Vector2Int(pos.x, pos.y));

                                            //occupyCase.Add(new Vector2Int(pos.x + x1, pos.y + y1), new Vector2Int(pos.x, pos.y));
                                        }

                                        if (x1 != 0)
                                        {
                                            if (dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x - x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                            {
                                                if (!occupyCase.ContainsKey(new Vector2Int(pos.x - x1, pos.y + y1)))
                                                {
                                                    occupyCase.Add(new Vector2Int(pos.x - x1, pos.y + y1), new List<Vector2Int>());
                                                }
                                                occupyCase[new Vector2Int(pos.x - x1, pos.y + y1)].Add(new Vector2Int(pos.x, pos.y));

                                                //occupyCase.Add(new Vector2Int(pos.x - x1, pos.y + y1), new Vector2Int(pos.x, pos.y));
                                            }
                                        }
                                    }
                                }

                                if(resourceToAdd.cantGoOverIt)
                                {
                                    for (int x1 = 0; x1 <= dataCollectible.gridSpawnSideAndHeightSize.x; x1++)
                                    {
                                        for (int y1 = 0; y1 <= dataCollectible.gridSpawnSideAndHeightSize.y; y1++)
                                        {
                                            if (dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x + x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                            {
                                                Vector2Int currPos = new Vector2Int(pos.x + x1, pos.y + y1);
                                                cantSpawnCase.Add(currPos, new Vector2Int(pos.x, pos.y));

                                                if(occupyCase.ContainsKey(currPos))
                                                {
                                                    foreach (Vector2Int value in occupyCase[currPos])
                                                    {
                                                        if(m_Map.GetGrid().GetCollectible().ContainsKey(value) && value != pos)
                                                        {
                                                            
                                                            m_Map.GetGrid().GetCollectible().Remove(value);
                                                        }
                                                    }
                                                }
                                            }

                                            if (x1 != 0)
                                            {
                                                if (dataCollectible.gridSpawn[dataCollectible.gridSpawnSideAndHeightSize.x - x1, dataCollectible.gridSpawnSideAndHeightSize.y - y1])
                                                {
                                                    Vector2Int currPos = new Vector2Int(pos.x - x1, pos.y + y1);
                                                    cantSpawnCase.Add(currPos, new Vector2Int(pos.x, pos.y));

                                                    if (occupyCase.ContainsKey(currPos))
                                                    {
                                                        foreach (Vector2Int value in occupyCase[currPos])
                                                        {
                                                            if (m_Map.GetGrid().GetCollectible().ContainsKey(value) && value != pos)
                                                            {
                                                                
                                                                m_Map.GetGrid().GetCollectible().Remove(value);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                break;
                            }
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
