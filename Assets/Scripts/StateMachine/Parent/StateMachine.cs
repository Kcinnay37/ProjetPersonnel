using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    //list des states courrant de la state machine
    private List<object> m_CurrStates;

    //dictionnaire de tout les state possible de la state machine
    protected Dictionary<object, State> m_States;
    
    //list des state a ajouter a la state machine
    private List<object> m_StatesToAdd;

    //list des state a retirer de la state machine
    private List<object> m_StatesToDelete;



    //list des dataStorage courrant de la state machine
    private Dictionary<object, DataStorage> m_CurrDataStorage;

    //dictionnaire de tout les data storage de la state machine
    private Dictionary<object, DataStorage> m_DataStorage;

    //list des dataStorage a ajouter a la state machine
    private List<object> m_DataStorageToAdd;

    //list des state a retirer de la state machine
    private List<object> m_DataStorageToDelete;


    private void Awake()
    {
        // initialise tout  les valeur ---------------------------
        m_CurrStates = new List<object>();
        m_States = new Dictionary<object, State>();
        m_StatesToAdd = new List<object>();
        m_StatesToDelete = new List<object>();

        m_CurrDataStorage = new Dictionary<object, DataStorage>();
        m_DataStorage = new Dictionary<object, DataStorage>();
        m_DataStorageToAdd = new List<object>();
        m_DataStorageToDelete = new List<object>();
        // -------------------------------------------

        //va initialiser tout les state dans l'enfant
        InitAllStatesAndData();
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
        //ajoute les dataStorage a ajouter et appel leur OnInit
        foreach (object dataStorage in m_DataStorageToAdd)
        {
            if(!m_CurrDataStorage.ContainsKey(dataStorage))
            {
                m_CurrDataStorage.Add(dataStorage, m_DataStorage[dataStorage]);
                m_DataStorage[dataStorage].OnInit();
            }
        }
        m_DataStorageToAdd.Clear();

        // ajoute les state a ajouter et appel leur OnInit
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

        foreach(object dataStorage in m_DataStorageToDelete)
        {
            if(m_CurrDataStorage.ContainsKey(dataStorage))
            {
                m_DataStorage[dataStorage].End();
                m_CurrDataStorage.Remove(dataStorage);
            }
        }
        m_DataStorageToDelete.Clear();
    }
    //-----------------------------------------------------------------------------------------------------

    private void OnEnable()
    {        
        // ajoute les states initial a partir de la state machine enfant
        AddInitialsStatesAndData();
    }

    private void OnDisable()
    {
        //appel le end sur tout les data de la state machine
        foreach (KeyValuePair<object, DataStorage> state in m_DataStorage)
        {
            state.Value.End();
        }

        //appel le end sur tout les state courrant de la state machine
        foreach (object state in m_CurrStates)
        {
            m_States[state].End();
        }

        //vide les lists
        m_CurrStates.Clear();
        m_StatesToAdd.Clear();
        m_StatesToDelete.Clear();
    }

    // gestion des states ---------------------------------------------------------------------------------

    // initialise tout les states possible
    public abstract void InitAllStatesAndData();

    // ajoute les state initial
    public abstract void AddInitialsStatesAndData();

    //initialise une nouvelle state dans la state machine
    public void AddNewState(object key, State value)
    {
        if (!m_States.ContainsKey(key))
        {
            m_States.Add(key, value);
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

    public void AddNewDataStorage(object key, DataStorage value)
    {
        if (!m_DataStorage.ContainsKey(key))
        {
            m_DataStorage.Add(key, value);
        }
    }

    public void AddCurrDataStorage(object dataStorage)
    {
        if (m_DataStorage.ContainsKey(dataStorage))
        {
            m_DataStorageToAdd.Add(dataStorage);
        }
    }

    public void PopCurrDataStorage(object dataStorage)
    {
        if (m_DataStorage.ContainsKey(dataStorage))
        {
            m_DataStorageToDelete.Add(dataStorage);
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

    //retourne le data global de la statemachine
    public virtual ScriptableObject GetData()
    {
        return null;
    }

    // retourn une state active de la statemachine
    public State GetState(object state)
    {
        if (!m_CurrStates.Contains(state)) return null;
        return m_States[state];
    }

    // retourne un data local de la state machine
    public DataStorage GetDataStorage(object state)
    {
        if (!m_CurrDataStorage.ContainsKey(state)) return null;
        return m_DataStorage[state];
    }
}
