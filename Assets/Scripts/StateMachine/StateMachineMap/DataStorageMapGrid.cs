using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorageMapGrid : DataStorage
{
    private DataStorageMapGenerate m_dataStorageMapGenerate;
    private DataStorageMapView m_dataStorageMapView;

    private DataMap m_GlobalDataMap;

    private bool m_IsGenerate;

    private EnumBlocks[,] m_GridBlock;
    private EnumBiomes[,] m_GridBiomes;
    private Vector2Int m_CurrPoint;

    public DataStorageMapGrid(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        m_dataStorageMapGenerate = (DataStorageMapGenerate)m_StateMachine.GetDataStorage(EnumStatesMap.generate);
        m_dataStorageMapView = (DataStorageMapView)m_StateMachine.GetDataStorage(EnumStatesMap.view);
        m_GlobalDataMap = (DataMap)m_StateMachine.GetData();

        m_IsGenerate = false;

        m_CurrPoint = Vector2Int.zero;
        m_GridBlock = new EnumBlocks[m_GlobalDataMap.nbChunkRight * m_GlobalDataMap.chunkWidth, m_GlobalDataMap.nbChunkDown * m_GlobalDataMap.chunkHeight];
        m_GridBiomes = new EnumBiomes[m_GlobalDataMap.nbChunkRight, m_GlobalDataMap.nbChunkDown];

        GenerateMap();
    }


    private void GenerateMap()
    {
        //si nouvelle partie
        m_dataStorageMapGenerate.GenerateMap();
        FindInitialPoint(m_GlobalDataMap.offsetInitialPoint);

        m_dataStorageMapView.ResetValue();
        m_dataStorageMapView.StartUpdateValue();
        m_dataStorageMapView.StartDraw();
        m_dataStorageMapView.StartClear();
        m_IsGenerate = true;
    }


    public bool IsGenerate()
    {
        return m_IsGenerate;
    }

    public EnumBlocks[,] GetGrid()
    {
        return m_GridBlock;
    }

    private void FindInitialPoint(int offSet)
    {
        int width = m_GridBlock.GetLength(0);
        int height = m_GridBlock.GetLength(1);

        // Recherche d'un point de spawn près du centre de la grille
        for (int y = height / 2 - offSet; y < height / 2 + offSet; y++)
        {
            if (y < 1 || y >= m_GridBlock.GetLength(1) - 1)
            {
                continue;
            }
            for (int x = width / 2 - offSet; x < width / 2 + offSet; x++)
            {
                if (x < 1 || x >= m_GridBlock.GetLength(0) - 1)
                {
                    continue;
                }
                // Vérification de la hauteur du point de spawn et de la présence de sol en dessous
                if (m_GridBlock[x, y] == EnumBlocks.backGroundEarth && m_GridBlock[x, y + 1] == EnumBlocks.backGroundEarth && m_GridBlock[x, y - 1] == EnumBlocks.earth)
                {
                    m_CurrPoint = new Vector2Int(x, y);
                    return;
                }
            }
        }
    }

    public void SetPoint(Vector3 worldPos)
    {
        if (worldPos == Vector3.zero) return;

        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_GridBlock[0, 0]);
        Vector3Int localPos = dataBlock.map.WorldToCell(worldPos);
        m_CurrPoint = new Vector2Int(localPos.x, localPos.y);
    }

    public Vector2Int GetPoint()
    {
        return m_CurrPoint;
    }

    public Vector3 GetPointToWorld()
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_GridBlock[0, 0]);
        Vector3 worldPos = dataBlock.map.CellToWorld(new Vector3Int(m_CurrPoint.x, m_CurrPoint.y, 0));
        return worldPos;
    }

    public void UpdatePoint()
    {
        DataStorageManagePlayer dataStorageManagePlayer = (DataStorageManagePlayer)StateMachineManager.m_Instance.GetDataStorage(EnumStatesManager.managePlayer);

        if(dataStorageManagePlayer != null)
        {
            SetPoint(dataStorageManagePlayer.GetPlayerPos());
        }
    }

    public Vector3Int ConvertWorldToCell(Vector3 worldPos)
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_GridBlock[0, 0]);
        Vector3Int localPos = dataBlock.map.WorldToCell(worldPos);
        return localPos;
    }

    public Vector3 ConvertCellToWorld(Vector2Int cellPos)
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_GridBlock[0, 0]);
        Vector3 worldPos = dataBlock.map.CellToWorld(new Vector3Int(cellPos.x, cellPos.y, 0));
        return worldPos;
    }

    public void SetGridBiomeAt(Vector2Int pos, EnumBiomes biome)
    {
        m_GridBiomes[pos.x, pos.y] = biome;
    }

    public void AddBlockAt(Vector3 pos, EnumBlocks block)
    {
        //m_View.UpdateCase(pos);
    }

    public void PopBlockAt(Vector3 pos)
    {
        Vector3Int localPos = ConvertWorldToCell(pos);

        EnumBlocks oldBlock = m_GridBlock[localPos.x, localPos.y];
        m_GridBlock[localPos.x, localPos.y] = EnumBlocks.backGroundEarth;

        m_dataStorageMapView.UpdateCase(new Vector2Int(localPos.x, localPos.y), oldBlock);
    }
}
