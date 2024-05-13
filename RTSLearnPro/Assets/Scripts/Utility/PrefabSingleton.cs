using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PrefabSingleton<T> : MonoBehaviour where T:Component
{
    protected static T m_Instance;

    private bool m_IsAwaked;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();

                if (m_Instance != null)
                {
                    PrefabSingleton<T> target = m_Instance as PrefabSingleton<T>;
                    if (!target.m_IsAwaked)
                    {
                        target.Awake();
                    }
                }
            }

            if (m_Instance == null)
            {
                string path = "Prefabs/"+typeof(T).Name;
                GameObject prefab = Resources.Load<GameObject>(path);
                m_Instance = Instantiate(prefab).GetComponent<T>();
                m_Instance = Instantiate(prefab).GetComponent<T>();
            }

            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        if(m_Instance==null)
        {
            m_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        m_IsAwaked = true;
    }
}
