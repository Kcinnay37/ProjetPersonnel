using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataBiome : ScriptableObject
{
    [System.Serializable]
    public struct Block
    {
        public EnumBlocks block;
        public float minValue;
        public float maxValue;
        [Range(0, 100)]
        public int rarity;
    }

    public List<Block> biomeBlocks = new List<Block>();

    [Header("")]
    [Tooltip("une valeur comprise entre 0,1 et 10 est généralement raisonnable")]
    [Range(0.0f, 30f)]
    public float scale = 1;
    public bool useRandomScale = false;
    [Range(0.0f, 10f)]
    public float minScale;
    [Range(0.0f, 10f)]
    public float maxScale;

    [Header("")]
    [Tooltip("une valeur comprise entre 1 et 8 est généralement raisonnable.")]
    [Range(1, 8)]
    public int octaves = 1;
    public bool useRandomOctave = false;
    [Range(0, 8)]
    public int minOctave;
    [Range(0, 8)]
    public int maxOctave;

    [Header("")]
    [Tooltip("une valeur comprise entre 0,1 et 1 est généralement raisonnable")]
    [Range(0.0f, 1f)]
    public float persistence = 1;
    public bool useRandomPersistence = false;
    [Range(0.0f, 1f)]
    public float minPersistence;
    [Range(0.0f, 1f)]
    public float maxPersistence;

    [Header("")]
    [Tooltip("une valeur comprise entre 1 et 3 est généralement raisonnable")]
    [Range(0.0f, 3f)]
    public float lacunarity = 1;
    public bool useRandomLacunarity = false;
    [Range(0.0f, 3f)]
    public float minLacunarity;
    [Range(0.0f, 3f)]
    public float maxLacunarity;

    [Header("")]
    public int seed = 0;
    public bool useRandomSeed = false;
    public int minSeed;
    public int maxSeed;

    [Header("")]
    public AnimationCurve heightCurve;

    [Header("")]
    public Vector2 offset;
}
