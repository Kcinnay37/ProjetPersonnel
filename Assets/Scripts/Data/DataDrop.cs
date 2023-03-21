using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Drops
{
    public Vector2 offsetDrop;

    public List<DropConsumable> dropConsumables;
    public List<DropEquipement> dropEquipements;
    public List<DropTool> dropTools;
    public List<DropMaterial> dropMaterials;
}

[System.Serializable]
public struct DropConsumable
{
    public EnumConsumables type;
    public int stack;
    [Range(0, 1000)]
    public int rarity;
}


[System.Serializable]
public struct DropEquipement
{
    public EnumEquipements type;
    public int stack;
    [Range(0, 1000)]
    public int rarity;
}

[System.Serializable]
public struct DropTool
{
    public EnumTools type;
    public int stack;
    [Range(0, 1000)]
    public int rarity;
}

[System.Serializable]
public struct DropMaterial
{
    public EnumMaterial type;
    public int stack;
    [Range(0, 1000)]
    public int rarity;
}