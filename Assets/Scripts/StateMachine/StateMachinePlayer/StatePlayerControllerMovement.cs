using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerControllerMovement : State
{
    private DataPlayer m_GlobalDataPlayer;
    private Rigidbody2D m_RigidBody;
    private Animator m_Animator;

    private int m_PlayerDir = 1;

    public StatePlayerControllerMovement(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();
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
        if (Input.GetKey(m_GlobalDataPlayer.leftKey) && Input.GetKey(m_GlobalDataPlayer.rightKey))
        {
            velo.x = 0;
        }
        else
        {
            // si il cour ou pas
            if (Input.GetKey(m_GlobalDataPlayer.runKey))
            {
                velo.x = Input.GetAxis("Horizontal") * m_GlobalDataPlayer.baseRunSpeed;
            }
            else
            {
                velo.x = Input.GetAxis("Horizontal") * m_GlobalDataPlayer.baseWalkSpeed;
            }
        }

        m_RigidBody.velocity = velo;
    }

    private void UpdateAnimator()
    {
        //rotationne le player dans la bonne direction
        if (m_RigidBody.velocity.x < 0)
        {
            m_PlayerDir = -1;
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = m_PlayerDir;
            m_StateMachine.transform.localScale = rota;
        }
        else if (m_RigidBody.velocity.x > 0)
        {
            m_PlayerDir = 1;
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = m_PlayerDir;
            m_StateMachine.transform.localScale = rota;
        }

        m_Animator.SetFloat("Velo", Mathf.Abs(m_RigidBody.velocity.x / m_GlobalDataPlayer.baseRunSpeed));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(m_GlobalDataPlayer.jumpKeyCode) && CheckCanJump())
        {
            m_RigidBody.AddForce(new Vector2(0, m_GlobalDataPlayer.baseJumpForce * m_RigidBody.mass));
        }
    }

    private bool CheckCanJump()
    {
        return true;
    }

    public int GetPlayerDir()
    {
        return m_PlayerDir;
    }
}
