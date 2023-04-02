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
        TextAsset jsonFile = Resources.Load<TextAsset>("SaveObject/" + name);

        if (jsonFile != null)
        {
            string json = jsonFile.text;
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

        string json = JsonConvert.SerializeObject(this.gridSpawn);

        // Créez le dossier "Resources/SaveObject" s'il n'existe pas
        string saveFolderPath = Path.Combine(Application.dataPath, "Resources", "SaveObject");
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        string saveFilePath = Path.Combine(saveFolderPath, name + ".json");
        File.WriteAllText(saveFilePath, json);
    }

    public void CreateGrid()
    {
        gridSpawn = new bool[1 + (gridSpawnSideAndHeightSize.x * 2), 1 + gridSpawnSideAndHeightSize.y];
    }
}
