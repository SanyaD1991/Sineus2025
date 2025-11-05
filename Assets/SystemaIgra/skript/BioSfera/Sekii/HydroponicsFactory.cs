using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UniversalResourceFactory1 : MonoBehaviour, IActionZone
{
    [System.Serializable]
    public class InputRequirement
    {
        public ResourceType resource;
        public int amountRequired = 1;
        [HideInInspector] public int amountDelivered = 0;

        public bool IsComplete => amountDelivered > 0; // Для авто-производства хватит 1 ресурса
    }

    [System.Serializable]
    public class OutputProduct
    {
        public ResourceType resource;
        public int amountProduced = 1;
    }

    [Header("🧱 Входные ресурсы")]
    public List<InputRequirement> inputs = new List<InputRequirement>();

    [Header("⚙️ Выходные ресурсы")]
    public List<OutputProduct> outputs = new List<OutputProduct>();

    [Header("⏳ Время цикла производства (сек)")]
    public float produceInterval = 10f;

    [Header("💧⚡ Гидропоника")]
    public float water = 100f;
    public float light = 100f;
    public float dropMin = 2f;
    public float dropMax = 8f;

    [Header("📦 Склад")]
    public Shelf3D targetShelf;

    [Header("🧠 UI")]
    public TextMeshPro infoText;

    [Header("🎬 События")]
    public UnityEvent onProductionStarted;
    public UnityEvent onProductionCompleted;
    public UnityEvent onResourceDelivered;
    public UnityEvent onQueueAdded;

    [Header("⚡ Авто-производство")]
    public bool autoProduce = true;
    public float resourceDuration = 60f; // Один ресурс = 60 сек
    private int productionCyclesPerResource;

    private PlayerCarry player;
    private Warehouse3D warehouse;
    private bool isProducing = false;
    private bool isPaused = false;

    private void Awake()
    {
        warehouse = FindObjectOfType<Warehouse3D>();
        UpdateInfoText();
        productionCyclesPerResource = Mathf.CeilToInt(resourceDuration / produceInterval);

        if (autoProduce)
            StartCoroutine(AutoProduceLoop());
    }

    private IEnumerator AutoProduceLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(produceInterval);

            if (!isProducing && !isPaused)
            {
                foreach (var input in inputs)
                {
                    if (player != null && player.HasResource(input.resource, 1))
                    {
                        player.RemoveResource(input.resource, 1);
                        input.amountDelivered = productionCyclesPerResource;
                        onQueueAdded?.Invoke();
                    }
                }
                TryStartProduction();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = other.GetComponent<PlayerCarry>();
        UpdateInfoText();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = null;
        UpdateInfoText("");
    }

    public void Interact(GameObject playerObj)
    {
        if (player == null)
            player = playerObj.GetComponent<PlayerCarry>();
        if (player == null) return;

        if (isPaused)
        {
            water = 100f;
            light = 100f;
            isPaused = false;
            UpdateInfoText("🔧 Станция восстановлена");
            TryStartProduction();
            return;
        }

        // Доставляем ресурс вручную
        foreach (var input in inputs)
        {
            if (player.HasResource(input.resource, 1))
            {
                player.RemoveResource(input.resource, 1);
                input.amountDelivered = productionCyclesPerResource;
                onQueueAdded?.Invoke();
            }
        }
        TryStartProduction();
    }

    private void TryStartProduction()
    {
        if (!isProducing && !isPaused)
        {
            foreach (var input in inputs)
                if (input.IsComplete)
                {
                    StartCoroutine(ProductionRoutine());
                    break;
                }
        }
    }

    private IEnumerator ProductionRoutine()
    {
        isProducing = true;
        onProductionStarted?.Invoke();

        while (true)
        {
            bool hasResource = false;
            foreach (var input in inputs)
            {
                if (input.amountDelivered > 0)
                {
                    input.amountDelivered--;
                    hasResource = true;
                }
            }

            if (!hasResource || water <= 0f || light <= 0f)
            {
                isPaused = true;
                isProducing = false;
                UpdateInfoText("⚠️ Вода/свет закончились или ресурс исчерпан\n[E] — восстановить станцию");
                yield break;
            }

            ProduceOutputs();

            water -= Random.Range(dropMin, dropMax);
            light -= Random.Range(dropMin, dropMax);
            water = Mathf.Clamp(water, 0f, 100f);
            light = Mathf.Clamp(light, 0f, 100f);

            UpdateInfoText($"⚙️ Производство | 💧 {water:F0} | ☀️ {light:F0} | ⏱ Остаток ресурсов: {GetRemainingResourceCycles()}");

            yield return new WaitForSeconds(produceInterval);
        }
    }

    private void ProduceOutputs()
    {
        foreach (var output in outputs)
        {
            Shelf3D shelf = targetShelf;
            if (shelf == null && warehouse != null)
            {
                foreach (var sh in warehouse.Shelves)
                    if (sh.ShelfResource == output.resource && !sh.IsFull)
                    {
                        shelf = sh;
                        break;
                    }
            }

            if (shelf != null && !shelf.IsFull)
                shelf.AddResource(output.amountProduced);

            if (ResourceInventoryMono.Instance != null)
                ResourceInventoryMono.Instance.Inventory.Add(output.resource, output.amountProduced);

            onResourceDelivered?.Invoke();
        }
    }

    private int GetRemainingResourceCycles()
    {
        int min = int.MaxValue;
        foreach (var input in inputs)
            min = Mathf.Min(min, input.amountDelivered);
        return min == int.MaxValue ? 0 : min;
    }

    private void UpdateInfoText(string txt = null)
    {
        if (infoText == null) return;

        if (!string.IsNullOrEmpty(txt))
            infoText.text = txt;
        else
        {
            string baseText = "";
            foreach (var input in inputs)
                baseText += $"📥 {input.resource.name}: {input.amountDelivered}/{input.amountRequired}\n";

            if (isProducing)
                baseText += $"💧 {water:F0} | ☀️ {light:F0}";
            else if (isPaused)
                baseText += "\n⚠️ Вода или свет закончились\n[E] — восстановить";

            infoText.text = baseText;
        }
    }

    public void OnEnter(GameObject playerObj) => UpdateInfoText("🔹 [E] — внести ресурс");
    public void OnExit(GameObject playerObj) => UpdateInfoText("");
}
