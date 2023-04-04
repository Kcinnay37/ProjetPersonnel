using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerControllerMovement : State
{
    private DataPlayer m_GlobalDataPlayer;
    private Rigidbody2D m_RigidBody;
    private Animator m_Animator;

    private int m_PlayerDir = 1;

    public StatePlayerControllerMovement(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();
        m_RigidBody = m_StateMachine.GetComponent<Rigidbody2D>();
        m_Animator = m_StateMachine.GetComponent<Animator>();
    }

    public override void End()
    {
        Vector3 velo = m_RigidBody.velocity;
        velo.x = 0;
        m_RigidBody.velocity = velo;
        UpdateAnimator();
    }

    public override void Update()
    {
        UpdateMove();
        UpdateJump();
        UpdateAnimator();

        if (Input.GetKeyDown(KeyCode.Mouse3))
        {
            m_StateMachine.StartCoroutine(testo());
        }
    }

    IEnumerator testo()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2Int localMousePos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(worldMousePos);

        Vector3 playerWorldPos = m_StateMachine.transform.position;
        playerWorldPos.y += 0.2f;
        Vector2Int localPlayerPos = (Vector2Int)Map.m_Instance.GetGrid().ConvertWorldToCell(playerWorldPos);

        
        Dictionary<Vector2Int, MapPathfinding.Node> posiblePath = Map.m_Instance.GetPathfinding().GetAllMovePossibility(localPlayerPos, new Vector2Int(3, 3), 1, 2);


        if (posiblePath.ContainsKey(localMousePos))
        {
            Debug.Log("test");
            MapPathfinding.Node node = posiblePath[localMousePos];
            while (!node.position.Equals(localPlayerPos))
            {
                Vector3 pos = new Vector3(node.position.x, node.position.y, 0);
                Map.m_Instance.GetGrid().AddBlockAt(pos, EnumBlocks.rockFire);
                if (!posiblePath.ContainsKey(node.pathfrom))
                {
                    break;
                }
                node = posiblePath[node.pathfrom];
                yield return new WaitForSeconds(0.25f);

            }

        }
        
    }

    private void UpdateMove()
    {
        Vector2 velo = m_RigidBody.velocity;

        //mets la velo a 0 si les deux touche opposé sont enfoncé
        if (Input.GetKey(m_GlobalDataPlayer.leftKey) && Input.GetKey(m_GlobalDataPlayer.rightKey))
        {
            velo.x = 0;
        }
        else
        {
            // si il cour ou pas
            if (Input.GetKey(m_GlobalDataPlayer.runKey))
            {
                velo.x = Input.GetAxis("Horizontal") * m_GlobalDataPlayer.baseRunSpeed;
            }
            else
            {
                velo.x = Input.GetAxis("Horizontal") * m_GlobalDataPlayer.baseWalkSpeed;
            }
        }

        m_RigidBody.velocity = velo;
    }

    private void UpdateAnimator()
    {
        //rotationne le player dans la bonne direction
        if (m_RigidBody.velocity.x < 0)
        {
            m_PlayerDir = -1;
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = m_PlayerDir;
            m_StateMachine.transform.localScale = rota;
        }
        else if (m_RigidBody.velocity.x > 0)
        {
            m_PlayerDir = 1;
            Vector3 rota = m_StateMachine.transform.localScale;
            rota.x = m_PlayerDir;
            m_StateMachine.transform.localScale = rota;
        }

        m_Animator.SetFloat("Velo", Mathf.Abs(m_RigidBody.velocity.x / m_GlobalDataPlayer.baseRunSpeed));
    }

    private void UpdateJump()
    {
        if (Input.GetKeyDown(m_GlobalDataPlayer.jumpKeyCode) && CheckCanJump())
        {
            m_RigidBody.AddForce(new Vector2(0, m_GlobalDataPlayer.baseJumpForce * m_RigidBody.mass));
        }
    }

    private bool CheckCanJump()
    {
        Vector2 playerPos = m_StateMachine.transform.position;

        Vector2 pos1 = playerPos;
        

        Vector2 pos2 = playerPos;
        
        if(m_PlayerDir == 1)
        {
            pos1.x -= 0.34f;
            pos2.x += 0.25f;
        }
        else
        {
            pos1.x += 0.34f;
            pos2.x -= 0.25f;
        }

        RaycastHit2D[] hit1 = Physics2D.RaycastAll(pos1, Vector2.down, 0.1f);
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(pos2, Vector2.down, 0.1f);
        RaycastHit2D[] hit3 = Physics2D.RaycastAll(playerPos, Vector2.down, 0.1f);

        Debug.DrawRay(pos1, Vector2.down * 0.1f);
        Debug.DrawRay(pos2, Vector2.down * 0.1f);
        Debug.DrawRay(playerPos, Vector2.down * 0.1f);

        foreach (RaycastHit2D hits in hit1)
        {
            if (hits.transform.CompareTag("Environement") || hits.transform.CompareTag("ResourceInWorld") || hits.transform.CompareTag("Enemie"))
            {
                return true;
            }
        }

        foreach (RaycastHit2D hits in hit2)
        {
            if (hits.transform.CompareTag("Environement") || hits.transform.CompareTag("ResourceInWorld") || hits.transform.CompareTag("Enemie"))
            {
                return true;
            }
        }

        foreach (RaycastHit2D hits in hit3)
        {
            if (hits.transform.CompareTag("Environement") || hits.transform.CompareTag("ResourceInWorld") || hits.transform.CompareTag("Enemie"))
            {
                return true;
            }
        }

        return false;
    }

    public int GetPlayerDir()
    {
        return m_PlayerDir;
    }
}
