using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject playMenu;
    void Start()
    {
        optionsMenu.SetActive(false);
        playMenu.SetActive(false);
    }
    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
