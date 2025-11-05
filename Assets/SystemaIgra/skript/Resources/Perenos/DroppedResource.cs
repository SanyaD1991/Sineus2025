using UnityEngine;

public class DroppedResource : MonoBehaviour
{
    public ResourceType resourceType;
    public int amount = 1;

    private void Awake()
    {
        // ✅ автоматическое присвоение тега
        if (!CompareTag("GroundItem"))
            gameObject.tag = "GroundItem";
    }
}
