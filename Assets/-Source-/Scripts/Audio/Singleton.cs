using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Generic Singleton class.
/// </summary>
/// <typeparam name="T">Class to make a singleton instance of</typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    /// <summary>
    /// Retrieves singleton instance.
    /// If there is none, create a new gameobject and add class component.
    /// </summary>
    /// <value>Returns class instance</value>
    public static T Instance {
        get {
            if (_instance == null) {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent<T>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Remove instance when script is destroyed.
    /// </summary>
    private void OnDestroy() {
        if (_instance == this) {
            _instance = null;
        }
    }
}

/// <summary>
/// Generic Singleton class, persists through scenes.
/// If you have a different name for your manager scene,
/// be sure to change it in the getter.
/// </summary>
/// <typeparam name="T">Class to make a singleton instance of</typeparam>
public class SingletonPersistent<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    /// <summary>
    /// Retrieves singleton instance.
    /// If there is none, create a new gameobject and add class component.
    /// </summary>
    /// <value>Returns class instance</value>
    public static T Instance {
        get {
            if (_instance == null) {
                Scene activeScene = SceneManager.GetActiveScene();
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Managers"));
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                obj.hideFlags = HideFlags.HideAndDontSave;
                _instance = obj.AddComponent<T>();
                SceneManager.SetActiveScene(activeScene);
            }
            return _instance;
        }
    }

    /// <summary>
    /// Remove instance when script is destroyed.
    /// </summary>
    private void OnDestroy() {
        if (_instance == this) {
            _instance = null;
        }
    }

    /// <summary>
    /// Uncomment if you want to use DontDestroyOnLoad instead of additive scenes
    /// </summary>
    // public virtual void Awake() {
    //     if (_instance == null) {
    //         _instance = this as T;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else {
    //         Destroy(this);
    //     }
    // }
}