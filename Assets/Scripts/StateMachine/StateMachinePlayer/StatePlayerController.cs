using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerController : State
{
    DataPlayer m_Data;
    Rigidbody2D m_RigidBody;
    Animator m_Animator;

    public StatePlayerController(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_Data = (DataPlayer)m_StateMachine.GetData();
        m_RigidBody = m_StateMachine.GetComponent<Rigidbody2D>();
        m_Animator = m_StateMachine.GetComponent<Animator>();
    }

    public override void Update()
    {
        UpdateMove();
        UpdateJump();
        UpdateAnimator();
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
            if (Input.GetKey(m_Data.runKey))
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
        if (m_RigidBody.velocity.x < 0)
        {
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = -1;
            m_StateMachine.transform.localScale = rota;
        }
        else if (m_RigidBody.velocity.x > 0)
        {
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = 1;
            m_StateMachine.transform.localScale = rota;
        }

        m_Animator.SetFloat("Velo", Mathf.Abs(m_RigidBody.velocity.x / m_Data.runSpeed));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(m_Data.jumpKeyCode) && CheckCanJump())
        {
            m_RigidBody.AddForce(new Vector2(0, m_Data.jumpForce * m_RigidBody.mass));
        }
    }

    private bool CheckCanJump()
    {
        return true;
    }
}
