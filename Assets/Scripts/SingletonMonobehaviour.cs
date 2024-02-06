using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    var gameObject = new GameObject($"Instance of {typeof(T)}");
                    _instance = gameObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}