using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ShipSectionUIManager : MonoBehaviour
{
    [Header("UI элементы")]
    public TextMeshProUGUI sectionTitle;
    public TextMeshProUGUI sectionInfo;
    public Image alertIndicator;
    public CanvasGroup uiCanvasGroup;

    [Header("Настройки отображения")]
    public float displayDuration = 5f;
    public float fadeSpeed = 3f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        uiCanvasGroup.alpha = 0;
    }

    public void ShowSection(ISectionDataProvider provider)
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        sectionTitle.text = provider.GetSectionTitle();
        sectionInfo.text = provider.GetMainInfo();

        float alert = provider.GetAlertLevel();
        alertIndicator.color = Color.Lerp(Color.green, Color.red, alert);

        //StartCoroutine(FadeCanvas(1));
        hideRoutine = StartCoroutine(AutoHide());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(displayDuration);
        yield return FadeCanvas(0);
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        while (Mathf.Abs(uiCanvasGroup.alpha - targetAlpha) > 0.01f)
        {
            uiCanvasGroup.alpha = Mathf.Lerp(uiCanvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        uiCanvasGroup.alpha = targetAlpha;
    }
}
