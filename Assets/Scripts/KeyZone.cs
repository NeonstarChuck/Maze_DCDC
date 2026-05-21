using Fusion;
using UnityEngine;

public class KeyZone : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    public float triggerDistance = 0.25f;

    [Header("Drag Scene Controller Tracking Here")]
    public Transform targetControllerTransform;

    [Header("Puzzle Rewards")]
    public GameObject hiddenObject;

    [Networked] private bool Solved { get; set; }
    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        UpdateHiddenObjectVisibility();
    }

    void Update()
    {
        if (Solved || targetControllerTransform == null) return;

        float dist = Vector3.Distance(targetControllerTransform.position, transform.position);
        if (dist < triggerDistance)
        {
            Debug.Log("Key puzzle triggered locally. Requesting Host to solve.");
            RPC_RequestKeySolve();
        }
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this))
        {
            if (change == nameof(Solved))
            {
                UpdateHiddenObjectVisibility();
            }
        }
    }

    private void UpdateHiddenObjectVisibility()
    {
        if (hiddenObject != null)
        {
            hiddenObject.SetActive(Solved);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestKeySolve()
    {
        if (Solved) return;
        
        Solved = true;

        // --- THE FIX: Auto-locate manager if inspector slot is empty ---
        if (progressManager == null) progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();

        if (progressManager != null)
        {
            progressManager.RPC_KeyPuzzleSolved();
        }
    }

    public void ResetKeyZoneState()
    {
        if (!Object.HasStateAuthority) return;
        Solved = false;
        Debug.Log($"[{gameObject.name}] Key tracking zone has been relocked.");
    }
}