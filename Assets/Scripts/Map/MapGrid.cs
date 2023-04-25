using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    private Map m_Map;

    private EnumBlocks[,] m_GridBlock;
    private EnumBiomes[,] m_GridBiomes;
    private Dictionary<Vector2Int, EnumAddInWorld> m_ResourceInWorldPos;
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
        m_ResourceInWorldPos = new Dictionary<Vector2Int, EnumAddInWorld>();
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

    public Dictionary<Vector2Int, EnumAddInWorld> GetCollectible()
    {
        return m_ResourceInWorldPos;
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
        if ((Vector2)worldPos == Vector2.zero) return;

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

    public bool CheckCanAddBlockAt(Vector3 pos)
    {
        Dictionary<EnumBlocks, EnumBlocks> BackGroundDict = GetBackGroundDict();

        Vector2Int localPos = (Vector2Int)ConvertWorldToCell(pos);

        //regarde si cest sur le player
        Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();
        playerPos.y += 0.1f;
        Vector2 playerOffset = PlayerManager.m_Instance.GetPlayerData().offsetBlock;

        Vector3 cornerBotLeft = new Vector3(playerPos.x - playerOffset.x, playerPos.y, 0);
        Vector3 cornerBotRight = new Vector3(playerPos.x + playerOffset.x, playerPos.y, 0);
        Vector3 cornerTopLeft = new Vector3(playerPos.x - playerOffset.x, playerPos.y + playerOffset.y, 0);
        Vector3 cornerTopRight = new Vector3(playerPos.x + playerOffset.x, playerPos.y + playerOffset.y, 0);

        Vector2Int localBotLeft = (Vector2Int)ConvertWorldToCell(cornerBotLeft);
        Vector2Int localBotRight = (Vector2Int)ConvertWorldToCell(cornerBotRight);
        Vector2Int localTopLeft = (Vector2Int)ConvertWorldToCell(cornerTopLeft);
        Vector2Int localTopRight = (Vector2Int)ConvertWorldToCell(cornerTopRight);

        for(int x = localBotLeft.x; x <= localBotRight.x; x++)
        {
            for(int y = localBotLeft.y; y <= localTopLeft.y; y++)
            {
                Vector2Int currPos = new Vector2Int(x, y);
                if(localPos.Equals(currPos))
                {
                    return false;
                }
            }
        }

        //regarde si c'est sur un mobs
        Dictionary<GameObject, MonsterManager.MonsterSpawn> monsterEnable = MonsterManager.m_Instance.GetAllEnableMonster();
        foreach(KeyValuePair<GameObject, MonsterManager.MonsterSpawn> Value in monsterEnable)
        {
            Vector3 monsterPos = Value.Key.transform.position;
            monsterPos.y += 0.1f;
            DataZombie monsterData = (DataZombie)Value.Key.GetComponent<StateMachineZombie>().GetData();
            Vector2 monsterOffset = monsterData.offsetBlock;

            cornerBotLeft = new Vector3(monsterPos.x - monsterOffset.x, monsterPos.y, 0);
            cornerBotRight = new Vector3(monsterPos.x + monsterOffset.x, monsterPos.y, 0);
            cornerTopLeft = new Vector3(monsterPos.x - monsterOffset.x, monsterPos.y + monsterOffset.y, 0);
            cornerTopRight = new Vector3(monsterPos.x + monsterOffset.x, monsterPos.y + monsterOffset.y, 0);

            localBotLeft = (Vector2Int)ConvertWorldToCell(cornerBotLeft);
            localBotRight = (Vector2Int)ConvertWorldToCell(cornerBotRight);
            localTopLeft = (Vector2Int)ConvertWorldToCell(cornerTopLeft);
            localTopRight = (Vector2Int)ConvertWorldToCell(cornerTopRight);

            for (int x = localBotLeft.x; x <= localBotRight.x; x++)
            {
                for (int y = localBotLeft.y; y <= localTopLeft.y; y++)
                {
                    Vector2Int currPos = new Vector2Int(x, y);
                    if (localPos.Equals(currPos))
                    {
                        return false;
                    }
                }
            }
        }

        //regarde si c'est sous une ressource
        Dictionary<Vector2Int, GameObject> dictResourceEnable = AddInWorldManager.m_Instance.GetAllResourceEnable();
        foreach(KeyValuePair<Vector2Int, GameObject> value in dictResourceEnable)
        {
            EnumAddInWorld enumData = m_ResourceInWorldPos[value.Key];
            DataAddInWorld data = (DataAddInWorld)Pool.m_Instance.GetData(enumData);

            for(int x = 0; x <= data.gridSpawnSideAndHeightSize.x; x++)
            {
                for(int y = 0; y <= data.gridSpawnSideAndHeightSize.y; y++)
                {
                    Vector2Int posRight = new Vector2Int(value.Key.x + x, value.Key.y + y);
                    Vector2Int posLeft = new Vector2Int(value.Key.x - x, value.Key.y + y);

                    if(localPos.Equals(posRight))
                    {
                        if(data.gridSpawn[data.gridSpawnSideAndHeightSize.x + x, data.gridSpawnSideAndHeightSize.y - y])
                        {
                            return false;
                        }
                    }
                    else if(localPos.Equals(posLeft))
                    {
                        if (data.gridSpawn[data.gridSpawnSideAndHeightSize.x - x, data.gridSpawnSideAndHeightSize.y - y])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    public bool CheckCanPopBlockAt(Vector3 pos)
    {
        //regarde si il a une ressource en haut
        Vector2Int localPos = (Vector2Int)ConvertWorldToCell(pos);

        Dictionary<Vector2Int, GameObject> dictResourceEnable = AddInWorldManager.m_Instance.GetAllResourceEnable();
        foreach (KeyValuePair<Vector2Int, GameObject> value in dictResourceEnable)
        {
            EnumAddInWorld enumData = m_ResourceInWorldPos[value.Key];
            DataAddInWorld data = (DataAddInWorld)Pool.m_Instance.GetData(enumData);

            for (int x = 0; x <= data.gridSpawnSideAndHeightSize.x; x++)
            {
                Vector2Int posRight = new Vector2Int(value.Key.x + x, value.Key.y - 1);
                Vector2Int posLeft = new Vector2Int(value.Key.x - x, value.Key.y - 1);

                if (localPos.Equals(posRight))
                {
                    if (data.gridSpawn[data.gridSpawnSideAndHeightSize.x + x, data.gridSpawnSideAndHeightSize.y])
                    {
                        return false;
                    }
                }
                else if (localPos.Equals(posLeft))
                {
                    if (data.gridSpawn[data.gridSpawnSideAndHeightSize.x - x, data.gridSpawnSideAndHeightSize.y])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}




