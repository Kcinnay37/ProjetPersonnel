using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataPlayer : ScriptableObject
{
    [Header("Key")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode runKey;

    public KeyCode jumpKeyCode;

    public KeyCode primarySlotKey;
    public KeyCode secondarySlotKey;

    public KeyCode dropResourceEquipKey;

    public KeyCode openBackpackKey;

    [Header("Stat")]
    public float baseWalkSpeed;
    public float baseRunSpeed;

    public float baseJumpForce;

    public int baseSizeEquip;
    public int baseSizeBackPack;

    public int baseMaxHealth;
    public int baseMaxArmor;
    public int baseMaxEnergy;
    public int baseDamage;
    public int baseAttackSpeed;

    public float blockDropDistance;

    public Material baseHat;
    public Material baseCloth;
    public Material basePants;
    public Material baseShoes;
    public Material hatNone;
}
