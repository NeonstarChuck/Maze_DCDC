using Fusion;
using UnityEngine;

public class KeyZoneStage2 : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    public float triggerDistance = 0.25f;

    [Header("Drag Scene Controller Tracking Here")]
    public Transform targetControllerTransform;

    [Header("Puzzle Object (Will HIDE when solved)")]
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

        // Measures real-world distance from the dragged controller transform
        float dist = Vector3.Distance(targetControllerTransform.position, transform.position);

        if (dist < triggerDistance)
        {
            Debug.Log("Stage 2 Key puzzle triggered locally. Requesting Host to solve.");
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
            // --- THE FIX: '!' means NOT. True becomes False, False becomes True.
            // When Solved is true, SetActive becomes false (Hides the object)
            hiddenObject.SetActive(!Solved);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestKeySolve()
    {
        if (Solved) return;
        
        Solved = true;

        // Auto-locate manager if inspector link is dropped
        if (progressManager == null) progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();

        if (progressManager != null)
        {
            progressManager.RPC_Stage2KeyZoneSolved();
        }
    }

    // --- ADDED FOR THE MASTER RESET SYSTEM ---
    public void ResetKeyZoneState()
    {
        if (!Object.HasStateAuthority) return;
        Solved = false;
        UpdateHiddenObjectVisibility(); // Bring the object back instantly on host
        Debug.Log($"[{gameObject.name}] Stage 2 Key zone reset. Object brought back.");
    }
}