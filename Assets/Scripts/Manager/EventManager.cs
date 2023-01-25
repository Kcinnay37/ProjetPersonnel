using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    // dictionary d'evenement
    private Dictionary<string, Func<Dictionary<string, object>, object>> m_EventDictionary;

    private static EventManager m_EventManager;

    public static EventManager m_Instance
    {
        get
        {
            // initialise l'instance si elle n'est pas initialisé
            if (m_EventManager == null)
            {
                Init();
            }
            return m_EventManager;
        }
    }

    private static void Init()
    {
        m_EventManager = new EventManager();
        m_EventManager.m_EventDictionary = new Dictionary<string, Func<Dictionary<string, object>, object>>();
    }

    // ajoute une Func au dictionnaire a l'index eventName
    public static void StartListening(string eventName, Func<Dictionary<string, object>, object> listener)
    {
        Func<Dictionary<string, object>, object> currEvent;
        if (m_Instance.m_EventDictionary.TryGetValue(eventName, out currEvent))
        {
            currEvent += listener;
            m_Instance.m_EventDictionary[eventName] = currEvent;
        }
        else
        {
            m_Instance.m_EventDictionary.Add(eventName, listener);
        }
    }

    // retire une Func au dictionnaire a l'index eventName
    public static void StopListening(string eventName, Func<Dictionary<string, object>, object> listener)
    {
        Func<Dictionary<string, object>, object> currEvent;
        if (m_Instance.m_EventDictionary.TryGetValue(eventName, out currEvent))
        {
            currEvent -= listener;
            m_Instance.m_EventDictionary[eventName] = currEvent;
        }
    }

    // fait jouer les fonction a l'index eventName et retourn les valeur dans une List
    public static List<object> TriggerEvent(string eventName, Dictionary<string, object> parametre)
    {
        List<object> returnValue = new List<object>();

        Func<Dictionary<string, object>, object> currEvent;
        if (m_Instance.m_EventDictionary.TryGetValue(eventName, out currEvent))
        {
            foreach(Func<Dictionary<string, object>, object> function in currEvent.GetInvocationList())
            {
                returnValue.Add(function(parametre));
            }
        }

        return returnValue;
    }
}
