using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UniversalResourceFactoryTimer : MonoBehaviour, IActionZone
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

    [Header("📥 Входные ресурсы (требуются для запуска)")]
    public List<InputRequirement> inputs = new List<InputRequirement>();

    [Header("⚙️ Выходные ресурсы (что производится)")]
    public List<OutputProduct> outputs = new List<OutputProduct>();

    [Header("⏳ Время производства (в секундах, по умолчанию 30 минут = 1800 сек)")]
    public float totalProductionTime = 1800f;

    [Header("📦 Склад (опционально)")]
    public Shelf3D targetShelf;

    [Header("🧠 UI")]
    public TextMeshPro infoText;
    public GameObject completionPanel; // Панель, вызываемая по завершению

    [Header("🎬 События")]
    public UnityEvent onProductionStarted;
    public UnityEvent onProductionCompleted;
    public UnityEvent onResourceDelivered;
    public UnityEvent onQueueAdded;

    private PlayerCarry player;
    private Warehouse3D warehouse;
    private bool isProducing = false;
    private float remainingTime;

    private void Awake()
    {
        warehouse = FindObjectOfType<Warehouse3D>();
        remainingTime = totalProductionTime;
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
            StartCoroutine(ProductionTimer());
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

    private IEnumerator ProductionTimer()
    {
        isProducing = true;
        remainingTime = totalProductionTime;
        onProductionStarted?.Invoke();

        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            UpdateInfoText($"⚙️ двигатель работает\n⏱ Осталось: {minutes:00}:{seconds:00}");
            yield return null;
        }

        CompleteProduction();
    }

    private void CompleteProduction()
    {
        isProducing = false;
        onProductionCompleted?.Invoke();

        ProduceOutputs();

        foreach (var input in inputs)
            input.amountDelivered = 0;

        UpdateInfoText("✅ топливо кончелось");

        if (completionPanel != null)
            completionPanel.SetActive(true);
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
            Debug.Log($"✅ конец: {output.amountProduced}× {output.resource.name}");
        }
    }

    private string GetInputStatus()
    {
        string status = "📥 Требуется:\n";
        foreach (var input in inputs)
            status += $"{input.resource.name}: {input.amountDelivered}/{input.amountRequired}\n";
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
        UpdateInfoText("🔹 [E] — внести ресурс и запустить двигатель работает ");
    }

    public void OnExit(GameObject playerObj)
    {
        UpdateInfoText("");
    }
}
