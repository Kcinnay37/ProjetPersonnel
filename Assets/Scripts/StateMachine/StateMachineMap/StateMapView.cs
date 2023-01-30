using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMapView : StateData
{
    private StateMapData m_DataStateMachine;
    private DataMap m_DataMap;

    public StateMapView(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public void ResetMapView()
    {
        DataMap dataMap = (DataMap)m_StateMachine.GetData();

        //pour tout les chunk de la cave
        foreach (DataMap.Biome chunk in dataMap.mapBiomes)
        {
            //va chercher le data du chunk actuel
            DataBiome currDataChunk = (DataBiome)Pool.m_Instance.GetData(chunk.dataBiome);

            //pour tout les block du chunk
            foreach (DataBiome.Block block in currDataChunk.biomeBlocks)
            {
                //va chercher le data du block
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(block.block);
                //clear ca tilemap
                dataBlock.map.ClearAllTiles();
            }
        }
    }

    public System.Collections.IEnumerable Draw(int x, int y)
    {
        m_DataStateMachine = (StateMapData)m_StateMachine.GetStateData(EnumStatesMap.data);
        m_DataMap = (DataMap)m_StateMachine.GetData();

        for (int i = x - m_DataMap.nbChunkView; i <= x + m_DataMap.nbChunkView; i++)
        {
            if (i < 0 || i >= m_DataStateMachine.m_DrawGrid.GetLength(0))
            {
                continue;
            }
            for (int j = y - m_DataMap.nbChunkView; j <= y + m_DataMap.nbChunkView; j++)
            {
                if (j < 0 || j >= m_DataStateMachine.m_DrawGrid.GetLength(1))
                {
                    continue;
                }

                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_DataStateMachine.m_Grid[i, j]);
                dataBlock.map.SetTile(new Vector3Int(i, j, 0), dataBlock.tile);
            }

        }
        yield return null;
    }

    public IEnumerator DrawAt(int x, int y)
    {
        m_DataStateMachine = (StateMapData)m_StateMachine.GetStateData(EnumStatesMap.data);
        m_DataMap = (DataMap)m_StateMachine.GetData();

        for(int i = x - m_DataMap.nbChunkView + 1; i <= x + m_DataMap.nbChunkView + 1; i++)
        {
            if(i < 0 || i >= m_DataStateMachine.m_DrawGrid.GetLength(0))
            {
                continue;
            }
            for (int j = y - m_DataMap.nbChunkView + 1; j <= y + m_DataMap.nbChunkView + 1; j++)
            {
                if (j < 0 || j >= m_DataStateMachine.m_DrawGrid.GetLength(1))
                {
                    continue;
                }

                if (i == x - m_DataMap.nbChunkView + 1 || i == x + m_DataMap.nbChunkView + 1 || j == y - m_DataMap.nbChunkView + 1 || j == y + m_DataMap.nbChunkView + 1)
                {
                    if(m_DataStateMachine.m_DrawGrid[i, j])
                    {
                        m_StateMachine.StartCoroutine(ClearCase(i, j));
                        m_DataStateMachine.m_DrawGrid[i, j] = false;
                    }
                }
                else if (!m_DataStateMachine.m_DrawGrid[i, j])
                {
                    m_StateMachine.StartCoroutine(DrawCase(i, j));
                    m_DataStateMachine.m_DrawGrid[i, j] = true;
                }
                
            }

        }
        yield return null;

    }

    private IEnumerator ClearCase(int x, int y)
    {
        for (int i = x * m_DataMap.chunkViewSize; i < (x * m_DataMap.chunkViewSize) + m_DataMap.chunkViewSize; i++)
        {
            if (i < 0 || i >= m_DataStateMachine.m_Grid.GetLength(0))
            {
                continue;
            }
            for (int j = y * m_DataMap.chunkViewSize; j < (y * m_DataMap.chunkViewSize) + m_DataMap.chunkViewSize; j++)
            {
                if (j < 0 || j >= m_DataStateMachine.m_Grid.GetLength(1))
                {
                    continue;
                }
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_DataStateMachine.m_Grid[i, j]);
                dataBlock.map.SetTile(new Vector3Int(i, j, 0), null);
                yield return null;
            }

        }
    }

    private IEnumerator DrawCase(int x, int y)
    {
        for(int i = x * m_DataMap.chunkViewSize; i < (x * m_DataMap.chunkViewSize) + m_DataMap.chunkViewSize; i++)
        {
            if (i < 0 || i >= m_DataStateMachine.m_Grid.GetLength(0))
            {
                continue;
            }
            for (int j = y * m_DataMap.chunkViewSize; j < (y * m_DataMap.chunkViewSize) + m_DataMap.chunkViewSize; j++)
            {
                if (j < 0 || j >= m_DataStateMachine.m_Grid.GetLength(1))
                {
                    continue;
                }
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_DataStateMachine.m_Grid[i, j]);
                dataBlock.map.SetTile(new Vector3Int(i, j, 0), dataBlock.tile);

                yield return null;
            }
        }
    }

    public void DrawAllGrid()
    {
        m_DataStateMachine = (StateMapData)m_StateMachine.GetStateData(EnumStatesMap.data);
        m_DataMap = (DataMap)m_StateMachine.GetData();
        for (int x = 0; x < m_DataStateMachine.m_Grid.GetLength(0); x++)
        {
            for (int y = 0; y < m_DataStateMachine.m_Grid.GetLength(1); y++)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_DataStateMachine.m_Grid[x, y]);
                dataBlock.map.SetTile(new Vector3Int(x, y, 0), dataBlock.tile);
            }
        }
    }
}
