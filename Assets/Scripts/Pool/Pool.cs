using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [System.Serializable]
    public struct Manager
    {
        public EnumManagers type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Map
    {
        public EnumMaps type;
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
    public struct Player
    {
        public EnumPlayers type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct NPC
    {
        public EnumNPCs type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Monster
    {
        public EnumMonster type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct UI
    {
        public EnumUI type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Tool
    {
        public EnumTools type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Consumable
    {
        public EnumConsumables type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Equipement
    {
        public EnumEquipements type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Collectible
    {
        public EnumCollectibles type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Material
    {
        public EnumMaterial type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct SpecialResource
    {
        public EnumSpecialResources type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct VFX
    {
        public EnumVFXs type;
        public UnityEngine.Object value;
    }

    [System.Serializable]
    public struct Audio
    {
        public EnumAudios type;
        public UnityEngine.Object value;
    }

    [Header("Manager")]
    [SerializeField] List<Manager> m_InstanceManager;
    [SerializeField] List<Manager> m_DataManager;

    [Header("Map")]
    [SerializeField] List<Map> m_InstanceMap;
    [SerializeField] List<Map> m_DataMap;

    [Header("Biome")]
    [SerializeField] List<Biome> m_InstanceBiome;
    [SerializeField] List<Biome> m_DataBiome;

    [Header("Block")]
    [SerializeField] List<Block> m_InstanceBlock;
    [SerializeField] List<Block> m_DataBlock;

    [Header("Player")]
    [SerializeField] List<Player> m_InstancePlayer;
    [SerializeField] List<Player> m_DataPlayer;

    [Header("NPC")]
    [SerializeField] List<NPC> m_InstanceNPC;
    [SerializeField] List<NPC> m_DataNPC;

    [Header("Monster")]
    [SerializeField] List<Monster> m_InstanceMonster;
    [SerializeField] List<Monster> m_DataMonster;

    [Header("UI")]
    [SerializeField] List<UI> m_InstanceUI;
    [SerializeField] List<UI> m_DataUI;

    [Header("Tool")]
    [SerializeField] List<Tool> m_InstanceTool;
    [SerializeField] List<Tool> m_DataTool;

    [Header("Consumable")]
    [SerializeField] List<Consumable> m_InstanceConsumable;
    [SerializeField] List<Consumable> m_DataConsumable;

    [Header("Equipement")]
    [SerializeField] List<Equipement> m_InstanceEquipement;
    [SerializeField] List<Equipement> m_DataEquipement;

    [Header("Collectible")]
    [SerializeField] List<Collectible> m_InstanceCollectible;
    [SerializeField] List<Collectible> m_DataCollectible;

    [Header("Material")]
    [SerializeField] List<Material> m_InstanceMaterial;
    [SerializeField] List<Material> m_DataMaterial;

    [Header("SpecialResource")]
    [SerializeField] List<SpecialResource> m_InstanceSpecialResource;
    [SerializeField] List<SpecialResource> m_DataSpecialResource;

    [Header("VFX")]
    [SerializeField] List<VFX> m_InstanceVFX;
    [SerializeField] List<VFX> m_DataVFX;

    [Header("Audio")]
    [SerializeField] List<Audio> m_InstanceAudio;
    [SerializeField] List<Audio> m_DataAudio;

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

        foreach (Manager value in m_InstanceManager)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Map value in m_InstanceMap)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Biome value in m_InstanceBiome)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Block value in m_InstanceBlock)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Player value in m_InstancePlayer)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (NPC value in m_InstanceNPC)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Monster value in m_InstanceMonster)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (UI value in m_InstanceUI)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Tool value in m_InstanceTool)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Consumable value in m_InstanceConsumable)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Equipement value in m_InstanceEquipement)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Collectible value in m_InstanceCollectible)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Material value in m_InstanceMaterial)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (SpecialResource value in m_InstanceSpecialResource)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (VFX value in m_InstanceVFX)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }

        foreach (Audio value in m_InstanceAudio)
        {
            m_AvailableInstancePool.Add(value.type, new List<GameObject>());
        }
    }

    private void InitInstancePool()
    {
        if(m_InstancePool == null)
        {
            m_InstancePool = new Dictionary<object, GameObject>();
        }
        m_InstancePool.Clear();


        foreach (Manager value in m_InstanceManager)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Map value in m_InstanceMap)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Biome value in m_InstanceBiome)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Block value in m_InstanceBlock)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Player value in m_InstancePlayer)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (NPC value in m_InstanceNPC)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Monster value in m_InstanceMonster)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (UI value in m_InstanceUI)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Tool value in m_InstanceTool)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Consumable value in m_InstanceConsumable)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Equipement value in m_InstanceEquipement)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Collectible value in m_InstanceCollectible)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Material value in m_InstanceMaterial)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (SpecialResource value in m_InstanceSpecialResource)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (VFX value in m_InstanceVFX)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }

        foreach (Audio value in m_InstanceAudio)
        {
            m_InstancePool.Add(value.type, (GameObject)value.value);
        }
    }

    private void InitDataPool()
    {
        if (m_DataPool == null)
        {
            m_DataPool = new Dictionary<object, ScriptableObject>();
        }
        m_DataPool.Clear();

        foreach (Manager value in m_DataManager)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Map value in m_DataMap)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Biome value in m_DataBiome)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Block value in m_DataBlock)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Player value in m_DataPlayer)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (NPC value in m_DataNPC)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Monster value in m_DataMonster)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (UI value in m_DataUI)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Tool value in m_DataTool)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Consumable value in m_DataConsumable)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Equipement value in m_DataEquipement)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Collectible value in m_DataCollectible)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Material value in m_DataMaterial)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (SpecialResource value in m_DataSpecialResource)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (VFX value in m_DataVFX)
        {
            m_DataPool.Add(value.type, (ScriptableObject)value.value);
        }

        foreach (Audio value in m_DataAudio)
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

        if(currObject.transform.parent.gameObject.activeInHierarchy)
        {
            currObject.transform.parent = m_ParentGameObject.transform;
        }
        
        m_AvailableInstancePool[type].Add(currObject);
    }

    // initialise un gameObject et le retourn
    private GameObject InitObject(object type)
    {
        GameObject newObject = Instantiate(m_InstancePool[type]);
        if (m_ParentGameObject == null)
        {
            m_ParentGameObject = GameObject.Find(m_ParentGameObjectName);
        }
        newObject.transform.SetParent(m_ParentGameObject.transform);
        newObject.SetActive(false);

        return newObject;
    }

    public ScriptableObject GetData(object type)
    {
        return m_DataPool[type];
    }
}