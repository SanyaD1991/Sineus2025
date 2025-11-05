using UnityEngine;

public class HydroponicSection : MonoBehaviour, ISectionDataProvider
{
    public float waterUsagePerMin;
    public float energyUsagePerMin;
    public float waterLightBalance; // от 0 до 1
    public float oxygenProductionPerMin;
    public float feedProductionPerMin;

    public SectionType SectionType => SectionType.Hydroponics;

    public string GetSectionTitle() => "Гидропоника";

    public string GetMainInfo()
    {
        return
            $"Потребление воды: {waterUsagePerMin:F1}/мин\n" +
            $"Потребление энергии: {energyUsagePerMin:F1}/мин\n" +
            $"Баланс света/воды: {waterLightBalance * 100f:F0}%\n" +
            $"Выработка кислорода: {oxygenProductionPerMin:F1}/мин\n" +
            $"Корм для коров: {feedProductionPerMin:F1}/мин";
    }

    public float GetAlertLevel() => waterLightBalance < 0.3f ? 1f : 0f;
}
