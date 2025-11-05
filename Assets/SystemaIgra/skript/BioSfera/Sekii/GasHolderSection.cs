using UnityEngine;

public class GasHolderSection : MonoBehaviour, ISectionDataProvider
{
    public float biogasStored = 10f;
    public float energyOutputPerMin = 5f;
    public float pressureLevel = 0.7f; // от 0 до 1
    public float safetyLimit = 1.0f;

    public SectionType SectionType => SectionType.GasHolder;

    public string GetSectionTitle() => "Газгольдер";

    public string GetMainInfo()
    {
        return
            $"Биогаз: {biogasStored:F1} ед.\n" +
            $"Выработка энергии: {energyOutputPerMin:F1}/мин\n" +
            $"Давление: {(pressureLevel * 100f):F0}% (Предел {safetyLimit * 100f:F0}%)";
    }

    public float GetAlertLevel()
    {
        return pressureLevel > 0.9f ? 1f : 0f; // если давление критическое
    }
}
