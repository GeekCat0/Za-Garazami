using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Wczytuje scenê po nazwie (string)
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneName);
    }

    // Zamyka grê
    public void QuitGame()
    {
        Application.Quit();

        // To tylko ¿eby dzia³a³o w edytorze Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}