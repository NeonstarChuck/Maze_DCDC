using Fusion;
using UnityEngine;

public class HandScannerNetworkBridge : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    
    [Header("Assign Local Component")]
    public HandScanner localScanner;

    [Header("Scanner Identity (Set 1, 2, 3, or 4 respectively)")]
    [Range(1, 4)] public int scannerID = 1;

    private void Start()
    {
        if (localScanner == null) 
            localScanner = GetComponent<HandScanner>();

        // Automatically subscribe to the local script's UnityEvent
        if (localScanner != null)
        {
            localScanner.onScanComplete.AddListener(OnLocalScanFinished);
        }
    }

    private void OnLocalScanFinished()
    {
        Debug.Log($"[Scanner Bridge] Hand scanner {scannerID} completed locally! Notifying Host...");
        RPC_SubmitScannerCompletion(scannerID);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SubmitScannerCompletion(int id)
    {
        // Auto-locate manager if inspector slot is missing
        if (progressManager == null) 
            progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();

        if (progressManager != null)
        {
            progressManager.RPC_RegisterHandScanner(id);
        }
    }
}