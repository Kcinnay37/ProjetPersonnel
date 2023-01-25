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
        public int chunkDepth;
    }

    public int chunkWidth;
    public int chunkHeight;

    public int nbChunk;
}
