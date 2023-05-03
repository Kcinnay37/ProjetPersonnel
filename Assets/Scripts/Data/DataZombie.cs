using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataZombie : ScriptableObject
{
    public float moveSpeed;
    public float jumpForce;
    public float visionAngle;
    public float visionDistance;
    public float distanceToAttack;
    public float attackRange;
    public float intervalAttack;
    public float maxHealth;
    public float waitForDispawn;
    public float delayStopAttackPlayer;
    public float damage;
    
    public Vector2Int sizeInAstar;
    public int jumpHeightInAstar;
    public int airMoveSpeedInAstar;

    public Vector2 offsetPointBotLeft;

    public Drops drop;

    public Vector2 offsetBlock;

    public EnumAudios deadSound;
    public EnumAudios attackSound;
    public EnumAudios hitSound;
}
