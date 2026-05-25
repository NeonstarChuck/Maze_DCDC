using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent onScanComplete;

    private bool isScanning = false;

    public bool IsCompleted { get; private set; }

    private void Start()
    {
        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;
    }

    public void StartScan()
    {
        if (!isScanning && !IsCompleted)
            StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        isScanning = true;

        if (scanBarRenderer != null)
            scanBarRenderer.material.color = scanningColor;

        float elapsed = 0f;

        while (elapsed < scanDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scanDuration;

            scanBar.localPosition =
                Vector3.Lerp(scanStartLocalPos, scanEndLocalPos, t);

            yield return null;
        }

        IsCompleted = true; // ✅ IMPORTANT SIGNAL FOR MANAGER

        onScanComplete?.Invoke();

        isScanning = false;

        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;
    }

    public void ResetScanner()
    {
        StopAllCoroutines();

        isScanning = false;
        IsCompleted = false; // reset state

        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;

        if (scanBar != null)
            scanBar.localPosition = scanStartLocalPos;

        Debug.Log($"[{gameObject.name}] Scanner reset");
    }
}