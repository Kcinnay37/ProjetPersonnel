using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombieDead : State
{
    private DataZombie m_DataZombie;

    private Animator m_Animator;

    public StateZombieDead(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_StateMachine.gameObject.layer = LayerMask.NameToLayer("EnemieDead");
        m_StateMachine.gameObject.tag = "EnemieDead";

        m_DataZombie = (DataZombie)m_StateMachine.GetData();

        m_Animator = m_StateMachine.GetComponent<Animator>();
        m_Animator.SetBool("Dead", true);

        m_StateMachine.StartCoroutine(CoroutineWaitForDispawn());
    }

    IEnumerator CoroutineWaitForDispawn()
    {
        yield return new WaitForSeconds(m_DataZombie.waitForDispawn);
        ResourceManager.m_Instance.Drops(m_DataZombie.drop, m_StateMachine.transform.position + new Vector3(m_DataZombie.drop.offsetDrop.x, m_DataZombie.drop.offsetDrop.y, 0));
        MonsterManager.m_Instance.DispawnMonster(m_StateMachine.gameObject);
    }
}
