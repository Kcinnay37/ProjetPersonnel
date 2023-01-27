using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            MapGenerator.m_Instance.GenerateMap();
        }
    }
}
