using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombieAttack : State
{
    private DataZombie m_DataZombie;

    private StateZombieMovement m_StateZombieMovement;
    private StateZombieBrain m_StateZombieBrain;
    private DataStorageZombieStat m_DataStorageZombieStat;

    private Coroutine m_CoroutineAttack;

    private Animator m_Animator;

    private Coroutine m_CoroutineStopAttackPlayer;

    public StateZombieAttack(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_DataZombie = (DataZombie)m_StateMachine.GetData();

        m_StateZombieMovement = (StateZombieMovement)m_StateMachine.GetState(EnumStatesMonster.movement);
        m_StateZombieBrain = (StateZombieBrain)m_StateMachine.GetState(EnumStatesMonster.brain);

        m_DataStorageZombieStat = (DataStorageZombieStat)m_StateMachine.GetDataStorage(EnumStatesMonster.stat);

        m_Animator = m_StateMachine.GetComponent<Animator>();

        m_DataStorageZombieStat.EnableSlider();
        m_DataStorageZombieStat.InitStat();
    }

    public override void End()
    {
        m_DataStorageZombieStat.DisableSlider();
        if(m_CoroutineAttack != null)
        {
            m_Animator.SetBool("Attack", false);
            m_StateMachine.StopCoroutine(m_CoroutineAttack);
            m_CoroutineAttack = null;
        }
        if(m_CoroutineStopAttackPlayer != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineStopAttackPlayer);
            m_CoroutineStopAttackPlayer = null;
        }
    }

    public override void Update()
    {
        if (!m_StateZombieBrain.GetSeePlayer())
        {
            if(m_CoroutineStopAttackPlayer == null)
            {
                m_CoroutineStopAttackPlayer = m_StateMachine.StartCoroutine(CoroutineStopAttackPlayer());
            }
        }
        else
        {
            if(m_CoroutineStopAttackPlayer != null)
            {
                m_StateMachine.StopCoroutine(m_CoroutineStopAttackPlayer);
                m_CoroutineStopAttackPlayer = null;
            }
        }

        if (m_StateZombieBrain.GetIsDistanceToAttack())
        {
            m_StateZombieMovement.StopMoving();
            Attack();
        }
        else
        {
            m_StateZombieMovement.CheckIsPlayerChangedPos();
            if (m_StateZombieMovement.GetIsArrived())
            {
                m_StateZombieMovement.MoveToPlayer();
            }
        }
    }

    private void Attack()
    {
        if(m_CoroutineAttack == null)
        {
            m_CoroutineAttack = m_StateMachine.StartCoroutine(CoroutineAttack());
        }
    }

    private IEnumerator CoroutineAttack()
    {
        m_Animator.SetFloat("AttackSpeed", 1 / m_DataZombie.intervalAttack);

        m_Animator.SetBool("Attack", true);

        yield return new WaitForSeconds((1 * m_DataZombie.intervalAttack) / 2);

        Vector2 dir = ((Vector2)PlayerManager.m_Instance.GetCurrPlayerPos() - (Vector2)m_StateMachine.transform.position).normalized;
        
        Vector2 forward = Vector2.zero;
        forward.x = m_StateMachine.transform.localScale.x;
        float angle = Vector2.Angle(forward, dir);

        if (angle <= m_DataZombie.visionAngle)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll((Vector2)m_StateMachine.transform.position, dir, m_DataZombie.attackRange);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.CompareTag("Player"))
                {
                    PlayerManager.m_Instance.CurrPlayerTakeDamage(m_DataZombie.damage);
                    break;
                }
            }
        }

        yield return new WaitForSeconds((1 * m_DataZombie.intervalAttack) / 2);


        m_Animator.SetBool("Attack", false);

        m_CoroutineAttack = null;
    }

    private IEnumerator CoroutineStopAttackPlayer()
    {
        yield return new WaitForSeconds(m_DataZombie.delayStopAttackPlayer);
        m_StateMachine.PopCurrState(EnumStatesMonster.attack);
        m_StateMachine.AddCurrState(EnumStatesMonster.patrol);
        m_CoroutineStopAttackPlayer = null;
    }
}
