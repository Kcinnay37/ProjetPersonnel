using UnityEngine;

public class StatePlayerBuild : StateRessource
{
    private DataPlayer m_GlobalDataPlayer;

    private DataBlock m_DataBlock;
    private EnumBlocks m_BlockType;
    private GameObject m_Object;

    private Transform m_RaycastPoint;

    private StatePlayerControllerMovement m_StatePlayerControllerMovement;
    private Animator m_Animator;

    public StatePlayerBuild(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void OnInit()
    {
        m_RaycastPoint = GameObject.Find("PlayerRaycastPoint").transform;

        m_GlobalDataPlayer = (DataPlayer)m_StateMachine.GetData();

        m_StatePlayerControllerMovement = (StatePlayerControllerMovement)m_StateMachine.GetState(EnumStatesPlayer.controllerMovement);
        m_Animator = m_StateMachine.GetComponent<Animator>();

        DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
        InventoryCase caseEquip = dataStoragePlayerEquip.GetEquipCase();
        m_DataBlock = (DataBlock)Pool.m_Instance.GetData(caseEquip.resource);
        m_BlockType = (EnumBlocks)caseEquip.resource;

        //Instanci l'outil
        m_Object = Pool.m_Instance.GetObject(m_DataBlock.instanceType);
        m_Object.GetComponent<ResourceInWorld>().InitResource(true, Vector2.zero, caseEquip.resource, EnumBlocks.block);
        

        Vector3 scale = m_Object.transform.localScale;
        
        if((m_StateMachine.transform.localScale.x < 0 && scale.x > 0) || (m_StateMachine.transform.localScale.x > 0 && scale.x < 0))
        {
            scale.x = -scale.x;
        }
        m_Object.transform.localScale = scale;

        m_Object.transform.parent = GameObject.Find("PlayerWeaponSlot").transform;
        m_Object.SetActive(true);

        m_Object.transform.localPosition = Vector3.zero;

        m_Object.transform.localRotation = Quaternion.identity;
    }

    public override void End()
    {
        m_GlobalDataPlayer = null;

        m_Object.GetComponent<SpriteRenderer>().sprite = null;
        Pool.m_Instance.RemoveObject(m_Object, m_DataBlock.instanceType);
        m_DataBlock = null;

        m_StatePlayerControllerMovement = null;
        m_Animator = null;
    }

    public override void ActionKeyDown()
    {

    }

    public override void ActionOldKey()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 firstPos = m_RaycastPoint.position;

        Vector2 dir = (mouseWorldPosition - firstPos).normalized;

        //regarde si la souris est trop loin
        if (Vector2.Distance(m_RaycastPoint.position, mouseWorldPosition) > m_GlobalDataPlayer.blockDropDistance)
        {
            return;
        }

        if(!Map.m_Instance.GetGrid().CheckCanAddBlockAt(mouseWorldPosition))
        {
            return;
        }

        if(m_StatePlayerControllerMovement == null)
        {
            m_StatePlayerControllerMovement = (StatePlayerControllerMovement)m_StateMachine.GetState(EnumStatesPlayer.controllerMovement);
        }

        float radianAngle = m_DataBlock.coneRadius * (Mathf.PI / 180);
        Vector2 dirCone = Vector2.right * m_StatePlayerControllerMovement.GetPlayerDir();

        float cosAngle = Vector2.Dot(dirCone, dir);
        float radAngle = Mathf.Acos(cosAngle);
        float degAngle = radAngle * Mathf.Rad2Deg;

        float dist = Vector2.Distance(firstPos, mouseWorldPosition);

        //pas necessaisaire 
        Vector2 dir1 = new Vector2(dirCone.x * Mathf.Cos(radianAngle) - dirCone.y * Mathf.Sin(radianAngle),
                        dirCone.x * Mathf.Sin(radianAngle) + dirCone.y * Mathf.Cos(radianAngle));

        Vector2 dir2 = new Vector2(dirCone.x * Mathf.Cos(-radianAngle) - dirCone.y * Mathf.Sin(-radianAngle),
                                dirCone.x * Mathf.Sin(-radianAngle) + dirCone.y * Mathf.Cos(-radianAngle));

        Debug.DrawRay(firstPos, dir * dist);
        Debug.DrawRay(firstPos, dir1 * dist);
        Debug.DrawRay(firstPos, dir2 * dist);
        //--

        if (degAngle > m_DataBlock.coneRadius)
        {
            return;
        }

        //regarde si il a de l'environement dans les jambe
        RaycastHit2D[] hits = Physics2D.RaycastAll(firstPos, dir, dist);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Environement"))
            {
                return;
            }
        }

        if (Map.m_Instance.GetGrid().AddBlockAt(mouseWorldPosition, m_BlockType))
        {
            AudioManager.m_Instance.PlaySoundAt(m_StateMachine.transform.position, EnumAudios.placeBlock);

            m_Animator.SetTrigger("DropBlock");

            DataStoragePlayerEquip dataStoragePlayerEquip = (DataStoragePlayerEquip)m_StateMachine.GetDataStorage(EnumStatesPlayer.equip);
            dataStoragePlayerEquip.DropOneAtCaseEquip();
        }
    }
}
