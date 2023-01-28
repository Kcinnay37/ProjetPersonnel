using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] EnumData m_CaveGeneratorData;
    [SerializeField] int m_InitialViewSize;
    [SerializeField] int m_RangeStepView;

    private EnumData[,] m_Grid;
    private bool[,] m_DrawGrid;

    private float m_Scale = 1;
    private int m_Octaves = 1;
    private float m_Persistence = 1;
    private float m_Lacunarity = 1;
    private int m_Seed = 0;

    public static Map m_Instance;

    private DataCaveGenerator m_DataCaveGenerator;
    private DataCaveChunk m_DataCurrChunk;

    private int m_CurrGridChunkX = 0;
    private int m_CurrGridChunkY = 0;

    private Dictionary<int, List<DataCaveGenerator.CaveChunk>> m_DictDepthChunk;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        GenerateMap();
    }

    public static Vector2 GetInitialPoint(int offSet)
    {
        int width = m_Instance.m_Grid.GetLength(0);
        int height = m_Instance.m_Grid.GetLength(1);

        // Recherche d'un point de spawn près du centre de la grille
        for (int y = height / 2 - offSet; y < height / 2 + offSet; y++)
        {
            for (int x = width / 2 - offSet; x < width / 2 + offSet; x++)
            {
                // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
                if (m_Instance.m_Grid[x, y] == EnumData.backGroundRock && m_Instance.m_Grid[x, y + 1] == EnumData.backGroundRock && m_Instance.m_Grid[x, y - 1] == EnumData.rockNormal)
                {
                    return new Vector2(x, y);
                }
            }
        }

        // Si aucun point de spawn n'a été trouvé, retourne (0, 0)
        return Vector2.zero;
    }

    void ResetMapView()
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
    }

    void DrawGridAt(Vector2 pos)
    {
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        for (int i = 0; i < m_Grid.GetLength(0); i++)
        {
            for (int j = 0; j < m_Grid.GetLength(1); j++)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_Grid[i, j]);
                dataBlock.map.SetTile(new Vector3Int(i, j, 0), dataBlock.tile);
            }
        }
    }

    void DrawViewCase(Vector2 pos)
    {
        for(int x = (int)pos.x * m_InitialViewSize; x < ((int)pos.x * m_InitialViewSize) + m_InitialViewSize; x++)
        {
            for (int y = (int)pos.y * m_InitialViewSize; y < ((int)pos.y * m_InitialViewSize) + m_InitialViewSize; y++)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_Grid[x, y]);
                dataBlock.map.SetTile(new Vector3Int(x, y, 0), dataBlock.tile);
            }
        }
    }


    public void GenerateRandomValues()
    {
        // Génération aléatoire de la valeur de scale entre minScale et maxScale
        if (m_DataCurrChunk.useRandomScale)
        {
            m_Scale = Random.Range(m_DataCurrChunk.minScale, m_DataCurrChunk.maxScale + 1);
        }

        // Génération aléatoire de la valeur de octaves entre minOctaves et maxOctaves
        if (m_DataCurrChunk.useRandomOctave)
        {
            m_Octaves = Random.Range(m_DataCurrChunk.minOctave, m_DataCurrChunk.maxOctave + 1);
        }

        // Génération aléatoire de la valeur de persistence entre minPersistence et maxPersistence
        if (m_DataCurrChunk.useRandomPersistence)
        {
            m_Persistence = Random.Range(m_DataCurrChunk.minPersistence, m_DataCurrChunk.maxPersistence + 1);
        }

        // Génération aléatoire de la valeur de lacunarity entre minLacunarity et maxLacunarity
        if (m_DataCurrChunk.useRandomLacunarity)
        {
            m_Lacunarity = Random.Range(m_DataCurrChunk.minLacunarity, m_DataCurrChunk.maxLacunarity + 1);
        }

        // Génération aléatoire de la valeur de seed entre minSeed et maxSeed
        if (m_DataCurrChunk.useRandomSeed)
        {
            m_Seed = Random.Range(m_DataCurrChunk.minSeed, m_DataCurrChunk.maxSeed + 1);
        }
    }

    public void GenerateMap()
    {
        InitValueMap();

        for (int y = 0; y < m_DataCaveGenerator.nbChunkDown; y++)
        {
            m_CurrGridChunkY = y;

            for (int x = 0; x < m_DataCaveGenerator.nbChunkRight; x++)
            {
                m_CurrGridChunkX = x;

                SetCurrChunk();
                InitValueChunk();
                GenerateCave();
            }
        }

        ResetMapView();
        DrawViewCase(new Vector2(m_DrawGrid.GetLength(0) / 2, m_DrawGrid.GetLength(1) / 2));
    }

    private void InitValueChunk()
    {
        m_Scale = m_DataCurrChunk.scale;

        m_Octaves = m_DataCurrChunk.octaves;

        m_Persistence = m_DataCurrChunk.persistence;

        m_Lacunarity = m_DataCurrChunk.lacunarity;

        m_Seed = m_DataCurrChunk.seed;

        GenerateRandomValues();
    }

    private void InitValueMap()
    {
        m_DataCaveGenerator = (DataCaveGenerator)Pool.m_Instance.GetData(m_CaveGeneratorData);
        m_Grid = new EnumData[m_DataCaveGenerator.nbChunkRight * m_DataCaveGenerator.chunkWidth, m_DataCaveGenerator.nbChunkDown * m_DataCaveGenerator.chunkHeight];

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
            m_DictDepthChunk = new Dictionary<int, List<DataCaveGenerator.CaveChunk>>();
        }
        else
        {
            m_DictDepthChunk.Clear();
        }

        for (int i = 0; i < m_DataCaveGenerator.nbChunkDown; i++)
        {
            m_DictDepthChunk.Add(i, new List<DataCaveGenerator.CaveChunk>());
        }

        foreach (DataCaveGenerator.CaveChunk chunk in m_DataCaveGenerator.cave)
        {
            for (int i = chunk.chunkMinDepth; i <= chunk.chunkMaxDepth; i++)
            {
                List<DataCaveGenerator.CaveChunk> currList;
                if (m_DictDepthChunk.TryGetValue(i, out currList))
                {
                    currList.Add(chunk);
                }
            }
        }
    }

    public void GenerateCave()
    {
        System.Random rand = new System.Random(m_Seed);

        // algorithme de bruit de Perlin pour générer un bruit de Perlin 2D
        float[,] noiseMap = Noise.GenerateNoiseMap(m_DataCaveGenerator.chunkWidth, m_DataCaveGenerator.chunkHeight, m_Scale, m_Octaves, m_Persistence, m_Lacunarity, rand.Next());

        // Parcour tout les element de la grid et defini il est de quelle type selon le bruit de perlin et les valeur de la cave
        for (int x = 0; x < m_DataCaveGenerator.chunkWidth; x++)
        {
            for (int y = 0; y < m_DataCaveGenerator.chunkHeight; y++)
            {
                foreach (DataCaveChunk.Block block in m_DataCurrChunk.blocksList)
                {
                    if (noiseMap[x, y] >= block.minValue && noiseMap[x, y] <= block.maxValue)
                    {
                        m_Grid[x + (m_DataCaveGenerator.chunkWidth * m_CurrGridChunkX), y + (m_DataCaveGenerator.chunkHeight * m_CurrGridChunkY)] = block.block;
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
        DataCaveGenerator.CaveChunk caveChunk = m_DictDepthChunk[m_CurrGridChunkY][index];
        m_DataCurrChunk = (DataCaveChunk)Pool.m_Instance.GetData(caveChunk.dataChunk);

        int checkRarity = Random.Range(1, 101);
        if(checkRarity > caveChunk.rarity)
        {
            SetCurrChunk();
        }
    }
}