using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StateMapView : StateData
{
    private StateMapData m_DataStateMachine;
    private DataMap m_DataMap;

    private Coroutine m_CoroutineUpdateValue;
    private Coroutine m_CoroutineDraw;
    private Coroutine m_CoroutineClear;

    private Vector2Int m_Position;

    private Dictionary<Vector2Int, Vector2Int> m_CaseToDraw;
    private Dictionary<Vector2Int, Vector2Int> m_CaseToClear;
    private Dictionary<Vector2Int, Vector2Int> m_DrawCase;
    
    public StateMapView(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public void ResetValue()
    {
        m_DataStateMachine = (StateMapData)m_StateMachine.GetStateData(EnumStatesMap.data);
        m_DataMap = (DataMap)m_StateMachine.GetData();
        ResetMapView();

        m_CaseToDraw = new Dictionary<Vector2Int, Vector2Int>();
        m_CaseToClear = new Dictionary<Vector2Int, Vector2Int>();
        m_DrawCase = new Dictionary<Vector2Int, Vector2Int>();
}

    public void SetPosition(Vector3Int worldPos)
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_DataStateMachine.m_Grid[0, 0]);
        Vector3Int localPos = dataBlock.map.WorldToCell(worldPos);
        m_Position = new Vector2Int(localPos.x, localPos.y);
    }

    public void ResetMapView()
    {
        //pour tout les chunk de la cave
        foreach (DataMap.Biome chunk in m_DataMap.mapBiomes)
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

    public void StartUpdateValue()
    {
        if (m_CoroutineUpdateValue == null)
        {
            m_CoroutineUpdateValue = m_StateMachine.StartCoroutine(CoroutineUpdateValue());
        }
    }

    public void StartDraw()
    {
        if(m_CoroutineDraw == null)
        {
            m_CoroutineDraw = m_StateMachine.StartCoroutine(CoroutineDraw());
        }
    }

    public void StartClear()
    {
        if (m_CoroutineClear == null)
        {
            m_CoroutineClear = m_StateMachine.StartCoroutine(CoroutineClear());
        }
    }

    // regarde au tour du point et defini les case a effacer et dessiner 
    public IEnumerator CoroutineUpdateValue()
    {
        while(true)
        {
            for (int i = m_Position.x - m_DataMap.nbChunkView; i <= m_Position.x + m_DataMap.nbChunkView ; i++)
            {
                if (i < 0 || i >= m_DataStateMachine.m_DrawGrid.GetLength(0))
                {
                    continue;
                }
                for (int j = m_Position.y - m_DataMap.nbChunkView; j <= m_Position.y + m_DataMap.nbChunkView; j++)
                {
                    if (j < 0 || j >= m_DataStateMachine.m_DrawGrid.GetLength(1))
                    {
                        continue;
                    }
                    if (!m_DataStateMachine.m_DrawGrid[i, j])
                    {
                        if(!m_CaseToDraw.ContainsKey(new Vector2Int(i, j)))
                        {
                            m_CaseToDraw.Add(new Vector2Int(i, j), new Vector2Int(i, j));
                        }
                    }

                }
            }
            foreach(KeyValuePair<Vector2Int, Vector2Int> pos in m_DrawCase)
            {
                if(pos.Value.x < m_Position.x - m_DataMap.nbChunkView || pos.Value.x > m_Position.x + m_DataMap.nbChunkView || pos.Value.y < m_Position.y - m_DataMap.nbChunkView || pos.Value.y > m_Position.y + m_DataMap.nbChunkView)
                {
                    if (!m_CaseToClear.ContainsKey(pos.Value))
                    {
                        m_CaseToClear.Add(pos.Key, pos.Value);
                    }
                }
            }

            yield return new WaitForSeconds(m_DataMap.timeUpdateView);
        }
    }

    public IEnumerator CoroutineDraw()
    {
        while (true)
        {
            foreach (KeyValuePair<Vector2Int, Vector2Int> pos in m_CaseToDraw)
            {
                m_StateMachine.StartCoroutine(DrawCase(pos.Value.x, pos.Value.y));
                m_DataStateMachine.m_DrawGrid[pos.Value.x, pos.Value.y] = true;
            }
            m_CaseToDraw.Clear();
            yield return null;
        }
    }

    public IEnumerator CoroutineClear()
    {
        while (true)
        {
            foreach (KeyValuePair<Vector2Int, Vector2Int> pos in m_CaseToClear)
            {
                m_StateMachine.StartCoroutine(ClearCase(pos.Value.x, pos.Value.y));
            }
            m_CaseToClear.Clear();
            yield return null;
        }
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
        m_DataStateMachine.m_DrawGrid[x, y] = false;
        m_DrawCase.Remove(new Vector2Int(x, y));
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
        m_DrawCase.Add(new Vector2Int(x, y), new Vector2Int(x, y));
    }

    //public void DrawAllGrid()
    //{
    //    m_DataStateMachine = (StateMapData)m_StateMachine.GetStateData(EnumStatesMap.data);
    //    m_DataMap = (DataMap)m_StateMachine.GetData();
    //    for (int x = 0; x < m_DataStateMachine.m_Grid.GetLength(0); x++)
    //    {
    //        for (int y = 0; y < m_DataStateMachine.m_Grid.GetLength(1); y++)
    //        {
    //            DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_DataStateMachine.m_Grid[x, y]);
    //            dataBlock.map.SetTile(new Vector3Int(x, y, 0), dataBlock.tile);
    //        }
    //    }
    //}
}
