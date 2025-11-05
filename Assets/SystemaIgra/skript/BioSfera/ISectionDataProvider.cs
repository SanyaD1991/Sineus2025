using UnityEngine.UIElements;

public interface ISectionDataProvider
{
    SectionType SectionType { get; }
    string GetSectionTitle();
    string GetMainInfo();     // Основной текст с показателями
    float GetAlertLevel();    // Например, для подсветки (0-1)
}
public enum SectionType
{
    Farm,
    Hydroponics,
    CryoChamber,
    BioLab,
    GasHolder,
    EngineRoom,
    Menu
}
