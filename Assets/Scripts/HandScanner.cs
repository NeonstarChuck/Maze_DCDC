using System.Collections;
using UnityEngine;

public class HandScanner : MonoBehaviour
{
    [SerializeField] private Transform scanBar;
    [SerializeField] private Vector3 scanStartLocalPos;
    [SerializeField] private Vector3 scanEndLocalPos;
    [SerializeField] private float scanDuration = 2f;
    [SerializeField] private UnityEngine.Events.UnityEvent onScanComplete;

    private bool isScanning = false;

    public void StartScan()
    {
        if (!isScanning)
            StartCoroutine(ScanRoutine());
    }

    private IEnumerator ScanRoutine()
    {
        isScanning = true;

        // move bar from top to bottom
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

        // reset bar
        scanBar.localPosition = scanStartLocalPos;
    }
}