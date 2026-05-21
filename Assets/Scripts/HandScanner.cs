using System.Collections;
using UnityEngine;

public class HandScanner : MonoBehaviour
{
    [Header("Scan Bar")]
    [SerializeField] private Transform scanBar;
    [SerializeField] private Vector3 scanStartLocalPos;
    [SerializeField] private Vector3 scanEndLocalPos;
    [SerializeField] private float scanDuration = 2f;

    [Header("Color")]
    [SerializeField] private Renderer scanBarRenderer;
    [SerializeField] private Color idleColor = Color.red;
    [SerializeField] private Color scanningColor = Color.green;

    [Header("Events")]
    [SerializeField] private UnityEngine.Events.UnityEvent onScanComplete;

    private bool isScanning = false;

    private void Start()
    {
        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;
    }

    public void StartScan()
    {
        if (!isScanning)
            StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        isScanning = true;

        // change to scanning color
        if (scanBarRenderer != null)
            scanBarRenderer.material.color = scanningColor;

        // move bar from start to end
        float elapsed = 0f;
        while (elapsed < scanDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scanDuration;
            scanBar.localPosition = Vector3.Lerp(scanStartLocalPos, scanEndLocalPos, t);
            yield return null;
        }

        // scan complete
        onScanComplete?.Invoke();
        isScanning = false;

        // reset color and bar position
        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;

        scanBar.localPosition = scanStartLocalPos;
    }
}