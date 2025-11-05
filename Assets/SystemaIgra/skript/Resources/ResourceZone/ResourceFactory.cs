using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UniversalResourceFactory : MonoBehaviour, IActionZone
{
    [System.Serializable]
    public class InputRequirement
    {
        public ResourceType resource;
        public int amountRequired = 1;
        [HideInInspector] public int amountDelivered = 0;

        public bool IsComplete => amountDelivered >= amountRequired;
        public int Remaining => Mathf.Max(0, amountRequired - amountDelivered);
    }

    [System.Serializable]
    public class OutputProduct
    {
        public ResourceType resource;
        public int amountProduced = 1;
    }

    [Header("🧱 Входные ресурсы (требуются для запуска)")]
    public List<InputRequirement> inputs = new List<InputRequirement>();

    [Header("⚙️ Выходные ресурсы (что производится)")]
    public List<OutputProduct> outputs = new List<OutputProduct>();

    [Header("⏳ Время производства (сек)")]
    public float produceTime = 5f;

    [Header("📦 Склад для вывода (опционально)")]
    public Shelf3D targetShelf;

    [Header("🧠 UI")]
    public TextMeshPro infoText;

    [Header("🎬 События для левел-дизайнера")]
    public UnityEvent onProductionStarted;
    public UnityEvent onProductionCompleted;
    public UnityEvent onResourceDelivered;
    public UnityEvent onQueueAdded;

    private PlayerCarry player;
    private Warehouse3D warehouse;
    private bool isProducing = false;

    private void Awake()
    {
        warehouse = FindObjectOfType<Warehouse3D>();
        UpdateInfoText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerActionController controller = other.GetComponent<PlayerActionController>();
        if (controller != null)
            controller.SetZone(this, gameObject);

        player = other.GetComponent<PlayerCarry>();
        UpdateInfoText();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerActionController controller = other.GetComponent<PlayerActionController>();
        if (controller != null)
            controller.ClearZone(this, gameObject);

        player = null;
        UpdateInfoText("");
    }

    public void Interact(GameObject playerObj)
    {
        if (player == null)
            player = playerObj.GetComponent<PlayerCarry>();
        if (player == null) return;

        TryDeliverResource();
    }

    private void TryDeliverResource()
    {
        // Проверяем, какой ресурс несёт игрок
        foreach (var input in inputs)
        {
            if (player.HasResource(input.resource, 1) && input.amountDelivered < input.amountRequired)
            {
                int deliver = Mathf.Min(input.amountRequired - input.amountDelivered, player.GetAmount(input.resource));

                player.RemoveResource(input.resource, deliver);
                input.amountDelivered += deliver;
                onQueueAdded?.Invoke();
                UpdateInfoText($"✅ Доставлено {deliver}× {input.resource.name}");
                break;
            }
        }

        if (AllInputsReady() && !isProducing)
            StartCoroutine(ProductionRoutine());
        else
            UpdateInfoText(GetInputStatus());
    }

    private bool AllInputsReady()
    {
        foreach (var input in inputs)
        {
            if (!input.IsComplete)
                return false;
        }
        return true;
    }

    private IEnumerator ProductionRoutine()
    {
        isProducing = true;
        onProductionStarted?.Invoke();

        float t = produceTime;
        while (t > 0)
        {
            UpdateInfoText($"⚙️ Производство: {t:F1} сек");
            t -= Time.deltaTime;
            yield return null;
        }

        ProduceOutputs();

        // Сбрасываем входные ресурсы
        foreach (var input in inputs)
            input.amountDelivered = 0;

        isProducing = false;
        onProductionCompleted?.Invoke();
        UpdateInfoText("✅ Производство завершено");
    }

    private void ProduceOutputs()
    {
        foreach (var output in outputs)
        {
            Shelf3D shelf = targetShelf;

            if (shelf == null && warehouse != null)
            {
                foreach (var sh in warehouse.Shelves)
                {
                    if (sh.ShelfResource == output.resource && !sh.IsFull)
                    {
                        shelf = sh;
                        break;
                    }
                }
            }

            if (shelf != null && !shelf.IsFull)
                shelf.AddResource(output.amountProduced);

            if (ResourceInventoryMono.Instance != null)
                ResourceInventoryMono.Instance.Inventory.Add(output.resource, output.amountProduced);

            onResourceDelivered?.Invoke();
            Debug.Log($"✅ Произведено: {output.amountProduced}× {output.resource.name}");
        }
    }

    private string GetInputStatus()
    {
        string status = "📥 Требуется:\n";
        foreach (var input in inputs)
        {
            status += $"{input.resource.name}: {input.amountDelivered}/{input.amountRequired}\n";
        }
        return status;
    }

    private void UpdateInfoText(string txt = null)
    {
        if (infoText == null) return;

        if (string.IsNullOrEmpty(txt))
            infoText.text = GetInputStatus();
        else
            infoText.text = txt;
    }

    public void OnEnter(GameObject playerObj)
    {
        UpdateInfoText("🔹 [E] — внести ресурс");
    }

    public void OnExit(GameObject playerObj)
    {
        UpdateInfoText("");
    }
}
