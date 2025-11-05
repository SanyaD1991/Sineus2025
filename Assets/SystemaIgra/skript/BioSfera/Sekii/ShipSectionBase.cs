using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class ShipSectionBase : MonoBehaviour, IActionZone
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
    public List<InputRequirement> inputs = new();

    [Header("⚙️ Выходные ресурсы (что производится)")]
    public List<OutputProduct> outputs = new();

    [Header("🕒 Время производства (сек)")]
    public float productionTime = 10f;

    [Header("📊 UI")]
    public TextMeshPro infoText;

    [Header("🎬 События")]
    public UnityEvent onProductionStart;
    public UnityEvent onProductionComplete;
    public UnityEvent onResourceDelivered;

    protected bool isProducing;
    protected PlayerCarry player;
    protected Warehouse3D warehouse;

    protected virtual void Awake()
    {
        warehouse = FindObjectOfType<Warehouse3D>();
        UpdateInfoText();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = other.GetComponent<PlayerCarry>();

        PlayerActionController controller = other.GetComponent<PlayerActionController>();
        if (controller != null)
            controller.SetZone(this, gameObject);

        UpdateInfoText();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        player = null;

        PlayerActionController controller = other.GetComponent<PlayerActionController>();
        if (controller != null)
            controller.ClearZone(this, gameObject);

        UpdateInfoText("");
    }

    public virtual void Interact(GameObject playerObj)
    {
        if (player == null)
            player = playerObj.GetComponent<PlayerCarry>();
        if (player == null) return;

        TryDeliverResource();
    }

    protected virtual void TryDeliverResource()
    {
        foreach (var input in inputs)
        {
            if (player.HasResource(input.resource, 1) && input.amountDelivered < input.amountRequired)
            {
                int deliver = Mathf.Min(input.amountRequired - input.amountDelivered, player.GetAmount(input.resource));
                player.RemoveResource(input.resource, deliver);
                input.amountDelivered += deliver;

                onResourceDelivered?.Invoke();
                UpdateInfoText($"✅ Доставлено {deliver}× {input.resource.name}");
                break;
            }
        }

        if (AllInputsReady() && !isProducing)
            StartCoroutine(ProductionRoutine());
        else
            UpdateInfoText(GetInputStatus());
    }

    protected bool AllInputsReady()
    {
        foreach (var i in inputs)
            if (!i.IsComplete) return false;
        return true;
    }

    protected virtual IEnumerator ProductionRoutine()
    {
        isProducing = true;
        onProductionStart?.Invoke();

        float t = productionTime;
        while (t > 0)
        {
            UpdateInfoText($"⚙️ Производство: {t:F1} сек");
            t -= Time.deltaTime;
            yield return null;
        }

        ProduceOutputs();

        foreach (var input in inputs)
            input.amountDelivered = 0;

        isProducing = false;
        onProductionComplete?.Invoke();
        UpdateInfoText("✅ Производство завершено");
    }

    protected virtual void ProduceOutputs()
    {
        foreach (var output in outputs)
        {
            if (ResourceInventoryMono.Instance != null)
                ResourceInventoryMono.Instance.Inventory.Add(output.resource, output.amountProduced);
        }
    }

    protected string GetInputStatus()
    {
        string status = "📦 Требуется:\n";
        foreach (var input in inputs)
            status += $"{input.resource.name}: {input.amountDelivered}/{input.amountRequired}\n";
        return status;
    }

    protected void UpdateInfoText(string txt = null)
    {
        if (infoText == null) return;
        infoText.text = string.IsNullOrEmpty(txt) ? GetInputStatus() : txt;
    }

    public virtual void OnEnter(GameObject playerObj) => UpdateInfoText("🔹 [E] — внести ресурс");
    public virtual void OnExit(GameObject playerObj) => UpdateInfoText("");
}
