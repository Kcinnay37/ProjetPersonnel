using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public class DataAddInWorld : ScriptableObject
{
    public EnumAddInWorld instanceType;
    public Vector2 offsetSpawn;
    public Vector2Int gridSpawnSideAndHeightSize;
    public bool[,] gridSpawn;

    public void LoadData()
    {
        string name = this.name;
        string saveFilePath = Path.Combine(Application.persistentDataPath, "SaveObject", name + ".json");

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            this.gridSpawn = JsonConvert.DeserializeObject<bool[,]>(json);
        }
        else if (this.gridSpawn == null)
        {
            CreateGrid();
            SaveData();
        }
    }

    public void SaveData()
    {
        string name = this.name;
        string saveFilePath = Application.persistentDataPath + "/SaveObject/" + name + ".json";

        string json = JsonConvert.SerializeObject(this.gridSpawn);
        File.WriteAllText(saveFilePath, json);
    }

    public void CreateGrid()
    {
        gridSpawn = new bool[1 + (gridSpawnSideAndHeightSize.x * 2), 1 + gridSpawnSideAndHeightSize.y];
    }
}
