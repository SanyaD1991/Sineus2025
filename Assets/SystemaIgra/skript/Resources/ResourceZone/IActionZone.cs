using UnityEngine;

public interface IActionZone
{
    void OnEnter(GameObject player);
    void OnExit(GameObject player);
    void Interact(GameObject player);
}
