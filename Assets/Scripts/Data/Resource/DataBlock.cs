using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class DataBlock : DataResource
{
    private Tilemap tilemap;
    public Tilemap map
    {
        get
        {
            if(tilemap == null)
            {
                tilemap = GameObject.Find(mapName).GetComponent<Tilemap>();
            }
            return tilemap;
        }
    }
    public string mapName;
    public TileBase tile;
    public Sprite sprite;
    public float minValue;
    public float maxValue;
}