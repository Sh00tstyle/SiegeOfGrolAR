using UnityEngine;

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _Instance;

    protected void SetDontDestroyOnLoad()
    {
        if (_Instance != null)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

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
                }
                else
                {
                    Debug.LogWarning("Singleton<" + typeof(T) + ">::The singleton instance was null and could not be found. Make sure the script is present on a GameObject in your scene!");
                }
            }

            return _Instance;
        }
    }

    public static bool IsInitialized 
    {
        get 
        {
            return Instance != null;
        }
    }
}