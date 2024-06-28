using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    public static T inst;

    protected virtual void Awake()
    {
        if(inst == null)
        {
            inst = (T)this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Debug.LogError(string.Format("{0} is already exist. Destroy {0}.", name));
            Destroy(inst.gameObject);
        }
    }
    
    public static T Instance
    {
        get
        {
            if (!inst)
            {
                GameObject obj;
                obj = GameObject.Find(typeof(T).Name);
                if(obj == null)
                {
                    obj = new GameObject(typeof(T).Name);
                    inst = obj.AddComponent<T>();
                }
                else
                {
                    inst = obj.GetComponent<T>();
                }
            }
            return inst;
        }
    }

    protected abstract void Init();
}
