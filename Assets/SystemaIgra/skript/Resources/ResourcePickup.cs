using UnityEngine;

public class ResourcePickup : MonoBehaviour, IResourceProvider
{
    public ResourceType resourceType;
    public int amount = 1;

    public ResourceType ResourceType => resourceType;
    public int Amount => amount;

    public void Collect()
    {
        //ResourceInventory.Instance.AddResource(resourceType, amount);
        Destroy(gameObject);
    }
}