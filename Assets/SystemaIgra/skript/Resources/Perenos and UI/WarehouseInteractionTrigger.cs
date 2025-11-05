using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class WarehouseInteractionTrigger : MonoBehaviour
{
    public UnityEvent onOpenUI;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        onOpenUI?.Invoke();
    }
}
