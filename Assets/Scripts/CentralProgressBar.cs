using System.Collections;
using UnityEngine;

public class CentralProgressBar : MonoBehaviour
{
    [Header("Bar Transform")]
    [SerializeField] private Transform bar;
    [SerializeField] private Vector3 startLocalPos; // same as your numpad scanStartLocalPos
    [SerializeField] private Vector3 endLocalPos;   // same as your numpad scanEndLocalPos

    [Header("Color")]
    [SerializeField] private Renderer barRenderer;
    [SerializeField] private Color idleColor = Color.gray;
    [SerializeField] private Color progressColor = Color.yellow;
    [SerializeField] private Color completeColor = Color.green;

    private void Start()
    {
        bar.localPosition = startLocalPos;
        if (barRenderer) barRenderer.material.color = idleColor;
    }

    public void SetProgress(float t) // t = 0..1
    {
        bar.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, t);
        if (barRenderer) barRenderer.material.color =
            Color.Lerp(progressColor, completeColor, t);
    }

    public void PlayCompletion()
    {
        StartCoroutine(CompletionFlash());
    }

    private IEnumerator CompletionFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            if (barRenderer) barRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            if (barRenderer) barRenderer.material.color = completeColor;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        bar.localPosition = startLocalPos;
        if (barRenderer) barRenderer.material.color = idleColor;
    }
}