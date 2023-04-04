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

    private float m_MaxHealth;
    private float m_CurrHealth;

    private float m_MaxArmor;
    private float m_CurrArmor;
    private float m_ArmorRegene;
    private float m_WaitForRegeneArmor;
    private Coroutine m_CoroutineArmor;

    private float m_MaxEnergy;
    private float m_CurrEnergy;
    private float m_EnergyRegene;
    private float m_WaitForRegeneEnergy;
    private Coroutine m_CoroutineEnergy;

    private float m_Damage;
    private float m_AttackSpeed;

    public DataStoragePlayerStat(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        InitStats();
    }

    public void InitStats()
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

        m_Damage = m_GlobalDataPlayer.baseDamage;
        m_AttackSpeed = m_GlobalDataPlayer.baseDamage;

        m_ArmorRegene = m_GlobalDataPlayer.regeneArmor;
        m_EnergyRegene = m_GlobalDataPlayer.regeneEnergy;
        m_WaitForRegeneArmor = m_GlobalDataPlayer.waitForRegeneArmor;
        m_WaitForRegeneEnergy = m_GlobalDataPlayer.waitForRegeneEnergy;

        UpdateSlider();
    }

    public void ResetMaxStats()
    {
        m_MaxHealth = m_GlobalDataPlayer.baseMaxHealth;
        if(m_CurrHealth > m_MaxHealth)
        {
            m_CurrHealth = m_MaxHealth;
        }

        m_MaxArmor = m_GlobalDataPlayer.baseMaxArmor;
        if(m_CurrArmor > m_MaxArmor)
        {
            m_CurrArmor = m_MaxArmor;
        }

        m_MaxEnergy = m_GlobalDataPlayer.baseMaxEnergy;
        if(m_CurrEnergy > m_MaxEnergy)
        {
            m_CurrEnergy = m_MaxEnergy;
        }

        m_Damage = m_GlobalDataPlayer.baseDamage;
        m_AttackSpeed = m_GlobalDataPlayer.baseDamage;

        UpdateSlider();
    }

    public void AddStats(DataEquipement.BonusStat stat)
    {
        m_MaxHealth += stat.health;
        m_MaxArmor += stat.armor;
        m_MaxEnergy += stat.energy;
        m_Damage += stat.damage;
        m_AttackSpeed += stat.attackSpeed;

        UpdateSlider();
    }

    public int GetSizeInventoryEquip()
    {
        return m_SizeEquip;
    }

    public void UpdateSlider()
    {
        UI.m_Instance.GetUIPlayerStat().UpdateSlider((float)m_CurrHealth / (float)m_MaxHealth, (float)m_CurrArmor / (float)m_MaxArmor, (float)m_CurrEnergy / (float)m_MaxEnergy);
    }

    private IEnumerator CoroutineWaitForRegeneArmor()
    {
        yield return new WaitForSeconds(m_WaitForRegeneArmor);
        m_CoroutineArmor = m_StateMachine.StartCoroutine(CoroutineRegeneArmor());
    }

    private IEnumerator CoroutineWaitForRegeneEnergy()
    {
        yield return new WaitForSeconds(m_WaitForRegeneEnergy);
        m_CoroutineEnergy = m_StateMachine.StartCoroutine(CoroutineRegeneEnergy());
    }

    private IEnumerator CoroutineRegeneArmor()
    {
        while (true)
        {
            yield return null;
            m_CurrArmor += m_ArmorRegene * Time.deltaTime;
            if(m_CurrArmor >= m_MaxArmor)
            {
                m_CurrArmor = m_MaxArmor;
                UpdateSlider();
                break;
            }
            UpdateSlider();
        }
        m_CoroutineArmor = null;
    }

    private IEnumerator CoroutineRegeneEnergy()
    {
        while(true)
        {
            yield return null;
            m_CurrEnergy += m_EnergyRegene * Time.deltaTime;
            if(m_CurrEnergy >= m_MaxEnergy)
            {
                m_CurrEnergy = m_MaxEnergy;
                UpdateSlider();
                break;
            }
            UpdateSlider();
        }
        m_CoroutineEnergy = null;
    }

    public void TakeDamage(float damage)
    {
        if(m_CoroutineArmor != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineArmor);
        }

        m_CurrArmor -= damage;
        if(m_CurrArmor < 0)
        {
            m_CurrHealth += m_CurrArmor;
            m_CurrArmor = 0;
        }

        if(m_CurrHealth < 0)
        {
            m_CurrHealth = 0;
            UpdateSlider();

            m_StateMachine.PopCurrState(EnumStatesPlayer.controllerMount);
            m_StateMachine.PopCurrState(EnumStatesPlayer.controllerMovement);
            m_StateMachine.PopCurrState(EnumStatesPlayer.controllerInventory);
            
            if(m_CoroutineEnergy != null)
            {
                m_StateMachine.StopCoroutine(m_CoroutineEnergy);
            }

            m_StateMachine.AddCurrState(EnumStatesPlayer.dead);

            return;
        }
        UpdateSlider();

        m_CoroutineArmor = m_StateMachine.StartCoroutine(CoroutineWaitForRegeneArmor());
    }

    public void TakeEnergy(float energy)
    {
        if(m_CoroutineEnergy != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineEnergy);
        }

        m_CurrEnergy -= energy;

        if(m_CurrEnergy < 0)
        {
            m_CurrEnergy = 0;
        }

        UpdateSlider();

        m_CoroutineEnergy = m_StateMachine.StartCoroutine(CoroutineWaitForRegeneEnergy());
    }
}
