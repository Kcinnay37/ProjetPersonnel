using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombieBrain : State
{
    public StateZombieBrain(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_StateMachine.StartCoroutine(WaitForStart(1));
    }

    public IEnumerator WaitForStart(float time)
    {
        yield return new WaitForSeconds(time);
        m_StateMachine.AddCurrState(EnumStatesMonster.patrol);
    }
}
