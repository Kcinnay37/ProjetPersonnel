using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataCaveGenerator : ScriptableObject
{
    [System.Serializable]
    public struct CaveChunk
    {
        public EnumData dataChunk;
        public int chunkMinDepth;
        public int chunkMaxDepth;
        [Range(0, 100)]
        public int rarity;
    }

    public List<CaveChunk> cave;

    public int chunkWidth;
    public int chunkHeight;

    public int nbChunkRight;
    public int nbChunkDown;
}
