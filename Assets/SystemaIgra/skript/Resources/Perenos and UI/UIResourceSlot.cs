using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Один элемент UI, показывающий ресурс и его количество.
/// </summary>
public class UIResourceSlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    private ResourceType resourceType;

    public void Setup(ResourceType type, int amount)
    {
        resourceType = type;
        if (iconImage != null)
            iconImage.sprite = type.icon; // иконка из ScriptableObject
        SetAmount(amount);
    }

    public void SetAmount(int amount)
    {
        if (amountText != null)
            amountText.text = amount.ToString();
    }
}
