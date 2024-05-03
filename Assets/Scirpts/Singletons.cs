using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletons<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Singleton;
    protected virtual void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this as T;
        }
        else
            Destroy(gameObject);
    }
}
