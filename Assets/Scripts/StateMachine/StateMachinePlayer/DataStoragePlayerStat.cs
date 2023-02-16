using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoragePlayerStat : DataStorage
{
    DataPlayer m_GlobalDataPlayer;

    private float m_WalkSpeed;
    private float m_RunSpeed;

    private float m_JumpForce;

    private int m_SizeEquip;
    private int m_SizeBackPack;

    private int m_MaxHealth;
    private int m_CurrHealth;

    private int m_MaxArmor;
    private int m_CurrArmor;

    private int m_MaxEnergy;
    private int m_CurrEnergy;

    private int damage;
    private int attackSpeed;
    private int damageBlock;

    public DataStoragePlayerStat(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        ResetStats();
    }

    public void ResetStats()
    {
        m_WalkSpeed = m_GlobalDataPlayer.baseWalkSpeed;
        m_RunSpeed = m_GlobalDataPlayer.baseRunSpeed;

        m_JumpForce = m_GlobalDataPlayer.baseJumpForce;

        m_SizeEquip = m_GlobalDataPlayer.baseSizeEquip;
        m_SizeBackPack = m_GlobalDataPlayer.baseSizeBackPack;

        m_MaxHealth = m_GlobalDataPlayer.baseMaxHealth;
        m_CurrHealth = m_GlobalDataPlayer.baseMaxHealth;

        m_MaxArmor = m_GlobalDataPlayer.baseMaxArmor;
        m_CurrArmor = m_GlobalDataPlayer.baseMaxArmor;

        m_MaxEnergy = m_GlobalDataPlayer.baseMaxEnergy;
        m_CurrEnergy = m_GlobalDataPlayer.baseMaxEnergy;

        damage = m_GlobalDataPlayer.baseDamage;
        attackSpeed = m_GlobalDataPlayer.baseDamage;
        damageBlock = m_GlobalDataPlayer.baseDamageBlock;
    }

    public int GetSizeInventoryEquip()
    {
        return m_SizeEquip;
    }
}
