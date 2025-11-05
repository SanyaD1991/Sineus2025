using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;      // Панель проигрыша
    public Button retryButton;    // Кнопка повторить
    public Button menuButton;     // Кнопка главное меню

    private void Awake()
    {
        panel.SetActive(false);   // Панель скрыта по умолчанию

        retryButton.onClick.AddListener(OnRetry);
        menuButton.onClick.AddListener(OnMenu);
    }

    // Показать панель проигрыша
    public void Show()
    {
        panel.SetActive(true);
    }

    private void OnRetry()
    {
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnMenu()
    {
        // Загружаем сцену главного меню (укажите нужное имя сцены)
        SceneManager.LoadScene("MainMenu");
    }
}
