using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UniversalTrigger : MonoBehaviour
{
    public enum InteractionType { Warehouse, CraftZone, Other }

    [Header("Настройки")]
    public InteractionType type;
    public GameObject uiPanel; // Меню взаимодействия
    private bool isPlayerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            if (uiPanel) uiPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (uiPanel) uiPanel.SetActive(false);
        }
    }

    public bool IsPlayerInside() => isPlayerInside;
}
