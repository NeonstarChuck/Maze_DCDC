using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScannerManager : MonoBehaviour
{
    public static ScannerManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private int requiredScanners = 2; // Change to 4 when ready

    [Header("References")]
    [SerializeField] private CentralProgressBar centralBar;

    [Header("Events")]
    public UnityEvent onAllScansComplete;

    private int completedCount = 0;
    private HashSet<HandScanner> registeredScanners = new HashSet<HandScanner>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegisterScanner(HandScanner scanner)
    {
        registeredScanners.Add(scanner);
    }

    public void ReportScanComplete(HandScanner scanner)
    {
        completedCount++;
        float progress = (float)completedCount / requiredScanners;
        centralBar?.SetProgress(progress);

        Debug.Log($"[ScannerManager] {completedCount}/{requiredScanners} scans complete.");

        if (completedCount >= requiredScanners)
        {
            onAllScansComplete?.Invoke();
            centralBar?.PlayCompletion();

            // Reset all scanners
            foreach (var s in registeredScanners)
                s.ResetScanner();

            completedCount = 0;
        }
    }
}