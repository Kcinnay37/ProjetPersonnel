using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateMachine m_StateMachine;

    public State(StateMachine stateMachine)
    {
        m_StateMachine = stateMachine;
    }

    // ordre dexecution -----------------------------------------------------------
    public virtual void OnInit() { }

    public virtual void FixedUpdate() { }

    public virtual void Update() { }

    public virtual void LateUpdate() { }

    public virtual void End() { }
    // --------------------------------------------------------------------------

    // collision ----------------------------------------------------------------
    public virtual void OnCollisionEnter2D(Collision2D collision) { }

    public virtual void OnCollisionStay2D(Collision2D collision) { }

    public virtual void OnCollisionExit2D(Collision2D collision) { }

    public virtual void OnTriggerEnter2D(Collider2D collision) { }

    public virtual void OnTriggerStay2D(Collider2D collision) { }

    public virtual void OnTriggerExit2D(Collider2D collision) { }
    // --------------------------------------------------------------------------
}
