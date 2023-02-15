using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerExcavationTool : StateRessource
{
    private DataPlayer m_DataPlayer;

    private StatePlayerController m_StatePlayerController;

    private DataTool m_DataTool;
    private GameObject m_Object;

    public StatePlayerExcavationTool(StateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void OnInit()
    {
        //initialise les valeur
        m_DataPlayer = (DataPlayer)m_StateMachine.GetData();
        m_StatePlayerController = (StatePlayerController)m_StateMachine.GetState(EnumStatesPlayer.controller);

        //Instanci l'outil
        m_Object = Pool.m_Instance.GetObject(EnumTools.pickaxe);

        m_Object.transform.parent = GameObject.Find("PlayerWeaponSlot").transform;
        m_Object.SetActive(true);
        m_Object.transform.localPosition = Vector3.zero;
    }

    public override void End()
    {
        Pool.m_Instance.RemoveObject(m_Object, EnumTools.pickaxe);
    }

    public override void ActionKeyDown()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

        StateManagerManageMap manageMap = (StateManagerManageMap)StateMachineManager.m_Instance.GetState(EnumStatesManager.manageMap);

        manageMap.PopBlockAt(mouseWorldPosition);
    }

    public override void ActionOldKey()
    {

    }
}