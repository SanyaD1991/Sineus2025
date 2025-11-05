using UnityEngine;

public class CryoChamberSection : MonoBehaviour, ISectionDataProvider
{
    [Header("Синхронизация с SurvivalManager")]
    public SurvivalManager survivalManager;

    [HideInInspector] public float crewHealth = 100f;
    [HideInInspector] public float oxygenConsumptionPerMin = 2f;
    [HideInInspector] public float energyConsumptionPerMin = 1f;
    [HideInInspector] public int crewCount = 4;

    public SectionType SectionType => SectionType.CryoChamber;
    public string GetSectionTitle() => "Криокамера";

    // Метод для актуализации значений
    // Метод для актуализации значений по вашим ресурсам
    public void UpdateValuesFromSurvivalManager()
    {
        if (survivalManager == null) return;

        int aliveCount = 0;
        float healthSum = 0f;

        foreach (var crew in survivalManager.crewMembers)
        {
            if (!crew.IsAlive) continue;
            aliveCount++;
            healthSum += crew.health; // предполагаем, что CrewMember имеет поле health
        }

        crewCount = aliveCount;
        crewHealth = aliveCount > 0 ? healthSum / aliveCount : 0f;

        // Обнуляем потребление
        oxygenConsumptionPerMin = 0f;
        energyConsumptionPerMin = 0f;

        // Считаем потребление по нужным ресурсам
        foreach (var res in survivalManager.survivalResources)
        {
            if (res.resourceType.name == "Algae")
                oxygenConsumptionPerMin += res.consumeRate * aliveCount; // Algae вместо Oxygen
            if (res.resourceType.name == "OrganicWaste")
                energyConsumptionPerMin += res.consumeRate * aliveCount; // OrganicWaste вместо Energy
        }
    }


    public string GetMainInfo()
    {
        return
            $"Экипаж: {crewCount} чел.\n" +
            $"Среднее здоровье: {crewHealth:F0}%\n" +
            $"Потребление O₂: {oxygenConsumptionPerMin:F1}/мин\n" +
            $"Потребление энергии: {energyConsumptionPerMin:F1}/мин";
    }

    public float GetAlertLevel()
    {
        return crewHealth < 40f ? 1f : 0f;
    }
}
