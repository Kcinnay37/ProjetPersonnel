using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorageManageMap : DataStorage
{
    private DataManager m_GlobalDataManager;

    private GameObject m_MapObject;
    private StateMachineMap m_StateMachineMap;
    private DataStorageMapGrid m_DataStorageMapGrid;

    public DataStorageManageMap(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        //Initialise la map a instancier
        m_GlobalDataManager = (DataManager)m_StateMachine.GetData();
        m_MapObject = Pool.m_Instance.GetObject(m_GlobalDataManager.map);
        m_MapObject.SetActive(true);

        m_StateMachineMap = m_MapObject.GetComponent<StateMachineMap>();
        m_DataStorageMapGrid = (DataStorageMapGrid)m_StateMachineMap.GetDataStorage(EnumStatesMap.grid);

        m_StateMachine.StartCoroutine(CheckMapLoad());
    }

    private IEnumerator CheckMapLoad()
    {
        while(true)
        {
            m_DataStorageMapGrid = (DataStorageMapGrid)m_StateMachineMap.GetDataStorage(EnumStatesMap.grid);
            if (m_DataStorageMapGrid != null && m_DataStorageMapGrid.IsGenerate())
            {
                m_StateMachine.AddCurrDataStorage(EnumStatesManager.manageUI);
                m_StateMachine.AddCurrDataStorage(EnumStatesManager.managePlayer);
                break;
            }
            yield return null;
        }
    }

    public void SetPoint(Vector3 worldPos)
    {
        m_DataStorageMapGrid.SetPoint(worldPos);
    }

    public Vector3 GetPointToWorld()
    {
        return m_DataStorageMapGrid.GetPointToWorld();
    }

    public bool PopBlockAt(Vector3 worldPos)
    {
        return m_DataStorageMapGrid.PopBlockAt(worldPos);
    }

    public bool AddBlockAt(Vector3 pos, EnumBlocks block)
    {
        return m_DataStorageMapGrid.AddBlockAt(pos, block);
    }
}
