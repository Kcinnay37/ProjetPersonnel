using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerDead : State
{
    Animator m_Animator;

    DataPlayer m_DataPlayer;

    public StatePlayerDead(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_DataPlayer = (DataPlayer)m_StateMachine.GetData();

        AudioManager.m_Instance.PlaySoundAt(m_StateMachine.transform.position, m_DataPlayer.deadSound);

        m_StateMachine.PopCurrState(EnumStatesPlayer.controllerMount);
        m_StateMachine.PopCurrState(EnumStatesPlayer.controllerMovement);
        m_StateMachine.PopCurrState(EnumStatesPlayer.controllerInventory);

        m_Animator = m_StateMachine.GetComponent<Animator>();

        m_Animator.SetBool("Dead", true);

        GameObject.Find("UI").transform.Find("UIScreen").Find("ButtonReloadScene").gameObject.SetActive(true);
    }
}
