using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    public static bool IsInitialized() => Instance != null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            Instance.Init();
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 派生クラス用のAwake()
    /// </summary>
    protected virtual void Init()
    {

    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        OnRelease();
    }

    /// <summary>
    /// 派生クラス用のOnDestroy()
    /// </summary>
    protected virtual void OnRelease()
    {

    }
}
