using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStorageManagePlayer : DataStorage
{
    private DataManager m_GlobalDataManager;

    private GameObject m_PlayerObject;
    private StateMachinePlayer m_StateMachinePlayer;

    // le offset pour centrer sur un block de 1 metre dans unity
    private const float offset = 0.5f;

    public DataStorageManagePlayer(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        // fais spawner le player
        m_GlobalDataManager = (DataManager)m_StateMachine.GetData();
        m_PlayerObject = Pool.m_Instance.GetObject(m_GlobalDataManager.player);
        m_StateMachinePlayer = m_PlayerObject.GetComponent<StateMachinePlayer>();

        DataStorageManageMap map = (DataStorageManageMap)m_StateMachine.GetDataStorage(EnumStatesManager.manageMap);

        Vector3 pos = map.GetPointToWorld();
        pos.z = -1;
        pos.x += offset;

        m_PlayerObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        m_PlayerObject.transform.position = pos;

        m_PlayerObject.SetActive(true);
    }

    public Vector3 GetPlayerPos()
    {
        if(m_PlayerObject != null) return m_PlayerObject.transform.position;
        else return Vector3.zero;
    }

    public void InitEquipUI()
    {
        DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachinePlayer.GetDataStorage(EnumStatesPlayer.equip);
        dataStoragePlayerEquip.InitUI();
    }
}
