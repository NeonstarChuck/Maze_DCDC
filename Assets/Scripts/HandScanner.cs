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
    public UnityEngine.Events.UnityEvent onScanComplete; // Changed to public so the bridge can see it

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

        if (scanBarRenderer != null)
            scanBarRenderer.material.color = scanningColor;

        float elapsed = 0f;
        while (elapsed < scanDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scanDuration;
            scanBar.localPosition = Vector3.Lerp(scanStartLocalPos, scanEndLocalPos, t);
            yield return null;
        }

        onScanComplete?.Invoke();
        isScanning = false;

        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;

        scanBar.localPosition = scanStartLocalPos;
    }

    // --- ADDED FOR THE MASTER RESET SYSTEM ---
    public void ResetScanner()
    {
        StopAllCoroutines();
        isScanning = false;
        
        if (scanBarRenderer != null)
            scanBarRenderer.material.color = idleColor;

        if (scanBar != null)
            scanBar.localPosition = scanStartLocalPos;
            
        Debug.Log($"[{gameObject.name}] Scanner graphics safely restored to default.");
    }
}