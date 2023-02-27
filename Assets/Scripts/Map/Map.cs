using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private EnumMaps m_DataMap;

    private MapGrid m_MapGrid;
    private MapGenerate m_MapGenerate;
    private MapView m_MapView;
    private MapPathfinding m_MapPathfinding;

    public static Map m_Instance;

    private bool m_IsGenerate;

    private void Awake()
    {
        m_Instance = this;

        m_MapGenerate = new MapGenerate(this);
        m_MapView = new MapView(this);
        m_MapGrid = new MapGrid(this);
        m_MapPathfinding = new MapPathfinding(this);
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        //if nouvelle partie
        m_MapGrid.InitValue();
        m_MapGenerate.GenerateMap();
        m_MapView.StartView();
        m_MapGrid.InitInitialPoint();
    }

    public MapGrid GetGrid()
    {
        return m_MapGrid;
    }

    public MapGenerate GetGenerate()
    {
        return m_MapGenerate;
    }

    public MapView GetView()
    {
        return m_MapView;
    }

    public DataMap GetData()
    {
        return (DataMap)Pool.m_Instance.GetData(m_DataMap);
    }

    public MapPathfinding GetPathfinding()
    {
        return m_MapPathfinding;
    }
}
