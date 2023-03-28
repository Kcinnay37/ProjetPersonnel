using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

[CreateAssetMenu]
public class DataCollectible : DataAddInWorld
{
    public int health;
    public Drops drop;
    public List<EnumTools> m_ToolsCanInteract;
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