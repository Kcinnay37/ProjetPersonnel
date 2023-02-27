using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombieMovement : State
{
    private DataZombie m_GlobalDataMonster;

    private Vector2Int m_PointToGo;
    private List<Vector2Int> m_Path;

    private Rigidbody2D m_Rigidbody;

    public StateZombieMovement(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_GlobalDataMonster = (DataZombie)m_StateMachine.GetData();

        m_Rigidbody = m_StateMachine.GetComponent<Rigidbody2D>();
    }

    public override void Update()
    {
        
    }

    public void Move(int dir)
    {
        Vector2 currVelo = m_Rigidbody.velocity;
        currVelo.x = m_GlobalDataMonster.moveSpeed * dir;

        m_Rigidbody.velocity = currVelo;
    }

    public void Jump()
    {
        m_Rigidbody.AddForce(new Vector2(0, m_GlobalDataMonster.jumpForce));
    }
}
