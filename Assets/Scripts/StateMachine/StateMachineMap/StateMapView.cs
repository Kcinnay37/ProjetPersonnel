using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StateMapView : StateData
{
    private StateMapManager m_StateMapManager;
    private DataMap m_DataMap;

    private Coroutine m_CoroutineUpdateValue;
    private Coroutine m_CoroutineDraw;
    private Coroutine m_CoroutineClear;

    private Dictionary<Vector2Int, Vector2Int> m_CaseToDraw;
    private Dictionary<Vector2Int, Vector2Int> m_CaseToClear;
    private Dictionary<Vector2Int, Vector2Int> m_DrawCase;

    private bool[,] m_DrawGrid;

    public StateMapView(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {

    }

    public override void End()
    {
        if(m_CoroutineUpdateValue != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineUpdateValue);
            m_CoroutineUpdateValue = null;
        }
        if(m_CoroutineDraw != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineDraw);
            m_CoroutineDraw = null;
        }
        if(m_CoroutineClear != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineClear);
            m_CoroutineClear = null;
        }
    }

    public void ResetValue()
    {
        m_StateMapManager = (StateMapManager)m_StateMachine.GetState(EnumStatesMap.manager);
        m_DataMap = (DataMap)m_StateMachine.GetData();
        ResetMapView();

        m_CaseToDraw = new Dictionary<Vector2Int, Vector2Int>();
        m_CaseToClear = new Dictionary<Vector2Int, Vector2Int>();
        m_DrawCase = new Dictionary<Vector2Int, Vector2Int>();

        m_DrawGrid = new bool[m_StateMapManager.GetGrid().GetLength(0), m_StateMapManager.GetGrid().GetLength(1)];
    }

    private Vector2Int GetPosition()
    {
        return m_StateMapManager.GetPoint();
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
            m_CoroutineUpdateValue = m_StateMachine.StartCoroutine(CoroutineUpdateValue2());
        }
    }

    public void StartDraw()
    {
        if(m_CoroutineDraw == null)
        {
            m_CoroutineDraw = m_StateMachine.StartCoroutine(CoroutineDraw2());
        }
    }

    public void StartClear()
    {
        if (m_CoroutineClear == null)
        {
            m_CoroutineClear = m_StateMachine.StartCoroutine(CoroutineClear2());
        }
    }

    public IEnumerator CoroutineUpdateValue2()
    {
        while (true)
        {
            Vector2Int position = GetPosition();

            for (int i = position.x - m_DataMap.distanceView; i <= position.x + m_DataMap.distanceView; i++)
            {
                if (i < 0 || i >= m_DrawGrid.GetLength(0))
                {
                    continue;
                }
                for (int j = position.y - m_DataMap.distanceView; j <= position.y + m_DataMap.distanceView; j++)
                {
                    if (j < 0 || j >= m_DrawGrid.GetLength(1))
                    {
                        continue;
                    }
                    if (!m_DrawGrid[i, j])
                    {
                        if (!m_CaseToDraw.ContainsKey(new Vector2Int(i, j)))
                        {
                            m_CaseToDraw.Add(new Vector2Int(i, j), new Vector2Int(i, j));
                        }
                    }

                }
            }
            foreach (KeyValuePair<Vector2Int, Vector2Int> pos in m_DrawCase)
            {
                if (pos.Value.x < position.x - m_DataMap.distanceView || pos.Value.x > position.x + m_DataMap.distanceView || pos.Value.y < position.y - m_DataMap.distanceView || pos.Value.y > position.y + m_DataMap.distanceView)
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

    public IEnumerator CoroutineDraw2()
    {
        while (true)
        {
            foreach (KeyValuePair<Vector2Int, Vector2Int> pos in m_CaseToDraw)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_StateMapManager.GetGrid()[pos.Value.x, pos.Value.y]);
                dataBlock.map.SetTile(new Vector3Int(pos.Value.x, pos.Value.y, 0), dataBlock.tile);

                m_DrawGrid[pos.Value.x, pos.Value.y] = true;
                m_DrawCase.Add(pos.Key, pos.Value);
            }
            m_CaseToDraw.Clear();
            yield return null;
        }
    }

    public IEnumerator CoroutineClear2()
    {
        while (true)
        {
            foreach (KeyValuePair<Vector2Int, Vector2Int> pos in m_CaseToClear)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_StateMapManager.GetGrid()[pos.Value.x, pos.Value.y]);
                dataBlock.map.SetTile(new Vector3Int(pos.Value.x, pos.Value.y, 0), null);

                m_DrawGrid[pos.Value.x, pos.Value.y] = false;
                m_DrawCase.Remove(pos.Key);
            }
            m_CaseToClear.Clear();
            yield return null;
        }
    }

    public void DrawAllGrid()
    {
        for (int x = 0; x < m_StateMapManager.GetGrid().GetLength(0); x++)
        {
            for (int y = 0; y < m_StateMapManager.GetGrid().GetLength(1); y++)
            {
                DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_StateMapManager.GetGrid()[x, y]);
                dataBlock.map.SetTile(new Vector3Int(x, y, 0), dataBlock.tile);
            }
        }
    }
}
