using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;

    public static T Instance 
    {
        get 
        {
            if (_Instance == null)
                Debug.LogError("The singleton of " + typeof(T) + " was null");

            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }
        else
        {
            Debug.LogError("An instance of the singleton " + name + "already exists, destroying second instance");
            Destroy(this);
        }
    }

    protected virtual void Initialize()
    {
        //Use this in subclasses for initialization
    }
}