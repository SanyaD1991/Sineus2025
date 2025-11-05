using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject-инвентарь ресурсов.
/// Хранит реальные данные, общие между сценами.
/// </summary>
[CreateAssetMenu(menuName = "Game/Resource Inventory")]
public class ResourceInventory : ScriptableObject
{
    [Serializable]
    public class ResourceEntry
    {
        public ResourceType type;
        public int amount;
    }

    [SerializeField] private List<ResourceEntry> resources = new();

    public event Action<ResourceType, int> OnResourceChanged;

    public int Get(ResourceType type)
    {
        var entry = resources.Find(e => e.type == type);
        return entry != null ? entry.amount : 0;
    }

    public void Add(ResourceType type, int amount)
    {
        if (type == null) return;

        var entry = resources.Find(e => e.type == type);
        if (entry == null)
        {
            entry = new ResourceEntry { type = type, amount = 0 };
            resources.Add(entry);
        }

        entry.amount += amount;
        OnResourceChanged?.Invoke(type, entry.amount);
    }

    public bool TrySpend(ResourceType type, int amount)
    {
        var entry = resources.Find(e => e.type == type);
        if (entry == null || entry.amount < amount)
            return false;

        entry.amount -= amount;
        OnResourceChanged?.Invoke(type, entry.amount);
        return true;
    }

    public void ClearAll()
    {
        foreach (var entry in GetAllResources())
        {
            entry.amount = 0;
            OnResourceChanged?.Invoke(entry.type, 0);
        }
    }

    public void SetResource(ResourceType type, int amount)
    {
        var entry = resources.Find(e => e.type == type);
        if (entry == null)
        {
            entry = new ResourceEntry { type = type, amount = amount };
            resources.Add(entry);
        }
        else
        {
            entry.amount = amount;
        }
        OnResourceChanged?.Invoke(type, amount);
    }

    public List<ResourceEntry> GetAllResources() => resources;
}
