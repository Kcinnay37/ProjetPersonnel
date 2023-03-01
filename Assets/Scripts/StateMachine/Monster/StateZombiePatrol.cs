using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombiePatrol : State
{
    private DataZombie m_GlobalData;
    
    private Dictionary<Vector2Int, MapPathfinding.Node> m_AllPossiblePath;
    private bool m_IsArrived;
    private Vector2Int m_CurrPosToGo;
    private List<Vector2Int> m_CurrPath;

    private StateZombieMovement m_StateZombieMovement;

    public StateZombiePatrol(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_GlobalData = (DataZombie)m_StateMachine.GetData();
        m_CurrPosToGo = Vector2Int.zero;

        m_AllPossiblePath = new Dictionary<Vector2Int, MapPathfinding.Node>();
        m_CurrPath = new List<Vector2Int>();

        m_IsArrived = true;

        m_StateZombieMovement = (StateZombieMovement)m_StateMachine.GetState(EnumStatesMonster.movement);
    }

    public override void Update()
    {
        if(m_StateZombieMovement.GetIsArrived())
        {
            m_StateZombieMovement.StartMoveRandomPath();
        }
    }


}
