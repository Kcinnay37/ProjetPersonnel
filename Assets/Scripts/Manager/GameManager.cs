using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }
}
