using Fusion;
using UnityEngine;
using TMPro;

public class KeyZone : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    public float triggerDistance = 0.25f;

    [Header("Target Tracking Transform (Controller/Hand/Key)")]
    public Transform targetControllerTransform;

    [Header("Puzzle Object (Will HIDE when solved)")]
    public GameObject hiddenObject;

    [Header("Co-op Status UI")]
    public TextMeshProUGUI statusText;
    public Color idleColor = Color.yellow;
    public Color successColor = Color.green;

    [Networked] private bool Solved { get; set; }
    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        UpdateHiddenObjectVisibility();
        UpdateDisplay();
    }

    void Update()
    {
        if (Solved || targetControllerTransform == null) return;

        float dist = Vector3.Distance(targetControllerTransform.position, transform.position);

        if (dist < triggerDistance)
        {
            Debug.Log("Stage 1 Key puzzle triggered locally. Requesting Host to solve.");
            RPC_RequestKeySolve();
        }
    }

    public override void Render()
    {
        UpdateDisplay();

        foreach (var change in _changes.DetectChanges(this))
        {
            if (change == nameof(Solved))
            {
                UpdateHiddenObjectVisibility();
            }
        }
    }

    private void UpdateDisplay()
    {
        if (statusText == null) return;

        if (Solved)
        {
            statusText.text = "SUCCESS!\nWAITING FOR OTHER PLAYER";
            statusText.color = successColor;
        }
        else
        {
            statusText.text = "INSERT SECURITY KEY 01";
            statusText.color = idleColor;
        }
    }

    private void UpdateHiddenObjectVisibility()
{
    if (hiddenObject != null)
    {
        hiddenObject.SetActive(Solved); // Unhides (Shows) when true!
    }
}
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestKeySolve()
    {
        if (Solved) return;
        
        Solved = true;

        if (progressManager == null) 
            progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();

        if (progressManager != null)
        {
            // ROUTE TO STAGE 1
            progressManager.RPC_KeyPuzzleSolved();
        }
    }

    public void ResetKeyZoneState()
    {
        if (!Object.HasStateAuthority) return;
        
        Solved = false;
        UpdateHiddenObjectVisibility(); 
        Debug.Log($"[{gameObject.name}] Stage 1 Key Zone reset complete.");
    }
}