using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private List<object> m_CurrStates;

    protected Dictionary<object, State> m_States;

    private List<object> m_StatesToAdd;
    private List<object> m_StatesToDelete;

    private Dictionary<object, StateData> m_StatesDatas;

    private void Awake()
    {
        // initialise tout ---------------------------
        m_CurrStates = new List<object>();
        m_States = new Dictionary<object, State>();
        m_StatesToAdd = new List<object>();
        m_StatesToDelete = new List<object>();
        m_StatesDatas = new Dictionary<object, StateData>();
        // -------------------------------------------

        InitAllStates();
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
        foreach (object state in m_StatesToAdd)
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
        foreach (object state in m_CurrStates)
        {
            m_States[state].FixedUpdate();
        }
    }

    private void CallUpdate()
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].Update();
        }
    }

    private void CallLateUpdate()
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].LateUpdate();
        }
    }

    private void CallEnd()
    {
        foreach (object state in m_StatesToDelete)
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

    private void OnEnable()
    {        
        // ajoute les states initial
        AddInitialsStates();

        foreach(KeyValuePair<object, StateData> state in m_StatesDatas)
        {
            state.Value.OnInit();
        }
    }

    private void OnDisable()
    {
        foreach (KeyValuePair<object, StateData> state in m_StatesDatas)
        {
            state.Value.End();
        }

        foreach (object state in m_CurrStates)
        {
            m_States[state].End();
        }
        m_CurrStates.Clear();
        m_StatesToAdd.Clear();
        m_StatesToDelete.Clear();
    }

    // gestion des states ---------------------------------------------------------------------------------

    // initialise tout les states possible
    public abstract void InitAllStates();

    // ajoute les state initial
    public abstract void AddInitialsStates();

    //ajoute une nouvelle state a la state machine
    public void AddNewState(object key, State value)
    {
        if (!m_States.ContainsKey(key))
        {
            m_States.Add(key, value);
        }       
    }

    public void AddNewStateData(object key, StateData value)
    {
        if (!m_StatesDatas.ContainsKey(key))
        {
            m_StatesDatas.Add(key, value);
        }
    }

    public void PopStateData(object state)
    {
        if (m_StatesDatas.ContainsKey(state))
        {
            m_StatesDatas.Remove(state);
        }
    }

    // ajoute une state dans la state courrant
    public void AddCurrState(object state)
    {
        if (m_States.ContainsKey(state))
        {
            m_StatesToAdd.Add(state);
        }
    }

    // pop une state des states courrant
    public void PopCurrState(object state)
    {
        if (m_States.ContainsKey(state))
        {
            m_StatesToDelete.Add(state);
        }
    }
    // -------------------------------------------------------------------------------------------------

    // appel les collision sur les state active ---------------------------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].OnCollisionEnter2D(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].OnCollisionStay2D(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].OnCollisionExit2D(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].OnTriggerEnter2D(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].OnTriggerStay2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (object state in m_CurrStates)
        {
            m_States[state].OnTriggerExit2D(collision);
        }
    }
    // -------------------------------------------------------------------------------------------------------

    public virtual ScriptableObject GetData()
    {
        return null;
    }

    // retourn une state de la statemachine
    public State GetState(object state)
    {
        return m_States[state];
    }

    public StateData GetStateData(object state)
    {
        return m_StatesDatas[state];
    }
}
