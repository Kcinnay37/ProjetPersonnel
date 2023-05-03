using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class DataBlock : DataResource
{
    public int health;

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
    public EnumBlocks instanceType;
    public EnumBlocks dataType;
    public float coneRadius;

    public EnumAudios hitSound;
    public EnumAudios destroySound;
    public EnumAudios placeSound;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }

    public override string GetText() { return base.GetText() + "Health: " + health.ToString() + "\n\n"; }
}