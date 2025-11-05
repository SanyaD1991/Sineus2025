    using UnityEngine;

    public class SectionTrigger : MonoBehaviour
    {
        public MonoBehaviour sectionProvider; // например, FarmSection
        public ShipSectionUIManager ShipSectionUIManager1; // например, FarmSection
        private ISectionDataProvider provider;

        private void Awake()
        {
            provider = sectionProvider as ISectionDataProvider;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            ShipSectionUIManager1.ShowSection(provider);
        }
    }
