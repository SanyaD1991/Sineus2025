using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Система переноски ресурсов игроком.
/// Поддерживает добавление/удаление ресурсов, визуализацию над головой, бросок и подбор.
/// </summary>
public class PlayerCarry : MonoBehaviour
{
    [Header("Параметры переноски")]
    [SerializeField] private int maxCapacity = 2;

    [Header("UI отображение")]
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private Vector3 textOffset = new Vector3(0, 2f, 0);

    [Header("Визуализация ресурсов над головой")]
    [SerializeField] private Transform headPoint;
    [SerializeField] private float verticalOffset = 0.3f;
    [SerializeField] private float radiusSpread = 0.25f;
    [SerializeField] private List<ResourceVisual> resourceVisuals = new List<ResourceVisual>();

    [Header("Бросок ресурса")]
    [SerializeField] private float throwForce = 6f;
    [SerializeField] private float upwardForce = 3f;

    private Dictionary<ResourceType, int> carried = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, List<GameObject>> visualInstances = new Dictionary<ResourceType, List<GameObject>>();

    private PlayerInput playerInput;
    private InputAction throwAction;

    public bool IsFull => TotalCarried() >= maxCapacity;
    public int RemainingCapacity => maxCapacity - TotalCarried();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
            throwAction = playerInput.actions["Throw"];
    }
 

    private void OnEnable()
    {
        if (throwAction != null)
            throwAction.performed += OnThrowPerformed;
    }

    private void OnDisable()
    {
        if (throwAction != null)
            throwAction.performed -= OnThrowPerformed;
    }

    private void Start()
    {
        if (textMesh == null)
        {
            GameObject txt = new GameObject("CarryText");
            txt.transform.SetParent(transform);
            txt.transform.localPosition = textOffset;
            textMesh = txt.AddComponent<TextMeshPro>();
            textMesh.fontSize = 3;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.color = Color.yellow;
        }

        if (headPoint == null)
        {
            GameObject head = new GameObject("HeadPoint");
            head.transform.SetParent(transform);
            head.transform.localPosition = new Vector3(0, 1.8f, 0);
            headPoint = head.transform;
        }

        UpdateText();
        UpdateVisuals();
    }

    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        ThrowTopResource();
    }

    // Добавление ресурса в рюкзак
    public void AddResource(ResourceType type, int amount)
    {
        if (type == null || amount <= 0) return;

        if (!carried.ContainsKey(type))
            carried[type] = 0;

        int canAdd = Mathf.Min(amount, RemainingCapacity);
        carried[type] += canAdd;

        UpdateText();
        UpdateVisuals();
    }

    // Удаление ресурса из рюкзака
    public void RemoveResource(ResourceType type, int amount)
    {
        if (type == null || !carried.ContainsKey(type)) return;

        carried[type] -= amount;
        if (carried[type] <= 0)
            carried.Remove(type);

        UpdateText();
        UpdateVisuals();
    }

    public bool HasResource(ResourceType type, int amount = 1)
    {
        return type != null && carried.ContainsKey(type) && carried[type] >= amount;
    }

    public int GetAmount(ResourceType type)
    {
        return type != null && carried.ContainsKey(type) ? carried[type] : 0;
    }

    public Dictionary<ResourceType, int> GetAllCarried() => new Dictionary<ResourceType, int>(carried);

    private int TotalCarried()
    {
        int total = 0;
        foreach (var kvp in carried)
            total += kvp.Value;
        return total;
    }

    private void UpdateText()
    {
        if (textMesh == null) return;

        if (carried.Count == 0)
        {
            textMesh.text = "🎒 Пусто";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Рюкзак ({TotalCarried()}/{maxCapacity}):");
        foreach (var kvp in carried)
            sb.AppendLine($"{kvp.Key.name}: {kvp.Value}");

        textMesh.text = sb.ToString();
    }

    private void UpdateVisuals()
    {
        // Удаляем старые префабы
        foreach (var kvp in visualInstances)
        {
            foreach (var obj in kvp.Value)
                if (obj != null) Destroy(obj);
        }
        visualInstances.Clear();

        float currentHeight = 0f;

        foreach (var kvp in carried)
        {
            ResourceType type = kvp.Key;
            GameObject prefab = type.carryPrefab;

            // Если нет префаба в типе, ищем в ResourceVisual
            if (prefab == null)
            {
                ResourceVisual visual = resourceVisuals.Find(v => v.type == type);
                if (visual != null) prefab = visual.prefab;
            }

            if (prefab == null) continue;

            visualInstances[type] = new List<GameObject>();

            // Создаем ровную стопку
            for (int i = 0; i < kvp.Value; i++)
            {
                GameObject obj = Instantiate(prefab, headPoint);

                // Выравнивание строго по вертикали
                obj.transform.localPosition = new Vector3(0f, currentHeight, 0f);
                obj.transform.localRotation = Quaternion.identity; // ровно, без вращения

                visualInstances[type].Add(obj);

                currentHeight += verticalOffset; // следующая коробка выше
            }

            // Можно добавить небольшую паузу между разными типами ресурсов
            currentHeight += 0.1f;
        }
    }

    // Бросок верхнего ресурса
    private void ThrowTopResource()
    {
        if (carried.Count == 0) return;

        ResourceType type = null;
        foreach (var kvp in carried) type = kvp.Key; // берём последний
        if (type == null) return;

        RemoveResource(type, 1);

        GameObject prefab = type.carryPrefab;
        if (prefab == null)
        {
            ResourceVisual visual = resourceVisuals.Find(v => v.type == type);
            if (visual != null) prefab = visual.prefab;
        }
        if (prefab == null) return;

        Vector3 spawnPos = headPoint.position + transform.forward * 0.5f;
        GameObject dropped = Instantiate(prefab, spawnPos, Quaternion.identity);

        Rigidbody rb = dropped.AddComponent<Rigidbody>();
        if (dropped.GetComponent<Collider>() == null) dropped.AddComponent<SphereCollider>();

        DroppedResource drop = dropped.AddComponent<DroppedResource>();
        drop.resourceType = type;
        drop.amount = 1;

        // Автоматически присваиваем тег GroundItem
        dropped.tag = "GroundItem";

        Vector3 force = transform.forward * throwForce + Vector3.up * upwardForce;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);
    }

    // Подбор ресурса с земли
    public void PickupDropped(DroppedResource dropped)
    {
        if (dropped == null) return;
        AddResource(dropped.resourceType, dropped.amount);
        Destroy(dropped.gameObject);
    }
}

[System.Serializable]
public class ResourceVisual
{
    public ResourceType type;
    public GameObject prefab;
}
