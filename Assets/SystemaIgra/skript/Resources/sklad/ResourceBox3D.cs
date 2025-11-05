using UnityEngine;

public class ResourceBox3D : MonoBehaviour
{
    private ResourceType resourceType;
    private int amount;

    public void Setup(ResourceType type, int amount)
    {
        this.resourceType = type;
        this.amount = amount;
        // Здесь можно добавить визуализацию (цвет, иконку и т.п.)
    }

    public ResourceType ResourceType => resourceType;
    public int Amount => amount;
}
