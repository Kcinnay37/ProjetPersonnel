using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

[CreateAssetMenu]
public class DataCollectible : ScriptableObject
{
    public EnumCollectibles instanceType;
    public int health;
    public Vector2 offsetSpawn;
    public Drops drop;
    public List<EnumTools> m_ToolsCanInteract;

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

[CustomEditor(typeof(DataCollectible))]
public class DataCollectibleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DataCollectible data = (DataCollectible)target;

        if (data.gridSpawn == null)
        {
            data.LoadData();
        }

        if (GUILayout.Button("Create Grid"))
        {
            data.CreateGrid();
            data.SaveData();
        }

        EditorGUI.BeginChangeCheck();

        if (data.gridSpawn != null)
        {
            for (int y = 0; y < data.gridSpawn.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < data.gridSpawn.GetLength(0); x++)
                {
                    data.gridSpawn[x, y] = EditorGUILayout.Toggle(data.gridSpawn[x, y]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            data.SaveData();
        }
    }
}