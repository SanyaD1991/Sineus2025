using System.Collections.Generic;
using UnityEngine;

public class Shelf3D : MonoBehaviour
{
    [Header("Настройка полки")]
    [SerializeField] private ResourceType shelfResource;
    [SerializeField] private int maxBoxesPerShelf = 10;      // макс. коробок на полке
    [SerializeField] private int columnsPerRow = 5;          // кол-во коробок в ряду
    [SerializeField] private Transform boxContainer;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float spacing = 1.2f;           // расстояние между коробками
    [SerializeField] private float shelfOffset = 2f;         // расстояние между полками
    [SerializeField] private Shelf3D shelfPrefab;            // префаб самой полки

    private List<ResourceBox3D> boxes = new List<ResourceBox3D>();
    private List<Shelf3D> shelves = new List<Shelf3D>();

    public bool IsFull => boxes.Count >= maxBoxesPerShelf;
    public ResourceType ShelfResource => shelfResource;
    public int CurrentAmount => boxes.Count;
    public List<Shelf3D> Shelves => shelves;

    private void Start()
    {
        if (shelves.Count == 0)
            shelves.Add(this);
    }

    public void FillFromInventory(ResourceInventoryMono inventory)
    {
        if (shelfResource == null || inventory == null) return;

        int available = inventory.Inventory.Get(shelfResource);
        int remaining = available;

        // Заполняем существующие полки
        foreach (var shelf in shelves)
        {
            int canPlace = Mathf.Min(remaining, shelf.maxBoxesPerShelf);
            shelf.SetAmount(canPlace);
            remaining -= canPlace;
            if (remaining <= 0) return;
        }

        // Создаем новые полки при необходимости
        while (remaining > 0)
        {
            var newShelf = SpawnNextShelf();
            int canPlace = Mathf.Min(remaining, newShelf.maxBoxesPerShelf);
            newShelf.SetAmount(canPlace);
            remaining -= canPlace;
        }
    }

    private Shelf3D SpawnNextShelf()
    {
        Vector3 newPos = transform.position + Vector3.forward * shelves.Count * shelfOffset;
        var newShelf = Instantiate(shelfPrefab, newPos, transform.rotation, transform.parent);
        newShelf.Setup(shelfResource, maxBoxesPerShelf, columnsPerRow);
        shelves.Add(newShelf);
        return newShelf;
    }

    public void Setup(ResourceType type, int maxBoxes, int columns = 5)
    {
        shelfResource = type;
        maxBoxesPerShelf = maxBoxes;
        columnsPerRow = columns;
    }

    public void TakeResource(int amount)
    {
        int toRemove = Mathf.Min(amount, boxes.Count);
        for (int i = 0; i < toRemove; i++)
        {
            var box = boxes[boxes.Count - 1];
            boxes.RemoveAt(boxes.Count - 1);
            Destroy(box.gameObject);
        }
    }

    public void AddResource(int amount)
    {
        SetAmount(boxes.Count + amount);
    }

    public void SetAmount(int amount)
    {
        amount = Mathf.Clamp(amount, 0, maxBoxesPerShelf);

        // Удаляем лишние коробки
        while (boxes.Count > amount)
        {
            var box = boxes[boxes.Count - 1];
            boxes.RemoveAt(boxes.Count - 1);
            Destroy(box.gameObject);
        }

        // Добавляем недостающие коробки
        while (boxes.Count < amount)
        {
            int index = boxes.Count;
            int row = index / columnsPerRow;     // ряд сверху вниз
            int col = index % columnsPerRow;     // колонка слева направо

            Vector3 pos = boxContainer.position
                        + Vector3.right * col * spacing
                        + Vector3.up * row * spacing;

            var go = Instantiate(boxPrefab, pos, Quaternion.identity, boxContainer);
            var box = go.GetComponent<ResourceBox3D>();
            box.Setup(shelfResource, 1);
            boxes.Add(box);
        }
    }
}
