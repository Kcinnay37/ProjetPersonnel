using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private EnumPlayers m_Player;
    [SerializeField] private float m_WaitForSpawn;

    private GameObject m_CurrPlayer;

    public static PlayerManager m_Instance;

    private void Awake()
    {
        m_Instance = this;
    }

    public void Start()
    {
        StartCoroutine(WaitForSpawn(m_WaitForSpawn));
    }

    public IEnumerator WaitForSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        m_CurrPlayer = Pool.m_Instance.GetObject(m_Player);
        m_CurrPlayer.SetActive(true);
    }

    public Vector3 GetCurrPlayerPos()
    {
        if (m_CurrPlayer == null) return Vector3.zero;
        return m_CurrPlayer.transform.position;
    }

    public bool CurrPlayerCollectResource(object resource)
    {
        StateMachinePlayer stateMachinePlayer = m_CurrPlayer.GetComponent<StateMachinePlayer>();
        StatePlayerControllerInventory statePlayerControllerInventory = (StatePlayerControllerInventory)stateMachinePlayer.GetState(EnumStatesPlayer.controllerInventory);
        return statePlayerControllerInventory.CollectResource(resource);
    }

    public void CurrPlayerAddState(EnumStatesPlayer state)
    {
        StateMachinePlayer stateMachinePlayer = m_CurrPlayer.GetComponent<StateMachinePlayer>();
        stateMachinePlayer.AddCurrState(state);
    }

    public void CurrPlayerRemoveState(EnumStatesPlayer state)
    {
        StateMachinePlayer stateMachinePlayer = m_CurrPlayer.GetComponent<StateMachinePlayer>();
        stateMachinePlayer.PopCurrState(state);
    }
}
