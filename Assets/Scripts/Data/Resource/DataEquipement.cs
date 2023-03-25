using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataEquipement : DataResource
{
    public struct BonusStat
    {

    }

    public BonusStat bonusStat;
    public Material playerMaterial;
    public EnumTypeEquipement typeEquipement;
    public string componentForMaterial;
}

public enum EnumTypeEquipement
{
    cloth,
    hat,
    pants,
    shoes,
    socks
}
