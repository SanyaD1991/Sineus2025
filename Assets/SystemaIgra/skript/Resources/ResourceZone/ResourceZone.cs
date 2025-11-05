using UnityEngine;

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceZone : MonoBehaviour, IActionZone
{
    public enum ZoneType { Take, Deposit }

    [Header("Настройка зоны")]
    [SerializeField] private Shelf3D shelf;
    [SerializeField] private ZoneType zoneType;
    [SerializeField] private int resourceAmount = 1;

    private ResourceInventoryMono globalInventory;

    private void Start()
    {
        globalInventory = ResourceInventoryMono.Instance;
        if (globalInventory == null)
            Debug.LogError("❌ ResourceInventoryMono.Instance не найден на сцене!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerActionController>();
            if (controller != null)
                controller.SetZone(this, gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerActionController>();
            if (controller != null)
                controller.ClearZone(this, gameObject);
        }
    }

    // Интерфейсные методы
    public void OnEnter(GameObject player)
    {
        Debug.Log($"🟡 Игрок вошёл в зону {zoneType}");
    }

    public void OnExit(GameObject player)
    {
        Debug.Log($"⚫ Игрок вышел из зоны {zoneType}");
    }

    public void Interact(GameObject player)
    {
        var carry = player.GetComponent<PlayerCarry>();
        if (carry == null) return;

        switch (zoneType)
        {
            case ZoneType.Take:
                TryTake(carry);
                break;
            case ZoneType.Deposit:
                TryDeposit(carry);
                break;
        }
    }

    private void TryTake(PlayerCarry player)
    {
        if (shelf == null || shelf.ShelfResource == null) return;

        int available = globalInventory.Inventory.Get(shelf.ShelfResource);
        if (available <= 0 || player.IsFull) return;

        int amount = Mathf.Min(resourceAmount, shelf.CurrentAmount, player.RemainingCapacity, available);

        shelf.TakeResource(amount);
        player.AddResource(shelf.ShelfResource, amount);
        globalInventory.Inventory.TrySpend(shelf.ShelfResource, amount);

        Debug.Log($"🟢 Игрок взял {amount} {shelf.ShelfResource.name}");
    }

    private void TryDeposit(PlayerCarry player)
    {
        if (shelf == null || shelf.ShelfResource == null) return;
        if (!player.HasResource(shelf.ShelfResource, resourceAmount))
            return;

        int amount = Mathf.Min(resourceAmount, player.GetAmount(shelf.ShelfResource));

        player.RemoveResource(shelf.ShelfResource, amount);
        shelf.AddResource(amount);
        globalInventory.Inventory.Add(shelf.ShelfResource, amount);

        Debug.Log($"🔵 Игрок положил {amount} {shelf.ShelfResource.name}");
    }
}
