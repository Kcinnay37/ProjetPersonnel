using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataStorageZombieStat : DataStorage
{
    private DataZombie m_DataZombie;

    private float m_MaxHealth;
    private float m_CurrHealth;

    private Slider m_SliderHealth;

    public DataStorageZombieStat(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_DataZombie = (DataZombie)m_StateMachine.GetData();
        m_SliderHealth = m_StateMachine.transform.Find("Canvas").Find("SliderHealth").GetComponent<Slider>();
    }

    public override void End()
    {
        DisableSlider();
    }

    public void InitStat()
    {
        m_MaxHealth = m_DataZombie.maxHealth;
        m_CurrHealth = m_MaxHealth;

        UpdateSlider();
    }

    public void EnableSlider()
    {
        m_SliderHealth.gameObject.SetActive(true);
    }

    public void DisableSlider()
    {
        if(m_SliderHealth != null)
        {
            m_SliderHealth.gameObject.SetActive(false);
        }
    }

    private void UpdateSlider()
    {
        m_SliderHealth.value = m_CurrHealth / m_MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        m_CurrHealth -= damage;
        if(m_CurrHealth <= 0)
        {
            m_CurrHealth = 0;
            UpdateSlider();

            m_StateMachine.PopCurrState(EnumStatesMonster.patrol);
            m_StateMachine.PopCurrState(EnumStatesMonster.attack);
            m_StateMachine.PopCurrState(EnumStatesMonster.movement);
            m_StateMachine.PopCurrState(EnumStatesMonster.brain);
            m_StateMachine.PopCurrState(EnumStatesMonster.stat);
            m_StateMachine.AddCurrState(EnumStatesMonster.dead);
        }
        else
        {
            UpdateSlider();
            m_StateMachine.PopCurrState(EnumStatesMonster.patrol);
            m_StateMachine.AddCurrState(EnumStatesMonster.attack);
        }

    }
}
