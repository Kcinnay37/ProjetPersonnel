using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

[CreateAssetMenu]
public class DataShop : DataAddInWorld
{
    public struct ItemShop
    {

    }

    public struct Recipe
    {

    }

    public struct ConsumableInShop
    {
        public EnumConsumables type;
        public int nb;
        public Recipe recipe;
    }

    public struct EquipementInShop
    {
        public EnumEquipements type;
        public int nb;
        public Recipe recipe;
    }

    public struct MaterialInShop
    {
        public EnumMaterial type;
        public int nb;
        public Recipe recipe;
    }

    public struct ToolInShop
    {
        public EnumTools type;
        public int nb;
        public Recipe recipe;
    }
}

[CustomEditor(typeof(DataShop))]
public class DataShopEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DataShop data = (DataShop)target;

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
