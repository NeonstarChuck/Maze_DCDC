using Fusion;
using UnityEngine;

public class RoomProgressManager : NetworkBehaviour
{
    [Header("Stage 1 Objects")]
    public Door1 leftDoor;
    public Door1 rightDoor;

    [Header("Stage 2 Objects")]
    public Door1 stage2LeftDoor;  
    public Door1 stage2RightDoor; 

    // --- NETWORKED VARIABLES ---
    [Networked] public bool ColorSolved { get; set; }
    [Networked] public bool KeySolved { get; set; }
    [Networked] public bool Stage2KeyZoneSolved { get; set; }
    [Networked] public bool Stage2KeypadSolved { get; set; }

    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        
        CheckStage1Completion();
        CheckStage2Completion();
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(ColorSolved):
                case nameof(KeySolved):
                    CheckStage1Completion();
                    break;

                case nameof(Stage2KeyZoneSolved):
                case nameof(Stage2KeypadSolved):
                    CheckStage2Completion();
                    break;
            }
        }
    }

    // --- ALL INTERNET SIGNALS (RPCs) ---
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ColorPuzzleSolved() => ColorSolved = true;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_KeyPuzzleSolved() => KeySolved = true;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeyZoneSolved() => Stage2KeyZoneSolved = true;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeypadSolved() => Stage2KeypadSolved = true;


    // --- UNLOCK CHECKS (Host Only) ---
    private void CheckStage1Completion()
    {
        if (ColorSolved && KeySolved && Object.HasStateAuthority)
        {
            if (leftDoor != null) leftDoor.IsOpen = true;
            if (rightDoor != null) rightDoor.IsOpen = true;
        }
    }

    private void CheckStage2Completion()
    {
        // Change: Requires BOTH Stage 2 variables to be true before opening BOTH doors
        if (Stage2KeyZoneSolved && Stage2KeypadSolved && Object.HasStateAuthority)
        {
            Debug.Log("[Fusion] Stage 2 Complete! Opening both Stage 2 doors simultaneously.");
            if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = true;
            if (stage2RightDoor != null) stage2RightDoor.IsOpen = true;
        }
    }
}