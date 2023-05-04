using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private EnumAddInWorld m_DataType;

    private DataCollectible m_Data;
    private int m_CurrHealth;

    private void OnEnable()
    {
        m_Data = (DataCollectible)Pool.m_Instance.GetData(m_DataType);
        m_CurrHealth = m_Data.health;
    }

    private void OnDisable()
    {
        
    }

    public void TakeDamage(int damage)
    {
        m_CurrHealth -= damage;
        if (m_CurrHealth < 0)
        {
            m_CurrHealth = 0;
        }

        if(m_CurrHealth == 0)
        {
            AudioManager.m_Instance.PlaySoundAt(transform.position, m_Data.destroySound);
            ResourceManager.m_Instance.Drops(m_Data.drop, transform.position + new Vector3(m_Data.drop.offsetDrop.x, m_Data.drop.offsetDrop.y, 0));
            AddInWorldManager.m_Instance.DestroyCollectible(gameObject);
        }
        else
        {
            AudioManager.m_Instance.PlaySoundAt(transform.position, m_Data.hitSound);
        }

    }

    public List<EnumTools> GetToolsCanInteract()
    {
        return m_Data.m_ToolsCanInteract;
    }
}
