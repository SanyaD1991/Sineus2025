using UnityEngine;

public class BioLabSection : MonoBehaviour, ISectionDataProvider
{
    public float wasteInputPerMin = 3f;
    public float fertilizerOutputPerMin = 2f;
    public float waterRecoveredPerMin = 0.5f;
    public float biogasOutputPerMin = 1f;

    public SectionType SectionType => SectionType.BioLab;

    public string GetSectionTitle() => "Биолаборатория";

    public string GetMainInfo()
    {
        return
            $"Переработка отходов: {wasteInputPerMin:F1}/мин\n" +
            $"Выработка биогаза: {biogasOutputPerMin:F1}/мин\n" +
            $"Вода (восстановлено): {waterRecoveredPerMin:F1}/мин\n" +
            $"Удобрения: {fertilizerOutputPerMin:F1}/мин";
    }

    public float GetAlertLevel()
    {
        return wasteInputPerMin > 6f ? 1f : 0f; // если перегрузка отходов
    }
}
