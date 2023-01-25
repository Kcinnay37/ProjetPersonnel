using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] EnumData caveResource;

    private Dictionary<int, DataBlock> m_Blocks;

    private float scale = 1;

    private int octaves = 1;

    private float persistence = 1;

    private float lacunarity = 1;

    private int seed = 0;

    int[,] grid;

    static MapGenerator m_Instance;

    DataCave m_Data;

    private void Awake()
    {
        GenerateMap();
    }

    const int offSetSpawnPlayer = 20;
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
                if (m_Instance.grid[x, y] == 0 && m_Instance.grid[x, y + 1] == 0 && m_Instance.grid[x, y - 1] == 1)
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
        foreach (DataBlock block in m_Data.blocksList)
        {
            block.map.ClearAllTiles();
        }

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                m_Data.blocksList[grid[i, j]].map.SetTile(new Vector3Int(i, j, 0), m_Data.blocksList[grid[i, j]].tile);
            }
        }
    }


    public void GenerateRandomValues(float minScale, float maxScale, int minOctaves, int maxOctaves, float minPersistence, float maxPersistence, float minLacunarity, float maxLacunarity, int minSeed, int maxSeed)
    {
        // Génération aléatoire de la valeur de scale entre minScale et maxScale
        if(m_Data.useRandomScale)
        {
            scale = Random.Range(minScale, maxScale + 1);
        }

        // Génération aléatoire de la valeur de octaves entre minOctaves et maxOctaves
        if(m_Data.useRandomOctave)
        {
            octaves = Random.Range(minOctaves, maxOctaves + 1);
        }

        // Génération aléatoire de la valeur de persistence entre minPersistence et maxPersistence
        if(m_Data.useRandomPersistence)
        {
            persistence = Random.Range(minPersistence, maxPersistence + 1);
        }

        // Génération aléatoire de la valeur de lacunarity entre minLacunarity et maxLacunarity
        if(m_Data.useRandomLacunarity)
        {
            lacunarity = Random.Range(minLacunarity, maxLacunarity + 1);
        }

        // Génération aléatoire de la valeur de seed entre minSeed et maxSeed
        if(m_Data.useRandomSeed)
        {
            seed = Random.Range(minSeed, maxSeed + 1);
        }
    }

    public void GenerateMap()
    {
        InitValue();
        GenerateRandomValues(m_Data.minScale, m_Data.maxScale, m_Data.minOctave, m_Data.maxOctave, m_Data.minPersistence, m_Data.maxPersistence, m_Data.minLacunarity, m_Data.maxLacunarity, m_Data.minSeed, m_Data.maxSeed);
        GenerateCave();
        DrawGrid();
    }

    private void InitValue()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        m_Data = (DataCave)Pool.m_Instance.GetData(caveResource);

        if (m_Blocks == null)
        {
            m_Blocks = new Dictionary<int, DataBlock>();
        }
        m_Blocks.Clear();
        for(int i = 0; i < m_Data.blocksList.Count; i++)
        {
            m_Blocks.Add(i, m_Data.blocksList[i]);
        }

        scale = m_Data.scale;

        octaves = m_Data.octaves;

        persistence = m_Data.persistence;

        lacunarity = m_Data.lacunarity;

        seed = m_Data.seed;

        grid = new int[m_Data.gridWidth, m_Data.gridHeight];
    }

    public void GenerateCave()
    {
        // On utilise un germe aléatoire pour initialiser la graine du générateur de nombres aléatoires
        System.Random rand = new System.Random(seed);
        // On utilise l'algorithme de bruit de Perlin pour générer un bruit de Perlin 2D
        float[,] noiseMap = Noise.GenerateNoiseMap(grid.GetLength(0), grid.GetLength(1), scale, octaves, persistence, lacunarity, rand.Next());

        // On parcourt tous les éléments de la grille
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                int count = 0;
                foreach(DataBlock block in m_Data.blocksList)
                {
                    if (noiseMap[x, y] >= block.minValue && noiseMap[x, y] <= block.maxValue)
                    {
                        grid[x, y] = count;
                    }
                    count++;
                }
            }
        }
    }

    public void FillBorders()
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Remplissage de la première et de la dernière colonne
        for (int y = 0; y < height; y++)
        {
            grid[0, y] = 1;
            grid[width - 1, y] = 1;
        }

        // Remplissage de la première et de la dernière ligne
        for (int x = 0; x < width; x++)
        {
            grid[x, 0] = 1;
            grid[x, height - 1] = 1;
        }
    }
}