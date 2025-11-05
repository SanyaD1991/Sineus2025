using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UniversalTrigger1 : MonoBehaviour
{
    [Header("Настройка")]
    public UnityEvent onInteract; // событие для подписки

    private PlayerCarry playerInZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = other.GetComponent<PlayerCarry>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = null;
    }

    // Этот метод можно вызвать из скрипта ввода
    public void Interact()
    {
        if (playerInZone != null)
            onInteract?.Invoke();
    }
}
