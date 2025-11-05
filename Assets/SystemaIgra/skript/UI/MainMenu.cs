using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    public Button startButton;
    public Button quitButton;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStart);
        quitButton.onClick.AddListener(OnQuit);
    }

    private void OnStart()
    {
        // Замените "GameScene" на имя вашей сцены с игрой
        SceneManager.LoadScene("SampleScene1");
    }

    private void OnQuit()
    {
        Application.Quit();
        Debug.Log("Игра закрыта");
    }
}
