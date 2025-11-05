using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Отвечает за сохранение и загрузку данных инвентаря.
/// Работает с ResourceInventoryMono.Instance.Inventory.
/// Сохраняет только изменения ресурсов без очистки ScriptableObject.
/// </summary>
public class InventorySaveSystem : MonoBehaviour
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "inventory_save.json");

    [Header("Автосохранение при выходе из игры")]
    [SerializeField] private bool autoSaveOnQuit = true;

    private void Start()
    {
        LoadInventory();
        // Подписка на события изменения ресурсов для автосохранения
        if (ResourceInventoryMono.Instance?.Inventory != null)
        {
            ResourceInventoryMono.Instance.Inventory.OnResourceChanged += OnResourceChanged;
        }
    }

    private void OnApplicationQuit()
    {
        if (autoSaveOnQuit)
            SaveInventory();
    }

    private void OnDestroy()
    {
        if (ResourceInventoryMono.Instance?.Inventory != null)
            ResourceInventoryMono.Instance.Inventory.OnResourceChanged -= OnResourceChanged;
    }

    /// <summary>
    /// Автосохранение при изменении ресурса
    /// </summary>
    private void OnResourceChanged(ResourceType type, int newAmount)
    {
        SaveInventory();
    }

    [ContextMenu("💾 Сохранить вручную")]
    public void SaveInventory()
    {
        var inventory = ResourceInventoryMono.Instance?.Inventory;
        if (inventory == null)
        {
            Debug.LogError("❌ ResourceInventoryMono.Instance не найден! Сохранение невозможно.");
            return;
        }

        var data = new InventoryData();
        foreach (var entry in inventory.GetAllResources())
        {
            data.resources.Add(new InventoryData.ResourceData
            {
                resourceTypeName = entry.type.name,
                amount = entry.amount
            });
        }

        string json = JsonUtility.ToJson(data, true);
        try
        {
            File.WriteAllText(SavePath, json);
            Debug.Log($"✅ Инвентарь сохранён: {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Ошибка при сохранении инвентаря: {e.Message}");
        }
    }

    [ContextMenu("📂 Загрузить вручную")]
    public void LoadInventory()
    {
        var inventory = ResourceInventoryMono.Instance?.Inventory;
        if (inventory == null)
        {
            Debug.LogError("❌ ResourceInventoryMono.Instance не найден! Загрузка невозможна.");
            return;
        }

        if (!File.Exists(SavePath))
        {
            Debug.Log("ℹ️ Файл сохранения не найден.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<InventoryData>(json);
            if (data == null || data.resources == null)
                return;

            // НЕ очищаем текущий ScriptableObject, просто применяем значения
            foreach (var entry in data.resources)
            {
                var type = Resources.Load<ResourceType>($"ResourcesTypes/{entry.resourceTypeName}");
                if (type != null)
                    inventory.SetResource(type, entry.amount);
                else
                    Debug.LogWarning($"⚠️ ResourceType '{entry.resourceTypeName}' не найден в Resources/ResourcesTypes/");
            }

            Debug.Log("✅ Инвентарь успешно загружен без очистки ScriptableObject.");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Ошибка при загрузке инвентаря: {e.Message}");
        }
    }
}

[Serializable]
public class InventoryData
{
    [Serializable]
    public class ResourceData
    {
        public string resourceTypeName;
        public int amount;
    }

    public List<ResourceData> resources = new();
}
