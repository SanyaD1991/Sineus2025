using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SpendZone : MonoBehaviour, IActionZone
{
    [Header("Настройки траты ресурса")]
    [SerializeField] private ResourceType requiredResource; // Тип ресурса
    [SerializeField] private int spendAmount = 1;           // Сколько тратится

    [Header("Событие при трате ресурса")]
    public UnityEvent<int> onResourceSpent; // Можно настроить в инспекторе (например, активировать постройку)

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerActionController controller = other.GetComponent<PlayerActionController>();
        if (controller != null)
        {
            controller.SetZone(this, gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerActionController controller = other.GetComponent<PlayerActionController>();
        if (controller != null)
        {
            controller.ClearZone(this, gameObject);
        }
    }

    public void Interact(GameObject playerObj)
    {
        PlayerCarry player = playerObj.GetComponent<PlayerCarry>();
        if (player == null) return;

        TrySpend(player);
    }

    private void TrySpend(PlayerCarry player)
    {
        if (!player.HasResource(requiredResource, spendAmount))
        {
            Debug.Log($"❌ У игрока нет ресурса {requiredResource.name}");
            return;
        }

        int available = player.GetAmount(requiredResource);
        int amountToSpend = Mathf.Min(spendAmount, available);

        player.RemoveResource(requiredResource, amountToSpend);
        Debug.Log($"💰 Игрок потратил {amountToSpend} {requiredResource.name}");

        // 🔥 Вызываем UnityEvent (настраивается в инспекторе)
        onResourceSpent?.Invoke(amountToSpend);
    }

    public void OnEnter(GameObject playerObj)
    {
        // Можно добавить визуальную подсветку, подсказку "Нажмите E"
    }

    public void OnExit(GameObject playerObj)
    {
        // Убрать подсказку
    }
}
