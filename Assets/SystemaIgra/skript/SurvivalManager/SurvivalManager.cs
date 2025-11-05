using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Настраиваемый ресурс для выживания
[System.Serializable]
public class SurvivalResource
{
    public ResourceType resourceType;   // Тип ресурса из Inventory
    public float consumeRate = 1f;      // Сколько единиц съедается за один цикл
}

public class SurvivalManager : MonoBehaviour
{
    public static SurvivalManager Instance { get; private set; }
    [Header("UI")]
    public GameObject gameOverPanel; // Привязать панель в инспекторе
    [Header("Экипаж")]
    public CrewMember[] crewMembers;

    [Header("Настраиваемые ресурсы")]
    public List<SurvivalResource> survivalResources = new List<SurvivalResource>();

    [Header("Настройка цикла потребления")]
    public float consumeInterval = 5f; // каждые N секунд потребляем ресурсы

    public ResourceInventoryMono inventoryMono;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
 
    }

    private void Start()
    {
        StartCoroutine(SurvivalCycle());
    }

    private IEnumerator SurvivalCycle()
    {
        while (!IsGameOver)
        {
            yield return new WaitForSeconds(consumeInterval);
            ConsumeResources();
            CheckLossConditions();
        }
    }

    private void ConsumeResources()
    {
        if (inventoryMono == null) return;

        foreach (var crew in crewMembers)
        {
            if (!crew.IsAlive) continue;

            foreach (var res in survivalResources)
            {
                int available = inventoryMono.Inventory.Get(res.resourceType);
                if (available >= res.consumeRate)
                {
                    inventoryMono.Inventory.TrySpend(res.resourceType, (int)res.consumeRate);
                }
                else
                {
                    Debug.Log($"⚠️ {crew.name} не хватает {res.resourceType.name}!");
                    // Недостаточно ресурса — наносим урон
                    crew.TakeDamage(10f); // например, 10 HP за отсутствие ресурса
                     
                }
            }
        }
    }

    private void CheckLossConditions()
    {
        bool allDead = true;
        foreach (var crew in crewMembers)
        {
            if (crew.IsAlive)
                allDead = false;
        }

        if (allDead)
            GameOver("Все члены экипажа погибли.");
    }

    private void GameOver(string reason)
    {
        IsGameOver = true;
        Debug.Log($"💀 GAME OVER: {reason}");
        gameOverPanel.SetActive(true); ;
        // TODO: показать экран проигрыша
    }
}
