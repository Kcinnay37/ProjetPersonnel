using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataPlayer : ScriptableObject
{
    [Header("Key")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;
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

    public float regeneArmor;
    public float waitForRegeneArmor;

    public float regeneHealth;
    public float waitForRegeneHealth;

    public float blockDropDistance;

    public float gravityScale;

    public Vector2 offsetBlock;

    public Material baseHat;
    public Material baseCloth;
    public Material basePants;
    public Material baseShoes;
    public Material hatNone;

    public EnumAudios hitSound;
    public EnumAudios deadSound;
}
