using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombieBrain : State
{
    private DataZombie m_DataZombie;

    private bool m_SeePlayer;
    private float m_DistanceToPlayer;

    public StateZombieBrain(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_StateMachine.gameObject.layer = LayerMask.NameToLayer("Enemie");
        m_StateMachine.gameObject.tag = "Enemie";

        m_SeePlayer = false;

        m_DataZombie = (DataZombie)m_StateMachine.GetData();

        m_StateMachine.StartCoroutine(WaitForStart(1));
    }

    public IEnumerator WaitForStart(float time)
    {
        yield return new WaitForSeconds(time);
        m_StateMachine.AddCurrState(EnumStatesMonster.patrol);
    }

    public override void Update()
    {
        LookPlayerForward();
    }

    private void LookPlayerForward()
    {
        Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();

        m_DistanceToPlayer = Vector2.Distance((Vector2)m_StateMachine.transform.position, (Vector2)playerPos);

        Vector2 dirToPlayer = ((Vector2)playerPos - (Vector2)m_StateMachine.transform.position).normalized;
        Vector2 forward = Vector2.zero;
        forward.x = m_StateMachine.transform.localScale.x;

        float angle = Vector2.Angle(forward, dirToPlayer);

        bool playerIsInRange = false;

        if (angle <= m_DataZombie.visionAngle)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll((Vector2)m_StateMachine.transform.position, dirToPlayer, m_DataZombie.visionDistance);
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.transform.CompareTag("Player"))
                {
                    playerIsInRange = true;
                }
            }
        }

        m_SeePlayer = playerIsInRange;
    }

    public bool GetSeePlayer()
    {
        return m_SeePlayer;
    }

    public bool GetIsDistanceToAttack()
    {
        if (m_DistanceToPlayer <= m_DataZombie.distanceToAttack)
        {
            return true;
        }
        return false;
    }
}
