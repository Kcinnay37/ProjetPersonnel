using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            StateMachineMap map = FindObjectOfType<StateMachineMap>();
            DataStorageMapGrid grid = (DataStorageMapGrid)map.GetDataStorage(EnumStatesMap.grid);
            grid.GenerateMap();
        }
    }
}
