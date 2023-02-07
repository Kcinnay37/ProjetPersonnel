//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class StateMapData : StateData
//{
//    public EnumBlocks[,] m_Grid;
//    public Vector2Int m_CurrPoint;

//    public StateMapData(StateMachine stateMachine) : base(stateMachine)
//    {

//    }

//    // initialise les valeur des data de la stateMachine
//    public override void OnInit()
//    {
//        m_CurrPoint = Vector2Int.zero;

//        DataMap dataMap = (DataMap)m_StateMachine.GetData();
//        m_Grid = new EnumBlocks[dataMap.nbChunkRight * dataMap.chunkWidth, dataMap.nbChunkDown * dataMap.chunkHeight];
//    }

//    public override void End()
//    {
//        m_Grid = null;
//        m_CurrPoint = Vector2Int.zero;
//    }

//    //Set le point initial de depart pour rendre la grid et le spawn du player
//    public void FindInitialPoint(int offSet)
//    {
//        int width = m_Grid.GetLength(0);
//        int height = m_Grid.GetLength(1);

//        // Recherche d'un point de spawn près du centre de la grille
//        for (int y = height / 2 - offSet; y < height / 2 + offSet; y++)
//        {
//            if (y < 1 || y >= m_Grid.GetLength(1) - 1)
//            {
//                continue;
//            }
//            for (int x = width / 2 - offSet; x < width / 2 + offSet; x++)
//            {
//                if (x < 1 || x >= m_Grid.GetLength(0) - 1)
//                {
//                    continue;
//                }
//                // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
//                if (m_Grid[x, y] == EnumBlocks.backGroundEarth && m_Grid[x, y + 1] == EnumBlocks.backGroundEarth && m_Grid[x, y - 1] == EnumBlocks.earth)
//                {
//                    m_CurrPoint = new Vector2Int(x, y);
//                    return;
//                }
//            }
//        }
//    }
//}
