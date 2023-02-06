using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManagerInitGame : State
{
    GameObject map;
    DataManager data;

    public StateManagerInitGame(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        data = (DataManager)m_StateMachine.GetData();
        map = Pool.m_Instance.GetObject(data.map);
        map.SetActive(true);
        m_StateMachine.StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(5);
        Pool.m_Instance.RemoveObject(map, data.map);
        m_StateMachine.StartCoroutine(test1());
    }

    IEnumerator test1()
    {
        yield return new WaitForSeconds(5);
        map = Pool.m_Instance.GetObject(data.map);
        map.SetActive(true);
    }
}
