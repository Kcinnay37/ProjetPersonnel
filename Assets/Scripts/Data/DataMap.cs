using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataMap : ScriptableObject
{
    [System.Serializable]
    public struct Biome
    {
        public EnumBiomes dataBiome;
        public int chunkMinDepth;
        public int chunkMaxDepth;
        [Range(0, 1000)]
        public int rarity;
    }

    public List<Biome> mapBiomes;

    public int chunkWidth;
    public int chunkHeight;

    public int nbChunkRight;
    public int nbChunkDown;

    [Header("")]
    public int distanceView;
    public float timeUpdateView;
    public float timeUpdatePoint;

    [Header("")]
    public int seed = 0;
    public bool useRandomSeed = false;
    public int minSeed;
    public int maxSeed;

    [Header("")]
    public int offsetInitialPoint;
    public bool drawAllGrid;
}
