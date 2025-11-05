using UnityEngine;

public class EngineRoomSection : MonoBehaviour, ISectionDataProvider
{
    public float fuelAmount = 50f;
    public float maxFuel = 100f;
    public float thrustPowerPerMin = 25f;
    public bool engineActive = true;

    public SectionType SectionType => SectionType.EngineRoom;

    public string GetSectionTitle() => "Двигательный отсек";

    public string GetMainInfo()
    {
        string state = engineActive ? "АКТИВЕН" : "ОСТАНОВЛЕН";
        return
            $"Состояние: {state}\n" +
            $"Топливо: {fuelAmount:F1}/{maxFuel:F1}\n" +
            $"Мощность: {thrustPowerPerMin:F1} ед./мин";
    }

    public float GetAlertLevel()
    {
        if (!engineActive) return 1f;
        return fuelAmount < 10f ? 0.7f : 0f;
    }
}
