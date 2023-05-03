using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataEquipement : DataResource
{
    [System.Serializable]
    public struct BonusStat
    {
        public int health;
        public int healthRegene;
        public int armor;
        public int armorRegene;
    }

    public BonusStat bonusStat;
    public Material playerMaterial;
    public EnumTypeEquipement typeEquipement;
    public string componentForMaterial;

    public EnumEquipements instanceType;
    public EnumEquipements dataType;

    public bool removeHair;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }

    public override string GetText()
    {
        return base.GetText() +
            "BonusHealth: " + bonusStat.health + "\n" +
            "BonusHealthRegene" + bonusStat.healthRegene + "\n" +
            "BonusArmor: " + bonusStat.armor + "\n" +
            "BonusArmorRegene: " + bonusStat.armorRegene + "\n\n";
    }
}

public enum EnumTypeEquipement
{
    cloth,
    hat,
    pants,
    shoes,
}
