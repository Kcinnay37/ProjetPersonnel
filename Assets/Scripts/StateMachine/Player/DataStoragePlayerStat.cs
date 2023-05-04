using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoragePlayerStat : DataStorage
{
    DataPlayer m_GlobalDataPlayer;

    private int m_SizeEquip;

    private float m_MaxHealth;
    private float m_CurrHealth;
    private float m_HealthRegene;
    private float m_WaitForRegeneHealth;
    private Coroutine m_CoroutineHealth;

    private float m_MaxArmor;
    private float m_CurrArmor;
    private float m_ArmorRegene;
    private float m_WaitForRegeneArmor;
    private Coroutine m_CoroutineArmor;

    public DataStoragePlayerStat(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        InitStats();
    }

    public override void End()
    {
        if(m_CoroutineArmor != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineArmor);
            m_CoroutineArmor = null;
        }

        if (m_CoroutineHealth != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineHealth);
            m_CoroutineHealth = null;
        }
    }

    public void InitStats()
    {
        m_SizeEquip = m_GlobalDataPlayer.baseSizeEquip;

        m_MaxHealth = m_GlobalDataPlayer.baseMaxHealth;
        m_CurrHealth = m_GlobalDataPlayer.baseMaxHealth;

        m_MaxArmor = m_GlobalDataPlayer.baseMaxArmor;
        m_CurrArmor = m_GlobalDataPlayer.baseMaxArmor;

        m_ArmorRegene = m_GlobalDataPlayer.regeneArmor;
        m_WaitForRegeneArmor = m_GlobalDataPlayer.waitForRegeneArmor;

        m_HealthRegene = m_GlobalDataPlayer.regeneHealth;
        m_WaitForRegeneHealth = m_GlobalDataPlayer.waitForRegeneHealth;

        UpdateSlider();
    }

    public void ResetMaxStats()
    {
        m_MaxHealth = m_GlobalDataPlayer.baseMaxHealth;
        m_HealthRegene = m_GlobalDataPlayer.regeneHealth;
        m_MaxArmor = m_GlobalDataPlayer.baseMaxArmor;
        m_ArmorRegene = m_GlobalDataPlayer.regeneArmor;
    }

    public void CheckStat()
    {
        if (m_CurrHealth > m_MaxHealth)
        {
            m_CurrHealth = m_MaxHealth;
        }
        if (m_CurrArmor > m_MaxArmor)
        {
            m_CurrArmor = m_MaxArmor;
        }

        UpdateSlider();
    }

    public void AddStats(DataEquipement.BonusStat stat)
    {
        m_MaxHealth += stat.health;
        m_HealthRegene += stat.healthRegene;
        m_MaxArmor += stat.armor;
        m_ArmorRegene += stat.armorRegene;

        if(m_CoroutineArmor == null)
        {
            m_CoroutineArmor = m_StateMachine.StartCoroutine(CoroutineWaitForRegeneArmor());
        }

        if(m_CoroutineHealth == null)
        {
            m_CoroutineHealth = m_StateMachine.StartCoroutine(CoroutineWaitForRegeneHealth());
        }
    }

    public int GetSizeInventoryEquip()
    {
        return m_SizeEquip;
    }

    public void UpdateSlider()
    {
        UI.m_Instance.GetUIPlayerStat().UpdateSlider((float)m_CurrHealth / (float)m_MaxHealth, (float)m_CurrArmor / (float)m_MaxArmor);
    }

    private IEnumerator CoroutineWaitForRegeneArmor()
    {
        yield return new WaitForSeconds(m_WaitForRegeneArmor);
        m_CoroutineArmor = m_StateMachine.StartCoroutine(CoroutineRegeneArmor());
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

    private IEnumerator CoroutineWaitForRegeneHealth()
    {
        yield return new WaitForSeconds(m_WaitForRegeneHealth);
        m_CoroutineHealth = m_StateMachine.StartCoroutine(CoroutineRegeneHealth());
    }


    private IEnumerator CoroutineRegeneHealth()
    {
        while (true)
        {
            yield return null;
            m_CurrHealth += m_HealthRegene * Time.deltaTime;
            if (m_CurrHealth >= m_MaxHealth)
            {
                m_CurrHealth = m_MaxHealth;
                UpdateSlider();
                break;
            }
            UpdateSlider();
        }
        m_CoroutineHealth = null;
    }

    public void TakeDamage(float damage)
    {
        if(m_CoroutineArmor != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineArmor);
            m_CoroutineArmor = null;
        }

        m_CurrArmor -= damage;
        if(m_CurrArmor < 0)
        {
            if(m_CoroutineHealth != null)
            {
                m_StateMachine.StopCoroutine(m_CoroutineHealth);
                m_CoroutineHealth = null;
            }

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

            m_StateMachine.AddCurrState(EnumStatesPlayer.dead);

            if(m_StateMachine.GetState(EnumStatesPlayer.dead) == null)
            {
                AudioManager.m_Instance.PlaySoundAt(m_StateMachine.transform.position, EnumAudios.damageDead);
            }
            return;
        }
        AudioManager.m_Instance.PlaySoundAt(m_StateMachine.transform.position, EnumAudios.damageHit);
        UpdateSlider();

        m_CoroutineArmor = m_StateMachine.StartCoroutine(CoroutineWaitForRegeneArmor());

        if(m_CoroutineHealth == null)
        {
            m_CoroutineHealth = m_StateMachine.StartCoroutine(CoroutineWaitForRegeneHealth());
        }
    }
}
