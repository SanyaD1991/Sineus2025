using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class TriggerZoneUI : MonoBehaviour
{
    [Header("🎯 Основное")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string groundItemTag = "GroundItem";

    [Header("🪟 Ссылки на UI")]
    [SerializeField] private GameObject panel;     // Панель UI (например, "Press E to interact")
    [SerializeField] private TMP_Text messageText; // Текстовое поле внутри панели

    [Header("💬 Текстовые сообщения")]
    [SerializeField] private string messageOnEnter = "Нажми [E] для взаимодействия";
    [SerializeField] private string messageOnExit = "";

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Игрок
        if (other.CompareTag(playerTag))
        {
            ShowPanel(messageOnEnter);
            return;
        }

        // GroundItem
        if (other.CompareTag(groundItemTag))
        {
            DroppedResource dropped = other.GetComponent<DroppedResource>();
            if (dropped != null)
            {
                string msg = $"Подобрать ?";
                ShowPanel(msg);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Игрок
        if (other.CompareTag(playerTag))
        {
            HidePanel();
            return;
        }

        // GroundItem
        if (other.CompareTag(groundItemTag))
        {
            HidePanel();
        }
    }

    private void ShowPanel(string msg)
    {
        if (panel != null)
            panel.SetActive(true);

        if (messageText != null)
            messageText.text = msg;
    }

    private void HidePanel()
    {
        if (panel != null)
            panel.SetActive(false);

        if (messageText != null && !string.IsNullOrEmpty(messageOnExit))
            messageText.text = messageOnExit;
    }
}
