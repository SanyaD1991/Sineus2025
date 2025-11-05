using System.Collections.Generic;
using UnityEngine;

public class Warehouse3D : MonoBehaviour
{
    [Header("Настройки склада")]
    [SerializeField] private List<Shelf3D> shelves = new List<Shelf3D>();
    public IReadOnlyList<Shelf3D> Shelves => shelves; // ✅ вот это свойство нужно

    public GameObject shelfPrefab;
    public Transform shelfContainer;
    public int maxBoxesPerShelf = 10;
    public float shelfSpacing = 2f;

    [Header("Инвентарь")]
    public ResourceInventoryMono inventoryMono;

    private void Start()
    {
        if (inventoryMono == null)
            inventoryMono = ResourceInventoryMono.Instance;

        inventoryMono.Inventory.OnResourceChanged += OnResourceChanged;

        foreach (var entry in inventoryMono.Inventory.GetAllResources())
        {
            UpdateVisual(entry.type, entry.amount);
        }
    }

    private void OnResourceChanged(ResourceType type, int newAmount)
    {
        UpdateVisual(type, newAmount);
    }

    private void UpdateVisual(ResourceType type, int amount)
    {
        int remaining = amount;

        // ищем полку с нужным ресурсом
        foreach (var shelf in shelves)
        {
            if (shelf.ShelfResource == type)
            {
                shelf.SetAmount(amount);
                return;
            }
        }

        // если нет — создаём новую
        if (shelfPrefab != null)
        {
            Vector3 spawnPos = shelfContainer.position + Vector3.forward * shelves.Count * shelfSpacing;
            GameObject go = Instantiate(shelfPrefab, spawnPos, Quaternion.identity, shelfContainer);

            Shelf3D newShelf = go.GetComponent<Shelf3D>();
            newShelf.SetAmount(Mathf.Min(amount, maxBoxesPerShelf));

            shelves.Add(newShelf);
        }
    }
}
