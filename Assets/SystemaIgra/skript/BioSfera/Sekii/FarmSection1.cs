using UnityEngine;
using System.Collections.Generic;

// Интерфейс для секций
public class Section : MonoBehaviour, ISectionDataProvider
{
    [Header("📦 Ресурсы на складе")]
    public List<ResourceStack> storedResources = new List<ResourceStack>();

    public SectionType SectionType => SectionType.Farm;

    public string GetSectionTitle() => "Cклад";

    // Основная информация
    public string GetMainInfo()
    {
        string info = "Склад:\n";

        if (storedResources.Count == 0)
        {
            info += "  — пусто\n";
        }
        else
        {
            foreach (var res in storedResources)
            {
                info += $"  {res.resource.name}: {res.amount:F1}\n";
            }
        }

        return info;
    }

    // Уровень тревоги (например, если какой-то ресурс превышает лимит)
    public float GetAlertLevel()
    {
        foreach (var res in storedResources)
        {
            if (res.amount > res.alertThreshold)
                return 1f;
        }
        return 0f;
    }
}

// Класс для хранения ресурса на складе
[System.Serializable]
public class ResourceStack
{
    public ResourceType resource;      // Тип ресурса
    public float amount;               // Количество
    public float alertThreshold = 50f; // Порог тревоги (для тревог, если нужно)
}
