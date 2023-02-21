using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnumPlayers m_Player;
    private GameObject m_CurrPlayer;

    public static GameManager m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    private void Start()
    {
        Map.m_Instance.GenerateMap();

        m_CurrPlayer = Pool.m_Instance.GetObject(m_Player);
        m_CurrPlayer.SetActive(true);
    }

    public Vector3 GetCurrPlayerPos()
    {
        if(m_CurrPlayer == null) return Vector3.zero;
        return m_CurrPlayer.transform.position;
    }

    public void InstanciateResourceInWorldAt(object dataResource, Vector3 pos, Vector2 velo, int dir)
    {
        DataResource data = (DataResource)Pool.m_Instance.GetData(dataResource);

        GameObject objectResource = Pool.m_Instance.GetObject(data.GetInstanceType());
        objectResource.GetComponent<ResourceInWorld>().InitResource(false, velo, dataResource, data.GetInstanceType());
        objectResource.transform.rotation = Quaternion.identity;
        objectResource.transform.position = pos;

        Vector3 scale = objectResource.transform.localScale;
        if ((dir < 0 && scale.x > 0) || (dir > 0 && scale.x < 0))
        {
            scale.x = -scale.x;
        }
        objectResource.transform.localScale = scale;

        objectResource.SetActive(true);
    }

    public bool CurrPlayerCollectResource(object resource)
    {
        StateMachinePlayer stateMachinePlayer = m_CurrPlayer.GetComponent<StateMachinePlayer>();
        StatePlayerControllerInventory statePlayerControllerInventory = (StatePlayerControllerInventory)stateMachinePlayer.GetState(EnumStatesPlayer.controllerInventory);
        return statePlayerControllerInventory.CollectResource(resource);
    }
}
