using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Универсальный UI-хэндлер для отображения панели и текста при нахождении/подборе ресурса.
/// Подключается к эвентам из PickupZone.
/// </summary>
public class ResourceUIHandler : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;

    [Header("Тексты сообщений")]
    [SerializeField] private string detectMessage = "Нажми [E] для подбора";
    [SerializeField] private string pickMessage = "Ресурс подобран!";
    [SerializeField] private string lostMessage = "";

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    /// <summary>
    /// Вызывается при нахождении ресурса с тегом "GroundItem"
    /// </summary>
    public void OnResourceDetected(DroppedResource dropped)
    {
        if (panel == null || messageText == null) return;

        panel.SetActive(true);
        messageText.text = $"{detectMessage}";
    }

    /// <summary>
    /// Вызывается при подборе ресурса
    /// </summary>
    public void OnResourcePicked(DroppedResource dropped)
    {
        if (panel == null || messageText == null) return;

        panel.SetActive(true);
        messageText.text = pickMessage;

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HidePanelAfterSeconds(1.5f));
    }

    /// <summary>
    /// Вызывается при потере ресурса из зоны
    /// </summary>
    public void OnResourceLost()
    {
        if (panel == null) return;

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);
        panel.SetActive(false);
    }

    private IEnumerator HidePanelAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (panel != null)
            panel.SetActive(false);
    }
}
