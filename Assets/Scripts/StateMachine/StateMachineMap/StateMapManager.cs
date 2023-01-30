using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMapManager : State
{
    StateMapData m_DataStateMachine;
    StateMapGenerate m_Generate;
    StateMapView m_View;
    DataMap m_DataMap;

    public StateMapManager(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_DataStateMachine = (StateMapData)m_StateMachine.GetStateData(EnumStatesMap.data);
        m_Generate = (StateMapGenerate)m_StateMachine.GetStateData(EnumStatesMap.generate);
        m_View = (StateMapView)m_StateMachine.GetStateData(EnumStatesMap.view);
        m_DataMap = (DataMap)m_StateMachine.GetData();

        GenerateMap();
    }

    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            pos.x += m_DataMap.chunkViewSize;
            RenderAt(pos);
        }
    }
    Vector2Int pos = new Vector2Int(30, 30);
    private void GenerateMap()
    {
        m_View.ResetMapView();

        //si nouvelle partie
        GenerateNewMap();

        RenderAt(pos);
    }

    private void GenerateNewMap()
    {
        m_DataStateMachine.InitData();
        m_Generate.GenerateMap();
        m_DataStateMachine.FindInitialPoint(50);
    }

    private void RenderAt(Vector2Int pos)
    {
        int x = pos.x / m_DataMap.chunkViewSize;
        int y = pos.y / m_DataMap.chunkViewSize;
        m_StateMachine.StartCoroutine(m_View.DrawAt(x, y));
    }
}
