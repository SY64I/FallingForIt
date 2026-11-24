using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance { get; private set; }

    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 1f;

    [SerializeField] private float defaultDisplayTime = 0f;

    private CanvasGroup canvasGroup;
    private Coroutine routine;

    private string currentObjective;
    private float currentDisplayTime;

    private void Awake()
    {
        Instance = this;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void ShowObjective(string text, float displayTime = -1f)
    {
        currentObjective = text;
        currentDisplayTime = displayTime < 0f ? defaultDisplayTime : displayTime;

        PlayCurrent();
    }

    public void ShowCurrent()
    {
        if (!string.IsNullOrEmpty(currentObjective))
            PlayCurrent();
    }

    public void Hide()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(Fade(canvasGroup.alpha, 0f, fadeOutTime));
    }

    private void PlayCurrent()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(currentObjective, currentDisplayTime));
    }

    private IEnumerator ShowRoutine(string text, float displayTime)
    {
        objectiveText.text = text;

        yield return Fade(canvasGroup.alpha, 1f, fadeInTime);

        if (displayTime > 0f)
        {
            yield return new WaitForSecondsRealtime(displayTime);
            yield return Fade(1f, 0f, fadeOutTime);
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            canvasGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}