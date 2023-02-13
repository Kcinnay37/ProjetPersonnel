using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerData : StateData
{
    DataPlayer m_DataPlayer;

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

    public StatePlayerData(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_DataPlayer = (DataPlayer)m_StateMachine.GetData();

        ResetStats();
    }

    public void ResetStats()
    {
        m_WalkSpeed = m_DataPlayer.baseWalkSpeed;
        m_RunSpeed = m_DataPlayer.baseRunSpeed;

        m_JumpForce = m_DataPlayer.baseJumpForce;

        m_SizeEquip = m_DataPlayer.baseSizeEquip;
        m_SizeBackPack = m_DataPlayer.baseSizeBackPack;

        m_MaxHealth = m_DataPlayer.baseMaxHealth;
        m_CurrHealth = m_DataPlayer.baseMaxHealth;

        m_MaxArmor = m_DataPlayer.baseMaxArmor;
        m_CurrArmor = m_DataPlayer.baseMaxArmor;

        m_MaxEnergy = m_DataPlayer.baseMaxEnergy;
        m_CurrEnergy = m_DataPlayer.baseMaxEnergy;

        damage = m_DataPlayer.baseDamage;
        attackSpeed = m_DataPlayer.baseDamage;
        damageBlock = m_DataPlayer.baseDamageBlock;
    }

    public int GetSizeInventoryEquip()
    {
        return m_SizeEquip;
    }
}
