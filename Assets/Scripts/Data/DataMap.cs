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
        [Range(0, 100)]
        public int rarity;
    }

    public List<Biome> mapBiomes;

    public int chunkWidth;
    public int chunkHeight;

    public int nbChunkRight;
    public int nbChunkDown;

    [Header("")]
    public int chunkViewSize;

    public int nbChunkView;

    public float timeUpdateView;
}
