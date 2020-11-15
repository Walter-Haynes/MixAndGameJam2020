using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementScript : MonoBehaviour
{
    public void StartGame() { 
        // Debug.Log("New Scene");
        SceneManager.LoadScene(1);
    }
}
