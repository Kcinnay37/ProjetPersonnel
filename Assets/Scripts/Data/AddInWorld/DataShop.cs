using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

[CreateAssetMenu]
public class DataShop : DataAddInWorld
{
    [System.Serializable]
    public struct ConsumableInShop
    {
        public EnumConsumables type;
        public Recipe recipe;
    }
    [System.Serializable]
    public struct EquipementInShop
    {
        public EnumEquipements type;
        public Recipe recipe;
    }
    [System.Serializable]
    public struct MaterialInShop
    {
        public EnumMaterial type;
        public Recipe recipe;
    }
    [System.Serializable]
    public struct ToolInShop
    {
        public EnumTools type;
        public Recipe recipe;
    }
    [System.Serializable]
    public struct MountInShop
    {
        public EnumMount type;
        public Recipe recipe;
    }
    [System.Serializable]
    public struct ConsumableInRecipe
    {
        public EnumConsumables type;
        public int nb;
    }
    [System.Serializable]
    public struct EquipementInRecipe
    {
        public EnumEquipements type;
        public int nb;
    }
    [System.Serializable]
    public struct MaterialInRecipe
    {
        public EnumMaterial type;
        public int nb;
    }
    [System.Serializable]
    public struct ToolInRecipe
    {
        public EnumTools type;
        public int nb;
    }
    [System.Serializable]
    public struct MountInRecipe
    {
        public EnumMount type;
        public int nb;
    }
    [System.Serializable]
    public struct BlockInRecipe
    {
        public EnumBlocks type;
        public int nb;
    }
    [System.Serializable]
    public struct ItemShop
    {
        public List<ConsumableInShop> consumableInShop;
        public List<EquipementInShop> equipementInShop;
        public List<MaterialInShop> materialInShop;
        public List<ToolInShop> toolInShop;
        public List<MountInShop> mountInShop;
    }
    [System.Serializable]
    public struct Recipe
    {
        public List<ConsumableInRecipe> consumableInRecipe;
        public List<EquipementInRecipe> equipementInRecipe;
        public List<MaterialInRecipe> materialInRecipe;
        public List<ToolInRecipe> toolInRecipe;
        public List<MountInRecipe> mountInRecipe;
        public List<BlockInRecipe> blockInRecipe;
    }

    public ItemShop itemShop;
    public int nbItem;
}

#if UNITY_EDITOR
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
#endif
