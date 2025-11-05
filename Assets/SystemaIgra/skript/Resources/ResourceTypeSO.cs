using UnityEngine;

[CreateAssetMenu(menuName = "Resources/ResourceType")]
public class ResourceType : ScriptableObject
{
    public string resourceName;
    public Sprite icon;

    [Header("Prefab для переноски игроком")]
    public GameObject carryPrefab; // вот это добавляем
}
