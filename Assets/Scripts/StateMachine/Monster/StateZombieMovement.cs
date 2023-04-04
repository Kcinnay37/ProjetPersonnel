using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateZombieMovement : State
{
    private DataZombie m_GlobalDataMonster;

    private Vector2Int m_PointToGo;
    private List<Vector2Int> m_Path;

    private Rigidbody2D m_Rigidbody;
    private Animator m_Animator;

    private bool m_IsArrived;

    private Vector2Int m_LastDir;

    private Coroutine m_CoroutineMoving;

    private int m_Try;

    private int m_ZombieDir = 1;

    public StateZombieMovement(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_GlobalDataMonster = (DataZombie)m_StateMachine.GetData();

        m_Rigidbody = m_StateMachine.GetComponent<Rigidbody2D>();
        m_Animator = m_StateMachine.GetComponent<Animator>();

        m_IsArrived = true;
        m_PointToGo = new Vector2Int();
        m_Path = new List<Vector2Int>();
        m_LastDir = Vector2Int.zero;
        m_CoroutineMoving = null;
        m_Try = 0;
    }

    public override void Update()
    {
        UpdateMove();

        UpdateAnimator();
    }

    public override void End()
    {
        StopMoving();
        if(m_CoroutineMoving != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineMoving);
            m_CoroutineMoving = null;
        }
        UpdateAnimator();
    }


    private void UpdateMove()
    {
        if (!m_IsArrived)
        {
            if (m_Path.Count == 0 || m_Try == 1)
            {
                m_Try = 0;
                m_IsArrived = true;
                return;
            }
            m_PointToGo = m_Path[0];

            if (m_CoroutineMoving == null)
            {
                Vector3 pos = m_StateMachine.transform.position;
                Vector2Int localPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(pos);
                Vector2Int dir = m_PointToGo - localPos;

                if (dir.Equals(Vector2Int.zero))
                {
                    m_Try = 0;
                    m_Path.RemoveAt(0);
                }
                else
                {
                    m_Try++;
                    m_CoroutineMoving = m_StateMachine.StartCoroutine(MoveAtDir(dir));
                }

            }
        }
    }

    private void UpdateAnimator()
    {
        //rotationne le player dans la bonne direction
        Vector3 rota = m_StateMachine.transform.localScale;
        rota.x = m_ZombieDir;
        m_StateMachine.transform.localScale = rota;

        m_Animator.SetFloat("MoveSpeed", Mathf.Abs(m_Rigidbody.velocity.x));
    }

    private IEnumerator MoveAtDir(Vector2Int dir)
    {
        if(dir.y > 0 && m_Rigidbody.velocity.y <= 0.1 && m_Rigidbody.velocity.y >= -0.1)
        {
            m_Rigidbody.AddForce(new Vector2(0, m_GlobalDataMonster.jumpForce));
        }

        float timer = 0;
        float maxTime = 1 / m_GlobalDataMonster.moveSpeed;

        while(true)
        {
            timer += Time.deltaTime;
            if(timer >= maxTime)
            {
                break;
            }

            Vector2 velo = m_Rigidbody.velocity;
            velo.x = (int)Mathf.Sign(dir.x) * m_GlobalDataMonster.moveSpeed;
            m_Rigidbody.velocity = velo;

            m_ZombieDir = (int)Mathf.Sign(dir.x) * 1;

            yield return null;
        }


        m_CoroutineMoving = null;
    }

    public bool StartMoveRandomPath()
    {
        m_IsArrived = false;
        m_Path.Clear();

        //Prend un nouveau chemin

        // va chercher tout les moves possible relatif a la position
        Vector3 worldPos = m_StateMachine.transform.position;
        worldPos.y += 0.2f;
        Vector2Int localPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(worldPos);

        Dictionary<Vector2Int, MapPathfinding.Node> m_AllPossiblePath;
        m_AllPossiblePath = Map.m_Instance.GetPathfinding().GetAllMovePossibility(localPos, m_GlobalDataMonster.sizeInAstar, m_GlobalDataMonster.jumpHeightInAstar, m_GlobalDataMonster.airMoveSpeedInAstar);

        //si il a assez de move possible
        if (m_AllPossiblePath.Count > 2)
        {
            //va chercher une position aleatoir a aller
            List<Vector2Int> allPos = new List<Vector2Int>(m_AllPossiblePath.Keys);
            int index = Random.Range(0, allPos.Count);
            Dictionary<EnumBlocks, EnumBlocks> valueCanGo = Map.m_Instance.GetGrid().GetBackGroundDict();
            EnumBlocks[,] grid = Map.m_Instance.GetGrid().GetGrid();
            Vector2Int dest = allPos[index];

            if (dest.y - 1 < 0)
            {
                return false;
            }

            while (valueCanGo.ContainsKey(grid[dest.x, dest.y - 1]))
            {
                index = Random.Range(0, allPos.Count);
                dest = allPos[index];

                if(dest.y - 1 < 0)
                {
                    return false;
                }
            }

            if (dest.Equals(localPos))
            {
                return false;
            }

            //set le path
            MapPathfinding.Node node = m_AllPossiblePath[dest];
            while (!node.position.Equals(localPos))
            {
                if (!m_AllPossiblePath.ContainsKey(node.pathfrom))
                {
                    break;
                }
                m_Path.Add(node.position);
                node = m_AllPossiblePath[node.pathfrom];
            }

            m_Path.Reverse();

            return true;
        }
        return false;
    }

    public bool MoveToPlayer()
    {
        m_IsArrived = false;
        m_Path.Clear();

        //Prend un nouveau chemin

        // va chercher tout les moves possible relatif a la position
        Vector3 worldPos = m_StateMachine.transform.position;
        worldPos.y += 0.2f;
        Vector2Int localPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(worldPos);

        Dictionary<Vector2Int, MapPathfinding.Node> m_AllPossiblePath;
        m_AllPossiblePath = Map.m_Instance.GetPathfinding().GetAllMovePossibility(localPos, new Vector2Int(1, 2), 1, 2);

        Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();
        Vector2Int localPlayerPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(playerPos);

        Dictionary<EnumBlocks, EnumBlocks> valueCanGo = Map.m_Instance.GetGrid().GetBackGroundDict();
        EnumBlocks[,] grid = Map.m_Instance.GetGrid().GetGrid();

        while (!valueCanGo.ContainsKey(grid[localPlayerPos.x, localPlayerPos.y]) && valueCanGo.ContainsKey(grid[localPlayerPos.x, localPlayerPos.y - 1]))
        {
            localPlayerPos.y--;
        }

        if(m_AllPossiblePath.ContainsKey(localPlayerPos))
        {
            //set le path
            MapPathfinding.Node node = m_AllPossiblePath[localPlayerPos];
            while (!node.position.Equals(localPos))
            {
                if (!m_AllPossiblePath.ContainsKey(node.pathfrom))
                {
                    break;
                }
                m_Path.Add(node.position);
                node = m_AllPossiblePath[node.pathfrom];
            }

            m_Path.Reverse();

            return true;
        }

        return false;
    }

    public void StopMoving()
    {
        if(m_CoroutineMoving != null)
        {
            m_StateMachine.StopCoroutine(m_CoroutineMoving);
            m_CoroutineMoving = null;
        }
        m_Path.Clear();

        m_Try = 0;
        m_IsArrived = true;

        Vector2 velo = m_Rigidbody.velocity;
        velo.x = 0;
        m_Rigidbody.velocity = velo;
    }

    public void CheckIsPlayerChangedPos()
    {
        Vector3 playerPos = PlayerManager.m_Instance.GetCurrPlayerPos();
        Vector2Int localPlayerPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(playerPos);

        Dictionary<EnumBlocks, EnumBlocks> valueCanGo = Map.m_Instance.GetGrid().GetBackGroundDict();
        EnumBlocks[,] grid = Map.m_Instance.GetGrid().GetGrid();

        while (!valueCanGo.ContainsKey(grid[localPlayerPos.x, localPlayerPos.y]) && valueCanGo.ContainsKey(grid[localPlayerPos.x, localPlayerPos.y - 1]))
        {
            localPlayerPos.y--;
        }

        if(m_Path.Count != 0)
        {
            if (!m_Path[m_Path.Count - 1].Equals(localPlayerPos))
            {
                MoveToPlayer();
            }
        }
    }

    public bool GetIsArrived()
    {
        return m_IsArrived;
    }
}
