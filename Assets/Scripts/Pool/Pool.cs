using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [System.Serializable]
    public struct StateMachine
    {
        public EnumStateMachines type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Biome
    {
        public EnumBiomes type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Block
    {
        public EnumBlocks type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Tool
    {
        public EnumTools type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct UI
    {
        public EnumUI type;
        public UnityEngine.Object value;
    }

    [Header("StateMachine")]
    [SerializeField] List<StateMachine> m_InstanceStateMachines;
    [SerializeField] List<StateMachine> m_DataStateMachines;

    [Header("Biomes")]
    [SerializeField] List<Biome> m_InstanceBiomes;
    [SerializeField] List<Biome> m_DataBiomes;

    [Header("Blocks")]
    [SerializeField] List<Block> m_InstanceBlocks;
    [SerializeField] List<Block> m_DataBlocks;

    [Header("Outils")]
    [SerializeField] List<Tool> m_InstanceTools;
    [SerializeField] List<Tool> m_DataTools;

    [Header("UI")]
    [SerializeField] List<UI> m_InstanceUI;
    [SerializeField] List<UI> m_DataUI;

    [Header("ParentObjectInHierarchy")]
    [SerializeField] string m_ParentGameObjectName;

    private GameObject m_ParentGameObject;

    private Dictionary<object, GameObject> m_InstancePool;
    private Dictionary<object, ScriptableObject> m_DataPool;
    private Dictionary<object, List<GameObject>> m_AvailableInstancePool;

    private static Pool m_Pool;

    public static Pool m_Instance
    {
        get 
        { 
            if(m_Pool == null)
            {
                m_Pool = FindObjectOfType<Pool>();
                m_Pool.Init();
            }
            return m_Pool;
        }
    }

    private void Init()
    {
        InitInstancePool();
        InitDataPool();
        InitAvailableInstancePool();
    }

    private void InitAvailableInstancePool()
    {
        // init dictionary ---------------------------
        if (m_AvailableInstancePool == null)
        {
            m_AvailableInstancePool = new Dictionary<object, List<GameObject>>();
        }
        m_AvailableInstancePool.Clear();
        // -------------------------------------------

        // init list in dictonary -------------------------------------
        foreach (StateMachine value in m_InstanceStateMachines)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Biome value in m_InstanceBiomes)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Block value in m_InstanceBlocks)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Tool value in m_InstanceTools)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (UI value in m_InstanceUI)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }
        // ------------------------------------------------------------
    }

    private void InitInstancePool()
    {
        if(m_InstancePool == null)
        {
            m_InstancePool = new Dictionary<object, GameObject>();
        }
        m_InstancePool.Clear();

        // init les object -------------------------------------
        foreach (StateMachine value in m_InstanceStateMachines)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Biome value in m_InstanceBiomes)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Block value in m_InstanceBlocks)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Tool value in m_InstanceTools)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (UI value in m_InstanceUI)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }
        // ------------------------------------------------------------
    }

    private void InitDataPool()
    {
        if (m_DataPool == null)
        {
            m_DataPool = new Dictionary<object, ScriptableObject>();
        }
        m_DataPool.Clear();

        foreach (StateMachine value in m_DataStateMachines)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Biome value in m_DataBiomes)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Block value in m_DataBlocks)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Tool value in m_DataTools)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (UI value in m_DataUI)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }
    }

    // retourn un object disponible si il na pas n'initialise un nouveau
    public GameObject GetObject(object type)
    {
        GameObject currObject = null;
        List<GameObject> currListObject;

        m_AvailableInstancePool.TryGetValue(type, out currListObject);
        
        if(currListObject.Count > 0)
        {
            currObject = currListObject[0];
            currListObject.RemoveAt(0);
        }
        else
        {
            currObject = InitObject(type);
        }

        return currObject;
    }

    // Enleve d'actif un game object et le met disponible
    public void RemoveObject(GameObject currObject, object type)
    {
        currObject.SetActive(false);

        if(m_ParentGameObject == null)
        {
            m_ParentGameObject = GameObject.Find(m_ParentGameObjectName);
        }

        currObject.transform.parent = m_ParentGameObject.transform;

        m_AvailableInstancePool[type].Add(currObject);
    }

    // initialise un gameObject et le retourn
    private GameObject InitObject(object type)
    {
        GameObject newObject = Instantiate(m_InstancePool[type]);
        newObject.SetActive(false);

        return newObject;
    }

    public ScriptableObject GetData(object type)
    {
        return m_DataPool[type];
    }
}