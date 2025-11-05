using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Зона подбора ресурсов с земли (по кнопке E или автоматически).
/// Подбирает только объекты с тегом "GroundItem".
/// Поддерживает UI и события для левел-дизайнера.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PickupZone : MonoBehaviour
{
    [Header("Подбор ресурсов")]
    [Tooltip("Если включено, требуется нажатие кнопки для подбора. Иначе подбирает автоматически.")]
    public bool pickUpOnKey = true;

    [Header("Настройки клавиши (Input System)")]
    public Key pickupKey = Key.E;

    [Header("События для левел-дизайнера")]
    public UnityEvent<DroppedResource> onResourceDetected; // вызывается при входе в триггер
    public UnityEvent<DroppedResource> onResourcePicked;   // вызывается при подборе ресурса
    public UnityEvent onResourceLost;                      // вызывается при выходе из зоны

    private PlayerCarry playerCarry;
    private bool canPickUp = true;
    private float pickCooldown = 0.25f;

    private void Awake()
    {
        playerCarry = GetComponentInParent<PlayerCarry>();
        if (playerCarry == null)
            Debug.LogWarning("⚠️ PlayerCarrySystem не найден у родителя!");

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerCarry == null) return;
        if (!other.CompareTag("GroundItem")) return;

        DroppedResource dropped = other.GetComponent<DroppedResource>();
        if (dropped == null) return;

        onResourceDetected?.Invoke(dropped);

        if (!pickUpOnKey)
            TryPickup(dropped);
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerCarry == null || !pickUpOnKey) return;
        if (!other.CompareTag("GroundItem")) return;

        DroppedResource dropped = other.GetComponent<DroppedResource>();
        if (dropped == null) return;

        onResourceDetected?.Invoke(dropped);

        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard[pickupKey].wasPressedThisFrame)
            TryPickup(dropped);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("GroundItem")) return;

        DroppedResource dropped = other.GetComponent<DroppedResource>();
        if (dropped == null) return;

        onResourceLost?.Invoke(); // ⚙️ сообщаем, что ресурс ушёл из зоны
    }

    private void TryPickup(DroppedResource dropped)
    {
        if (!canPickUp || dropped == null) return;

        if (playerCarry.IsFull)
        {
            Debug.Log("🎒 Рюкзак полон!");
            return;
        }

        canPickUp = false;
        playerCarry.PickupDropped(dropped);

        onResourcePicked?.Invoke(dropped); // ⚙️ вызываем эвент

        StartCoroutine(ResetPickupCooldown());
    }

    private System.Collections.IEnumerator ResetPickupCooldown()
    {
        yield return new WaitForSeconds(pickCooldown);
        canPickUp = true;
    }
}
