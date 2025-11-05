using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResourcePanel : MonoBehaviour
{
    [SerializeField] private Transform contentParent; // Контейнер на Canvas
    [SerializeField] private GameObject resourceUIPrefab; // Префаб с иконкой и текстом

    [SerializeField] private ResourceInventoryMono inventoryMono;
    private readonly Dictionary<ResourceType, TMP_Text> resourceTexts = new();
    
    private void OnEnable()
    {
        if (inventoryMono == null || inventoryMono.Inventory == null)
        {
            Debug.LogError("❌ UIResourcePanel не инициализирован (inventoryMono или Inventory = null)");
            return;
        }

        inventoryMono.Inventory.OnResourceChanged += UpdateResourceUI;
        CreateInitialUI();
    }

    private void OnDisable()
    {
        if (inventoryMono?.Inventory != null)
            inventoryMono.Inventory.OnResourceChanged -= UpdateResourceUI;
    }

    private void CreateInitialUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        resourceTexts.Clear();

        foreach (var entry in inventoryMono.Inventory.GetAllResources())
        {
            GameObject obj = Instantiate(resourceUIPrefab, contentParent);
            obj.transform.Find("icon").GetComponent<Image>().sprite = entry.type.icon;
            TMP_Text text = obj.transform.Find("AmountText").GetComponent<TMP_Text>();
            text.text = entry.amount.ToString();
            resourceTexts.Add(entry.type, text);
        }
    }

    private void UpdateResourceUI(ResourceType type, int newAmount)
    {
        if (!resourceTexts.TryGetValue(type, out TMP_Text text))
            return;

        text.text = newAmount.ToString();
    }
}
