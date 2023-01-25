using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectPool
    {
        public EnumObject type;
        public GameObject gameObject;
    }

    [System.Serializable]
    public struct DataPool
    {
        public EnumData type;
        public ScriptableObject data;
    }

    [SerializeField] private List<ObjectPool> m_ObjectsPoolList;
    [SerializeField] private List<DataPool> m_DataPoolList;
    [SerializeField] string m_ParentGameObjectName;

    private GameObject m_ParentGameObject;

    private Dictionary<EnumObject, GameObject> m_ObjectsPool;

    private Dictionary<EnumData, ScriptableObject> m_DataPool;

    private Dictionary<EnumObject, List<GameObject>> m_AvailablePool;

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
        InitObjectsPool();
        InitDataPool();
        InitAvailablePool();
    }

    private void InitAvailablePool()
    {
        if (m_AvailablePool == null)
        {
            m_AvailablePool = new Dictionary<EnumObject, List<GameObject>>();
        }
        m_AvailablePool.Clear();

        foreach (ObjectPool currObject in m_ObjectsPoolList)
        {
            m_AvailablePool.Add(currObject.type, new List<GameObject>());
        }
    }

    private void InitObjectsPool()
    {
        if(m_ObjectsPool == null)
        {
            m_ObjectsPool = new Dictionary<EnumObject, GameObject>();
        }
        m_ObjectsPool.Clear();

        foreach (ObjectPool currObject in m_ObjectsPoolList)
        {
            m_ObjectsPool.Add(currObject.type, currObject.gameObject);
        }
    }

    private void InitDataPool()
    {
        if (m_DataPool == null)
        {
            m_DataPool = new Dictionary<EnumData, ScriptableObject>();
        }
        m_DataPool.Clear();


        foreach (DataPool currObject in m_DataPoolList)
        {
            m_DataPool.Add(currObject.type, currObject.data);
        }
    }

    // retourn un game object disponible si il na pas n'initialise un nouveau
    public GameObject GetGameObject(EnumObject type)
    {
        GameObject currObject = null;
        List<GameObject> currListObject;

        m_AvailablePool.TryGetValue(type, out currListObject);
        
        if(currListObject.Count > 0)
        {
            currObject = currListObject[0];
            currListObject.RemoveAt(0);
        }
        else
        {
            currObject = InitGameObject(type);
        }

        return currObject;
    }

    // Enleve d'actif un game object et le met disponible
    public void RemoveGameObject(GameObject currObject, EnumObject type)
    {
        currObject.SetActive(false);

        if(m_ParentGameObject == null)
        {
            m_ParentGameObject = GameObject.Find(m_ParentGameObjectName);
        }

        currObject.transform.parent = m_ParentGameObject.transform;

        m_AvailablePool[type].Add(currObject);
    }

    // initialise un gameObject et le retourn
    private GameObject InitGameObject(EnumObject type)
    {
        GameObject newObject = Instantiate(m_ObjectsPool[type]);
        newObject.SetActive(false);

        return newObject;
    }

    public ScriptableObject GetDataResource(EnumData type)
    {
        return m_DataPool[type];
    }
}