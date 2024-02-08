using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    static bool appIsQuitting = false;
    public static T Instance {
        get {

            if (appIsQuitting) return null;

            if (_instance == null) {
                _instance = FindObjectOfType<T>();

                if (_instance == null) {
                    var gameObject = new GameObject($"Instance of {typeof(T)}");
                    _instance = gameObject.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    public void OnDestroy() {
        appIsQuitting = true;
    }
}