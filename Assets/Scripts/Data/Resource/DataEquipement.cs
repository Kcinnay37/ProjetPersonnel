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
        public int armor;
        public int energy;
        public int damage;
        public int attackSpeed;
    }

    public BonusStat bonusStat;
    public Material playerMaterial;
    public EnumTypeEquipement typeEquipement;
    public string componentForMaterial;

    public EnumEquipements instanceType;
    public EnumEquipements dataType;

    public override object GetInstanceType() { return instanceType; }
    public override object GetDataType() { return dataType; }

    public override string GetText()
    {
        return base.GetText() +
            "BonusHealth: " + bonusStat.health + "\n" +
            "BonusArmor: " + bonusStat.armor + "\n" +
            "BonusEnergy: " + bonusStat.energy + "\n" +
            "BonusDamage: " + bonusStat.damage + "\n" +
            "BonusAttackSpeed: " + bonusStat.attackSpeed + "\n\n";
    }
}

public enum EnumTypeEquipement
{
    cloth,
    hat,
    pants,
    shoes,
}
