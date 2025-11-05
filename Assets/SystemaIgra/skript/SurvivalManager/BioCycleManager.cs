using UnityEngine;
using System.Collections.Generic;

public class BioCycleManager : MonoBehaviour
{
    [Header("Биомодули корабля")]
    public List<UniversalResourceFactory> factories = new List<UniversalResourceFactory>();

    [Header("Основные параметры цикла")]
    public float checkInterval = 3f;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0;
            AutoActivateFactories();
        }
    }

    private void AutoActivateFactories()
    {
        foreach (var factory in factories)
        {
            bool ready = true;
            foreach (var input in factory.inputs)
            {
                int available = ResourceInventoryMono.Instance.Inventory.Get(input.resource);
                if (available < input.amountRequired)
                {
                    ready = false;
                    break;
                }
            }

            if (ready)
            {
                foreach (var input in factory.inputs)
                    ResourceInventoryMono.Instance.Inventory.TrySpend(input.resource, input.amountRequired);

                // Имитация производства
                foreach (var output in factory.outputs)
                    ResourceInventoryMono.Instance.Inventory.Add(output.resource, output.amountProduced);
            }
        }
    }
}
