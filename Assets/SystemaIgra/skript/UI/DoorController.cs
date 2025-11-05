using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour
{
    [Header("Настройки двери")]
    public Transform doorTransform;   // сама дверь
    public float openHeight = 3f;     // на сколько поднимается
    public float openSpeed = 2f;      // скорость подъёма
    public float squashSpeed = 2f;    // скорость «сплющивания»

    [Header("Способ открытия")]
    public bool openByKey = true;             // открыть кнопкой
    public bool openByResource = false;       // открыть за ресурс
    public ResourceType requiredResource;     // ресурс для открытия
    public int requiredAmount = 1;

    [Header("Инвентарь игрока (для открытия за ресурс)")]
    public PlayerCarry playerCarry;

    private Vector3 closedPos;
    private Vector3 openPos;
    private Vector3 originalScale;
    private bool isOpen = false;
    private bool isAnimating = false;

#if ENABLE_INPUT_SYSTEM
    private bool playerInZone = false;
#endif

    private void Start()
    {
        if (doorTransform == null) doorTransform = transform;

        closedPos = doorTransform.position;
        openPos = closedPos + Vector3.up * openHeight;

        originalScale = doorTransform.localScale;

        // делаем коллайдер триггером
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (playerInZone && !isOpen)
        {
            if (openByKey && Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenDoor();
            }
            else if (openByResource && playerCarry != null)
            {
                if (playerCarry.HasResource(requiredResource, requiredAmount))
                {
                    playerCarry.RemoveResource(requiredResource, requiredAmount);
                    OpenDoor();
                }
            }
        }
#endif

        // Анимация движения и сплющивания
        if (isAnimating)
        {
            // позиция
            doorTransform.position = Vector3.Lerp(doorTransform.position, isOpen ? openPos : closedPos, Time.deltaTime * openSpeed);

            // масштаб по Y
            float targetYScale = isOpen ? 0.2f : 1f;
            Vector3 newScale = doorTransform.localScale;
            newScale.y = Mathf.Lerp(newScale.y, targetYScale, Time.deltaTime * squashSpeed);
            doorTransform.localScale = newScale;

            // проверка окончания анимации
            if (Vector3.Distance(doorTransform.position, isOpen ? openPos : closedPos) < 0.01f &&
                Mathf.Abs(doorTransform.localScale.y - targetYScale) < 0.01f)
            {
                doorTransform.position = isOpen ? openPos : closedPos;
                Vector3 scaleFix = doorTransform.localScale;
                scaleFix.y = targetYScale;
                doorTransform.localScale = scaleFix;
                isAnimating = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
#if ENABLE_INPUT_SYSTEM
        playerInZone = true;
#endif
    }

    private void OnTriggerExit(Collider other)
    {
#if ENABLE_INPUT_SYSTEM
        if (other.CompareTag("Player"))
            playerInZone = false;
#endif
    }

    public void OpenDoor()
    {
        isOpen = true;
        isAnimating = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
        isAnimating = true;
    }
}
