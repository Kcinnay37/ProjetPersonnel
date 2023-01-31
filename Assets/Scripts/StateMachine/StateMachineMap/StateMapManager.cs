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
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        m_View.SetPosition(new Vector3Int((int)worldMousePos.x, (int)worldMousePos.y, 0));

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

        }
    }
    Vector3Int pos = new Vector3Int(100, 100, 0);
    private void GenerateMap()
    {
        

        //si nouvelle partie
        GenerateNewMap();

        m_View.ResetValue();
        m_View.ResetMapView();
        m_View.SetPosition(pos);
        m_View.StartUpdateValue();
        m_View.StartDraw();
        m_View.StartClear();
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
        //m_StateMachine.StartCoroutine(m_View.DrawAt(x, y));
        //m_StateMachine.StartCoroutine(m_View.DrawAt(x, y));
    }
}
