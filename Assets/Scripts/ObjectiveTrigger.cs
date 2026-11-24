using System.Collections;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    private enum ShowMode
    {
        WhenPlayerEnters,
        WhenSceneStarts
    }

    [SerializeField, TextArea(2, 4)] private string objective = "New objective";
    [SerializeField] private ShowMode showWhen = ShowMode.WhenPlayerEnters;

    [SerializeField] private float displayTime = 0f;

    [SerializeField] private float startDelay = 1f;

    [SerializeField] private bool onlyOnce = true;

    private bool shown;

    private IEnumerator Start()
    {
        if (showWhen != ShowMode.WhenSceneStarts)
            yield break;

        yield return new WaitForSecondsRealtime(startDelay);
        Show();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (showWhen != ShowMode.WhenPlayerEnters)
            return;

        if (other.GetComponentInParent<PlayerControls>() == null)
            return;

        Show();
    }

    private void Show()
    {
        Debug.Log("Objective trigger activated: " + gameObject.name);
        if (onlyOnce && shown)
            return;

        if (ObjectiveUI.Instance == null)
        {
            return;
        }

        shown = true;
        ObjectiveUI.Instance.ShowObjective(objective, displayTime);
    }
}