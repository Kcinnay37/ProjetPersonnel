using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    private Map m_Map;

    private EnumBlocks[,] m_GridBlock;
    private EnumBiomes[,] m_GridBiomes;
    private Dictionary<Vector2Int, EnumCollectibles> m_CollectiblePos;
    private Vector2Int m_CurrPoint;

    public MapGrid(Map map)
    {
        m_Map = map;
        m_CurrPoint = Vector2Int.zero;
    }


    public void InitValue()
    {
        m_GridBlock = new EnumBlocks[m_Map.GetData().nbChunkRight * m_Map.GetData().chunkWidth, m_Map.GetData().nbChunkDown * m_Map.GetData().chunkHeight];
        m_GridBiomes = new EnumBiomes[m_Map.GetData().nbChunkRight, m_Map.GetData().nbChunkDown];
        m_CollectiblePos = new Dictionary<Vector2Int, EnumCollectibles>();
    }

    public void InitInitialPoint()
    {
        FindInitialPoint(m_Map.GetData().offsetInitialPoint);
    }

    public EnumBlocks[,] GetGrid()
    {
        return m_GridBlock;
    }

    public DataBlock GetBlockAt(int x, int y)
    {
        DataBlock block = (DataBlock)Pool.m_Instance.GetData(m_GridBlock[x, y]);
        return block;
    }

    public Dictionary<Vector2Int, EnumCollectibles> GetCollectible()
    {
        return m_CollectiblePos;
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

    public EnumBiomes GetBiomeAtCurrPoint()
    {
        return m_GridBiomes[m_CurrPoint.x / m_Map.GetData().chunkWidth, m_CurrPoint.y / m_Map.GetData().chunkHeight];
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

    public Dictionary<EnumBlocks, EnumBlocks> GetBackGroundDict()
    {
        Dictionary<EnumBlocks, EnumBlocks> backGroundBlockDictionary = new Dictionary<EnumBlocks, EnumBlocks>();
        DataMap dataMap = Map.m_Instance.GetData();

        foreach (DataMap.Biome biome in dataMap.mapBiomes)
        {
            DataBiome dataBiome = (DataBiome)Pool.m_Instance.GetData(biome.dataBiome);

            if (!backGroundBlockDictionary.ContainsKey(dataBiome.biomeBlocks[dataBiome.biomeBlocks.Count - 1].block))
            {
                backGroundBlockDictionary.Add(dataBiome.biomeBlocks[dataBiome.biomeBlocks.Count - 1].block, dataBiome.biomeBlocks[dataBiome.biomeBlocks.Count - 1].block);
            }
        }
        return backGroundBlockDictionary;
    }

    public Vector3 GetPointToWorld()
    {
        DataBlock dataBlock = (DataBlock)Pool.m_Instance.GetData(m_GridBlock[0, 0]);
        Vector3 worldPos = dataBlock.map.CellToWorld(new Vector3Int(m_CurrPoint.x, m_CurrPoint.y, 0));
        return worldPos;
    }

    public void UpdatePoint()
    {
        Vector3 pos = PlayerManager.m_Instance.GetCurrPlayerPos();
        if (pos != Vector3.zero)
        {
            SetPoint(pos);
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

    public bool AddBlockAt(Vector3 pos, EnumBlocks block)
    {
        Vector3Int localPos = ConvertWorldToCell(pos);

        EnumBiomes currBiome = m_GridBiomes[localPos.x / m_Map.GetData().chunkWidth, localPos.y / m_Map.GetData().chunkHeight];
        DataBiome dataBiom = (DataBiome)Pool.m_Instance.GetData(currBiome);

        EnumBlocks blockBackground = dataBiom.biomeBlocks[dataBiom.biomeBlocks.Count - 1].block;
        EnumBlocks oldBlock = m_GridBlock[localPos.x, localPos.y];

        if (oldBlock != blockBackground)
        {
            return false;
        }

        m_GridBlock[localPos.x, localPos.y] = block;
        m_Map.GetView().UpdateCase(new Vector2Int(localPos.x, localPos.y), oldBlock);

        return true;
    }

    public bool PopBlockAt(Vector3 pos)
    {
        Vector3Int localPos = ConvertWorldToCell(pos);

        EnumBiomes currBiome = m_GridBiomes[localPos.x / m_Map.GetData().chunkWidth, localPos.y / m_Map.GetData().chunkHeight];
        DataBiome dataBiom = (DataBiome)Pool.m_Instance.GetData(currBiome);

        EnumBlocks oldBlock = m_GridBlock[localPos.x, localPos.y];
        EnumBlocks newBlock = dataBiom.biomeBlocks[dataBiom.biomeBlocks.Count - 1].block;

        if (oldBlock == newBlock)
        {
            return false;
        }

        m_GridBlock[localPos.x, localPos.y] = dataBiom.biomeBlocks[dataBiom.biomeBlocks.Count - 1].block;
        m_Map.GetView().UpdateCase(new Vector2Int(localPos.x, localPos.y), oldBlock);

        Vector3 posBlockInstance = pos;
        posBlockInstance.z = -2;
        ResourceManager.m_Instance.InstanciateResourceInWorldAt(oldBlock, posBlockInstance, Vector2.zero, 1);

        return true;
    }
}


