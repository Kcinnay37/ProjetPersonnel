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

        Dictionary<Vector2Int, EnumCollectibles> test = Map.m_Instance.GetGrid().GetCollectible();
        foreach (KeyValuePair<Vector2Int, EnumCollectibles> lol in test)
        {
            Map.m_Instance.GetGrid().AddBlockAt(new Vector3(lol.Key.x, lol.Key.y, 0), EnumBlocks.rockIce);
        }
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
}
