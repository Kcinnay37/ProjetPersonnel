using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerJump : State
{
    Rigidbody2D m_RigidBody;
    DataStateMachinePlayer m_Data;

    public StatePlayerJump(StateMachine stateMachine) : base(stateMachine)
    {
        m_RigidBody = m_StateMachine.GetComponent<Rigidbody2D>();
        m_Data = (DataStateMachinePlayer)m_StateMachine.GetData();
    }

    public override void Update()
    {
        CheckJump();
    }

    private void CheckJump()
    {
        if(Input.GetKeyDown(m_Data.jumpKeyCode) && CheckCanJump())
        {
            m_RigidBody.AddForce(new Vector2(0, m_Data.jumpForce * m_RigidBody.mass));
        }
    }

    private bool CheckCanJump()
    {
        return true;
    }
}
