using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMapManager : State
{
    private StateMapGenerate m_Generate;
    private StateMapView m_View;

    private DataMap m_DataMap;

    private bool m_IsGenerate;

    private EnumBlocks[,] m_Grid;
    private Vector2Int m_CurrPoint;

    public StateMapManager(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_Generate = (StateMapGenerate)m_StateMachine.GetStateData(EnumStatesMap.generate);
        m_View = (StateMapView)m_StateMachine.GetStateData(EnumStatesMap.view);
        m_DataMap = (DataMap)m_StateMachine.GetData();

        m_IsGenerate = false;

        m_CurrPoint = Vector2Int.zero;
        m_Grid = new EnumBlocks[m_DataMap.nbChunkRight * m_DataMap.chunkWidth, m_DataMap.nbChunkDown * m_DataMap.chunkHeight];

        GenerateMap();
    }

    public override void Update()
    {
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        //SetPoint(new Vector3Int((int)worldMousePos.x, (int)worldMousePos.y, 0));
    }



    private void GenerateMap()
    {
        //si nouvelle partie
        GenerateNewMap();

        m_View.ResetValue();
        m_View.StartUpdateValue();
        m_View.StartDraw();
        m_View.StartClear();
        m_IsGenerate = true;
    }

    private void GenerateNewMap()
    {
        m_Generate.GenerateMap();
        FindInitialPoint(50);
    }

    public bool IsGenerate()
    {
        return m_IsGenerate;
    }

    public EnumBlocks[,] GetGrid()
    {
        return m_Grid;
    }

    private void FindInitialPoint(int offSet)
    {
        int width = m_Grid.GetLength(0);
        int height = m_Grid.GetLength(1);

        // Recherche d'un point de spawn près du centre de la grille
        for (int y = height / 2 - offSet; y < height / 2 + offSet; y++)
        {
            if (y < 1 || y >= m_Grid.GetLength(1) - 1)
            {
                continue;
            }
            for (int x = width / 2 - offSet; x < width / 2 + offSet; x++)
            {
                if (x < 1 || x >= m_Grid.GetLength(0) - 1)
                {
                    continue;
                }
                // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
                if (m_Grid[x, y] == EnumBlocks.backGroundEarth && m_Grid[x, y + 1] == EnumBlocks.backGroundEarth && m_Grid[x, y - 1] == EnumBlocks.earth)
                {
                    m_CurrPoint = new Vector2Int(x, y);
                    return;
                }
            }
        }
    }

    public void SetPoint(Vector3 worldPos)
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_Grid[0, 0]);
        Vector3Int localPos = dataBlock.map.WorldToCell(worldPos);
        m_CurrPoint = new Vector2Int(localPos.x, localPos.y);
    }

    public Vector2Int GetPoint()
    {
        return m_CurrPoint;
    }

    public Vector3 GetPointToWorld()
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_Grid[0, 0]);
        Vector3 worldPos = dataBlock.map.CellToWorld(new Vector3Int(m_CurrPoint.x, m_CurrPoint.y, 0));
        return worldPos;
    }

}
