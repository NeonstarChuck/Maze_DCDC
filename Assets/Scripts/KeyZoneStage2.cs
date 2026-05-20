using Fusion;
using UnityEngine;

public class KeyZoneStage2 : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    public float triggerDistance = 0.25f;

    [Header("Drag Scene Controller Tracking Here")]
    public Transform targetControllerTransform;

    [Header("Puzzle Rewards (Optional)")]
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
        // Change detector forces BOTH headsets to see the hidden object unhide instantly
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
        if (progressManager != null)
        {
            // The only structural change: targeted directly to Stage 2 variables
            progressManager.RPC_Stage2KeyZoneSolved();
        }
    }
}