using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    [SerializeField] ScriptableObject m_Data;

    private List<EnumState> m_CurrStates;
    protected Dictionary<EnumState, State> m_States;

    private List<EnumState> m_StatesToAdd;
    private List<EnumState> m_StatesToDelete;

    private void Awake()
    {
        // initialise tout ---------------------------
        m_CurrStates = new List<EnumState>();
        m_States = new Dictionary<EnumState, State>();
        m_StatesToAdd = new List<EnumState>();
        m_StatesToDelete = new List<EnumState>();

        InitAllStates();
        // -------------------------------------------
    }

    private void Start()
    {
        // ajoute les states initial
        AddInitialsStates();
    }

    // appelle l'ordre d'execution ---------------
    private void FixedUpdate()
    {
        CallStart();
        CallFixedUpdate();
    }

    private void Update()
    {
        CallUpdate();
    }

    private void LateUpdate()
    {
        CallLateUpdate();
        CallEnd();
    }
    // --------------------------------------------

    // Fonction pour appeler l'ordre d'execution ---------------------------------
    private void CallStart()
    {
        // ajoute les state a ajouter et appel leur Awake 
        foreach (EnumState state in m_StatesToAdd)
        {
            if (!m_CurrStates.Contains(state))
            {
                m_CurrStates.Add(state);
                m_States[state].OnInit();
            }
        }
        m_StatesToAdd.Clear();
    }

    private void CallFixedUpdate()
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].FixedUpdate();
        }
    }

    private void CallUpdate()
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].Update();
        }
    }

    private void CallLateUpdate()
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].LateUpdate();
        }
    }

    private void CallEnd()
    {
        foreach (EnumState state in m_StatesToDelete)
        {
            if (m_CurrStates.Contains(state))
            {
                m_States[state].End();
                m_CurrStates.Remove(state);
            }
        }
        m_StatesToDelete.Clear();
    }
    //-----------------------------------------------------------------------------------------------------

    // gestion des states ---------------------------------------------------------------------------------

    // initialise tout les states possible
    public abstract void InitAllStates();

    // ajoute les state initial
    public abstract void AddInitialsStates();

    // ajoute une state dans la state courrant
    public void AddCurrState(EnumState state)
    {
        if(state != EnumState.none)
        {
            if (m_States.ContainsKey(state))
            {
                m_StatesToAdd.Add(state);
            }
        }
    }

    // pop une state des states courrant
    public void PopCurrState(EnumState state)
    {
        if (state != EnumState.none)
        {
            if (m_States.ContainsKey(state))
            {
                m_StatesToDelete.Add(state);
            }
        }
    }
    // -------------------------------------------------------------------------------------------------

    // appel les collision sur les state active ---------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].OnCollisionEnter2D(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].OnCollisionStay2D(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].OnCollisionExit2D(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].OnTriggerEnter2D(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].OnTriggerStay2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (EnumState state in m_CurrStates)
        {
            m_States[state].OnTriggerExit2D(collision);
        }
    }
    // -------------------------------------------------------------------------------------------------------
    
    // retourn le data de la stateMachine
    public ScriptableObject GetData()
    {
        return m_Data;
    }

    // retourn une state de la statemachine
    public State GetState(EnumState state)
    {
        return m_States[state];
    }
}
