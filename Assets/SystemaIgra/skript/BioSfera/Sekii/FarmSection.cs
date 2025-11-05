using UnityEngine;

public class FarmSection : MonoBehaviour, ISectionDataProvider
{
    public float manureAmount;         // Количество навоза
    public float foodStock;            // Запасы еды
    public float productionPerMinute;  // Производство
    public float consumptionPerMinute; // Потребление

    public SectionType SectionType => SectionType.Farm;

    public string GetSectionTitle() => "Ферма — животные";

    public string GetMainInfo()
    {
        return
            $"Навоз: {manureAmount:F1} кг\n" +
            $"Производство: {productionPerMinute:F1}/мин\n" +
            $"Еда: {foodStock:F1} ед. (Потребление {consumptionPerMinute:F1}/мин)";
    }

    public float GetAlertLevel()
    {
        return manureAmount > 50f ? 1f : 0f; // Например, если много навоза — тревога
    }
}
