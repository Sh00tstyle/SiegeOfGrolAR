using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;

    public static T Instance 
    {
        get 
        {
            if (_Instance == null)
            {
                T potentialInstance = FindObjectOfType<T>();

                if(potentialInstance != null)
                {
                    _Instance = potentialInstance;
                    DontDestroyOnLoad(_Instance.gameObject);
                }
                else
                {
                    Debug.LogError("The singleton of " + typeof(T) + " was null and could not be found. Make sure the script is present on a GameObject in your scene!");
                }
            }

            return _Instance;
        }
    }

    public static bool IsInitialized 
    {
        get 
        {
            return _Instance != null;
        }
    }

    /**
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

            Destroy(this); // Note: This destroys only the script component and not the entire gameobject
        }
    }
    /**/

    protected virtual void Initialize()
    {
        // Can be removed
    }
    private void Awake()
    {
        Initialize(); // Can be removed
    }

}