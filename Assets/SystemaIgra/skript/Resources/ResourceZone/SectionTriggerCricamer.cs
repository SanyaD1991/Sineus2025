using UnityEngine;

public class SectionTriggerCriocamer : MonoBehaviour
{
    public MonoBehaviour sectionProvider; // например, CryoChamberSection
    public ShipSectionUIManager ShipSectionUIManager1;

    private ISectionDataProvider provider;

    private void Awake()
    {
        provider = sectionProvider as ISectionDataProvider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Если это CryoChamberSection, актуализируем значения
        if (provider is CryoChamberSection cryo)
        {
            cryo.UpdateValuesFromSurvivalManager();
        }

        ShipSectionUIManager1.ShowSection(provider);
    }
}
