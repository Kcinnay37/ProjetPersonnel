using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataCaveChunk : ScriptableObject
{
    [System.Serializable]
    public struct Block
    {
        public EnumData block;
        public float minValue;
        public float maxValue;
    }

    public List<Block> blocksList = new List<Block>();

    [Header("")]
    public int gridWidth;
    public int gridHeight;

    [Header("")]
    [Tooltip("une valeur comprise entre 0,1 et 10 est généralement raisonnable")]
    public float scale = 1;
    public bool useRandomScale = false;
    public float minScale;
    public float maxScale;

    [Header("")]
    [Tooltip("une valeur comprise entre 1 et 8 est généralement raisonnable.")]
    public int octaves = 1;
    public bool useRandomOctave = false;
    public int minOctave;
    public int maxOctave;

    [Header("")]
    [Tooltip("une valeur comprise entre 0,1 et 1 est généralement raisonnable")]
    public float persistence = 1;
    public bool useRandomPersistence = false;
    public float minPersistence;
    public float maxPersistence;

    [Header("")]
    [Tooltip("une valeur comprise entre 1 et 3 est généralement raisonnable")]
    public float lacunarity = 1;
    public bool useRandomLacunarity = false;
    public float minLacunarity;
    public float maxLacunarity;

    [Header("")]
    public int seed = 0;
    public bool useRandomSeed = false;
    public int minSeed;
    public int maxSeed;
}
