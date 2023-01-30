using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMapData : StateData
{
    public EnumBlocks[,] m_Grid;
    public Vector2Int m_InitialPoint;
    public bool[,] m_DrawGrid;

    public StateMapData(StateMachine stateMachine) : base(stateMachine)
    {
    }

    // initialise les valeur des data de la stateMachine
    public void InitData()
    {
        m_InitialPoint = Vector2Int.zero;

        DataMap dataMap = (DataMap)m_StateMachine.GetData();
        m_Grid = new EnumBlocks[dataMap.nbChunkRight * dataMap.chunkWidth, dataMap.nbChunkDown * dataMap.chunkHeight];

        int drawGridX = m_Grid.GetLength(0) / dataMap.chunkViewSize;
        if(m_Grid.GetLength(0) % dataMap.chunkViewSize != 0)
        {
            drawGridX += 1;
        }

        int drawGridY = m_Grid.GetLength(1) / dataMap.chunkViewSize;
        if (m_Grid.GetLength(1) % dataMap.chunkViewSize != 0)
        {
            drawGridY += 1;
        }

        m_DrawGrid = new bool[drawGridX, drawGridY];
    }

    //Set le point initial de depart pour rendre la grid et le spawn du player
    public void FindInitialPoint(int offSet)
    {
        //int width = m_Grid.GetLength(0);
        //int height = m_Grid.GetLength(1);

        //// Recherche d'un point de spawn près du centre de la grille
        //for (int y = height / 2 - offSet; y < height / 2 + offSet; y++)
        //{
        //    if (y < 1 || y >= m_Grid.GetLength(1) - 1)
        //    {
        //        continue;
        //    }
        //    for (int x = width / 2 - offSet; x < width / 2 + offSet; x++)
        //    {
        //        if (x < 1 || x >= m_Grid.GetLength(0) - 1)
        //        {
        //            continue;
        //        }
        //        // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
        //        if (m_Grid[x, y] == EnumBlocks.backGroundEarth && m_Grid[x, y + 1] == EnumBlocks.backGroundEarth && m_Grid[x, y - 1] == EnumBlocks.earth)
        //        {
        //            m_InitialPoint = new Vector2(x, y);
        //            return;
        //        }
        //    }
        //}
        m_InitialPoint = new Vector2Int(m_Grid.GetLength(0) / 2, m_Grid.GetLength(0) / 2);
        //SetInitialPoint(offSet + 10);
    }
}
