using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] EnumData caveGeneratorData;

    private float scale = 1;

    private int octaves = 1;

    private float persistence = 1;

    private float lacunarity = 1;

    private int seed = 0;

    EnumData[,] grid;

    public static MapGenerator m_Instance;

    private DataCaveGenerator m_DataCaveGenerator;
    private DataCaveChunk m_DataCurrChunk;

    private int currGridChunkX = 0;
    private int currGridChunkY = 0;

    private Dictionary<int, List<DataCaveGenerator.CaveChunk>> m_DictDepthChunk;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        GenerateMap();
    }

    const int offSetSpawnPlayer = 50;
    public static Vector2 FindPlayerSpawnPoint()
    {
        int width = m_Instance.grid.GetLength(0);
        int height = m_Instance.grid.GetLength(1);

        // Recherche d'un point de spawn près du centre de la grille
        for (int y = height / 2 - offSetSpawnPlayer; y < height / 2 + offSetSpawnPlayer; y++)
        {
            for (int x = width / 2 - offSetSpawnPlayer; x < width / 2 + offSetSpawnPlayer; x++)
            {
                // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
                if (m_Instance.grid[x, y] == EnumData.backGroundRock && m_Instance.grid[x, y + 1] == EnumData.backGroundRock && m_Instance.grid[x, y - 1] == EnumData.rockNormal)
                {
                    return new Vector2(x, y);
                }
            }
        }

        // Si aucun point de spawn n'a été trouvé, retourne (0, 0)
        return Vector2.zero;
    }

    void DrawGrid()
    {
        //pour tout les chunk de la cave
        foreach (DataCaveGenerator.CaveChunk chunk in m_DataCaveGenerator.cave)
        {
            //va chercher le data du chunk actuel
            DataCaveChunk currDataChunk = (DataCaveChunk)Pool.m_Instance.GetData(chunk.dataChunk);

            //pour tout les block du chunk
            foreach (DataCaveChunk.Block block in currDataChunk.blocksList)
            {
                //va chercher le data du block
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(block.block);
                //clear ca tilemap
                dataBlock.map.ClearAllTiles();
            }
        }

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(grid[i, j]);
                dataBlock.map.SetTile(new Vector3Int(i, j, 0), dataBlock.tile);
            }
        }
    }


    public void GenerateRandomValues()
    {
        // Génération aléatoire de la valeur de scale entre minScale et maxScale
        if (m_DataCurrChunk.useRandomScale)
        {
            scale = Random.Range(m_DataCurrChunk.minScale, m_DataCurrChunk.maxScale + 1);
        }

        // Génération aléatoire de la valeur de octaves entre minOctaves et maxOctaves
        if (m_DataCurrChunk.useRandomOctave)
        {
            octaves = Random.Range(m_DataCurrChunk.minOctave, m_DataCurrChunk.maxOctave + 1);
        }

        // Génération aléatoire de la valeur de persistence entre minPersistence et maxPersistence
        if (m_DataCurrChunk.useRandomPersistence)
        {
            persistence = Random.Range(m_DataCurrChunk.minPersistence, m_DataCurrChunk.maxPersistence + 1);
        }

        // Génération aléatoire de la valeur de lacunarity entre minLacunarity et maxLacunarity
        if (m_DataCurrChunk.useRandomLacunarity)
        {
            lacunarity = Random.Range(m_DataCurrChunk.minLacunarity, m_DataCurrChunk.maxLacunarity + 1);
        }

        // Génération aléatoire de la valeur de seed entre minSeed et maxSeed
        if (m_DataCurrChunk.useRandomSeed)
        {
            seed = Random.Range(m_DataCurrChunk.minSeed, m_DataCurrChunk.maxSeed + 1);
        }
    }

    public void GenerateMap()
    {
        // init les value ----------------------------
        m_DataCaveGenerator = (DataCaveGenerator)Pool.m_Instance.GetData(caveGeneratorData);
        grid = new EnumData[m_DataCaveGenerator.nbChunkRight * m_DataCaveGenerator.chunkWidth, m_DataCaveGenerator.nbChunkDown * m_DataCaveGenerator.chunkHeight];

        if(m_DictDepthChunk == null)
        {
            m_DictDepthChunk = new Dictionary<int, List<DataCaveGenerator.CaveChunk>>();
        }
        else
        {
            m_DictDepthChunk.Clear();
        }

        for(int i = 0; i < m_DataCaveGenerator.nbChunkDown; i++)
        {
            m_DictDepthChunk.Add(i, new List<DataCaveGenerator.CaveChunk>());
        }

        foreach(DataCaveGenerator.CaveChunk chunk in m_DataCaveGenerator.cave)
        {
            for(int i = chunk.chunkMinDepth; i <= chunk.chunkMaxDepth; i++)
            {
                List<DataCaveGenerator.CaveChunk> currList;
                if(m_DictDepthChunk.TryGetValue(i, out currList))
                {
                    currList.Add(chunk);
                }
            }
        }
        // ----------------------------------------------

        for (int y = 0; y < m_DataCaveGenerator.nbChunkDown; y++)
        {
            currGridChunkY = y;

            for (int x = 0; x < m_DataCaveGenerator.nbChunkRight; x++)
            {
                currGridChunkX = x;

                SetCurrChunk();
                InitValue();
                GenerateCave();
            }
        }

        DrawGrid();
    }

    private void InitValue()
    {
        scale = m_DataCurrChunk.scale;

        octaves = m_DataCurrChunk.octaves;

        persistence = m_DataCurrChunk.persistence;

        lacunarity = m_DataCurrChunk.lacunarity;

        seed = m_DataCurrChunk.seed;

        GenerateRandomValues();
    }

    public void GenerateCave()
    {
        System.Random rand = new System.Random(seed);

        // algorithme de bruit de Perlin pour générer un bruit de Perlin 2D
        float[,] noiseMap = Noise.GenerateNoiseMap(m_DataCaveGenerator.chunkWidth, m_DataCaveGenerator.chunkHeight, scale, octaves, persistence, lacunarity, rand.Next());

        // Parcour tout les element de la grid et defini il est de quelle type selon le bruit de perlin et les valeur de la cave
        for (int x = 0; x < m_DataCaveGenerator.chunkWidth; x++)
        {
            for (int y = 0; y < m_DataCaveGenerator.chunkHeight; y++)
            {
                foreach (DataCaveChunk.Block block in m_DataCurrChunk.blocksList)
                {
                    if (noiseMap[x, y] >= block.minValue && noiseMap[x, y] <= block.maxValue)
                    {
                        grid[x + (m_DataCaveGenerator.chunkWidth * currGridChunkX), y + (m_DataCaveGenerator.chunkHeight * currGridChunkY)] = block.block;
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
        int index = Random.Range(0, m_DictDepthChunk[currGridChunkY].Count);
        DataCaveGenerator.CaveChunk caveChunk = m_DictDepthChunk[currGridChunkY][index];
        m_DataCurrChunk = (DataCaveChunk)Pool.m_Instance.GetData(caveChunk.dataChunk);

        int checkRarity = Random.Range(1, 101);
        if(checkRarity > caveChunk.rarity)
        {
            SetCurrChunk();
        }
    }
}