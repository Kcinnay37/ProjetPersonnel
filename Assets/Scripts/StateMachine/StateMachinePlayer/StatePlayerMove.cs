using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerMove : State
{
    DataPlayer m_Data;
    Rigidbody2D m_RigidBody;
    Animator m_Animator;

    public StatePlayerMove(StateMachine stateMachine) : base(stateMachine)
    {
        m_Data = (DataPlayer)m_StateMachine.GetData();
        m_RigidBody = m_StateMachine.GetComponent<Rigidbody2D>();
        m_Animator = m_StateMachine.GetComponent<Animator>();
    }

    public override void Update()
    {
        UpdateMove();
        UpdateAnimator();

        // si il bouge plus enleve la state moving
        //if (m_RigidBody.velocity.x == 0)
        //{
        //    m_StateMachine.PopCurrState(StateMachine.states.moving);
        //}
    }
    
    private void UpdateMove()
    {
        Vector2 velo = m_RigidBody.velocity;

        //mets la velo a 0 si les deux touche opposé sont enfoncé
        if (Input.GetKey(m_Data.leftKey) && Input.GetKey(m_Data.rightKey))
        {
            velo.x = 0;
        }
        else
        {
            // si il cour ou pas
            if(Input.GetKey(m_Data.runKey))
            {
                velo.x = Input.GetAxis("Horizontal") * m_Data.runSpeed;
            }
            else
            {
                velo.x = Input.GetAxis("Horizontal") * m_Data.walkSpeed;
            }
        }

        m_RigidBody.velocity = velo;
    }

    private void UpdateAnimator()
    {
        //rotationne le player dans la bonne direction
        if(m_RigidBody.velocity.x < 0)
        {
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = -1;
            m_StateMachine.transform.localScale = rota;
        }
        else if(m_RigidBody.velocity.x > 0)
        {
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = 1;
            m_StateMachine.transform.localScale = rota;
        }

        m_Animator.SetFloat("Velo", Mathf.Abs(m_RigidBody.velocity.x / m_Data.runSpeed));
    }
}
