using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActionController : MonoBehaviour
{
    private PlayerInput input;
    private InputAction interactAction;
    private IActionZone currentZone;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        interactAction = input.actions["vzal"];
    }

    private void OnEnable()
    {
        interactAction.performed += Onvzal;
    }

    private void OnDisable()
    {
        interactAction.performed -= Onvzal;
    }

    public void Onvzal(InputAction.CallbackContext ctx)
    {
        currentZone?.Interact(gameObject);
    }

    public void SetZone(IActionZone zone, GameObject zoneObj)
    {
        currentZone = zone;
        zone.OnEnter(gameObject);
    }

    public void ClearZone(IActionZone zone, GameObject zoneObj)
    {
        if (currentZone == zone)
        {
            zone.OnExit(gameObject);
            currentZone = null;
        }
    }
}
