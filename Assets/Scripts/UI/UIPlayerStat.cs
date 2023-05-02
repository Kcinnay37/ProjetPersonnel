using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerStat
{
    private UI m_UI;
    private Slider m_SliderHealth;
    private Slider m_SliderArmor;

    public UIPlayerStat(UI ui)
    {
        m_UI = ui;

        Transform playerStat = m_UI.transform.Find("UIScreen").Find("UIPlayerStat");

        m_SliderHealth = playerStat.Find("SliderHealth").GetComponent<Slider>();
        m_SliderArmor = playerStat.Find("SliderArmor").GetComponent<Slider>();

        m_SliderHealth.value = 1;
        m_SliderArmor.value = 1;
    }

    public void UpdateSlider(float health, float armor)
    {
        m_SliderHealth.value = health;
        m_SliderArmor.value = armor;
    }
}
