using System.Collections.Generic;
using UnityEngine;

public class ResourceInventoryMono : MonoBehaviour
{
    public static ResourceInventoryMono Instance { get; private set; }

    [Header("Ссылка на глобальный инвентарь (ScriptableObject)")]
    [SerializeField] private ResourceInventory inventory;

    [Header("Отображение текущих значений (read-only)")]
    [SerializeField] private List<ResourceAmount> resourcesView = new List<ResourceAmount>();

    // ✅ Публичный доступ к ScriptableObject-инвентарю
    public ResourceInventory Inventory => inventory;

    private void Awake()
    {
        Instance = this;

        if (inventory == null)
        {
            Debug.LogWarning("⚠️ ResourceInventory не назначен в инспекторе!");
            return;
        }

        RefreshView();
        inventory.OnResourceChanged += OnResourceChanged;
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnResourceChanged -= OnResourceChanged;
    }

    private void OnResourceChanged(ResourceType type, int newAmount)
    {
        var entry = resourcesView.Find(r => r.type == type);
        if (entry != null)
            entry.amount = newAmount;
        else
            resourcesView.Add(new ResourceAmount { type = type, amount = newAmount });
    }

    private void RefreshView()
    {
        resourcesView.Clear();
        foreach (var entry in inventory.GetAllResources())
        {
            resourcesView.Add(new ResourceAmount { type = entry.type, amount = entry.amount });
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        RefreshView();
    }
#endif
}
