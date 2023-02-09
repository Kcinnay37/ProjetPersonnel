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

    [Header("Stat")]
    public float walkSpeed;
    public float runSpeed;

    public float jumpForce;

    public int sizeEquip;
    public int sizeBackPack;

    public int health;
    public int armor;
    public int energy;
    public int damage;
    public int attackSpeed;
    public int damageBlock;
}
