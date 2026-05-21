using Fusion;
using UnityEngine;

public class RoomProgressManager : NetworkBehaviour
{
    [Header("Stage 1 Doors (Drag from Scene Hierarchy)")]
    public Door1 stage1LeftDoor;
    public Door1 stage1RightDoor;

    [Header("Stage 2 Doors (Drag from Scene Hierarchy)")]
    public Door1 stage2LeftDoor;  
    public Door1 stage2RightDoor; 

    // --- NETWORKED STATE VARIABLES ---
    [Networked] public bool ColorSolved { get; set; }
    [Networked] public bool KeySolved { get; set; }
    [Networked] public bool Stage2KeyZoneSolved { get; set; }
    [Networked] public bool Stage2KeypadSolved { get; set; }

    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    void Update()
    {
        // --- THE FIX: Added 'R' key backup input for easy desktop testing ---
        if (OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[ProgressManager] Reset triggered! Sending direct master reset command...");
            RPC_RequestReset();
        }
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this))
        {
            if (change == nameof(Stage2KeypadSolved))
            {
                if (!Stage2KeypadSolved)
                {
                    KeypadNetworkBridge keypadBridge = UnityEngine.Object.FindFirstObjectByType<KeypadNetworkBridge>();
                    if (keypadBridge != null) 
                    {
                        keypadBridge.ResetLocalKeypadUI();
                    }
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ColorPuzzleSolved()
    {
        ColorSolved = true;
        CheckStage1Completion(); 
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_KeyPuzzleSolved()
    {
        KeySolved = true;
        CheckStage1Completion(); 
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeyZoneSolved()
    {
        Stage2KeyZoneSolved = true;
        CheckStage2Completion(); 
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeypadSolved()
    {
        Stage2KeypadSolved = true;
        CheckStage2Completion(); 
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestReset()
    {
        Debug.Log("[Host Reset] Wiping progress state registers and locking doors.");

        ColorSolved = false;
        KeySolved = false;
        Stage2KeyZoneSolved = false;
        Stage2KeypadSolved = false;

        if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = false;
        if (stage1RightDoor != null) stage1RightDoor.IsOpen = false;
        if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = false;
        if (stage2RightDoor != null) stage2RightDoor.IsOpen = false;

        PuzzleManager[] colorPuzzles = UnityEngine.Object.FindObjectsByType<PuzzleManager>(FindObjectsSortMode.None);
        foreach (PuzzleManager p in colorPuzzles)
        {
            if (p != null) p.ResetPuzzleState();
        }

        KeyZone[] keyZones = UnityEngine.Object.FindObjectsByType<KeyZone>(FindObjectsSortMode.None);
        foreach (KeyZone kz in keyZones)
        {
            if (kz != null) kz.ResetKeyZoneState();
        }

        KeyZoneStage2[] stage2KeyZones = UnityEngine.Object.FindObjectsByType<KeyZoneStage2>(FindObjectsSortMode.None);
        foreach (KeyZoneStage2 kz2 in stage2KeyZones) if (kz2 != null) kz2.ResetKeyZoneState();

        RPC_BroadcastVisualReset();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_BroadcastVisualReset()
    {
        KeypadNetworkBridge keypadBridge = UnityEngine.Object.FindFirstObjectByType<KeypadNetworkBridge>();
        if (keypadBridge != null) 
        {
            keypadBridge.ResetLocalKeypadUI();
        }
    }

    private void CheckStage1Completion()
    {
        if (ColorSolved && KeySolved)
        {
            Debug.Log("[Host] Stage 1 Requirements Met! Opening Sector 1 Partition Gates.");
            if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = true;
            if (stage1RightDoor != null) stage1RightDoor.IsOpen = true;
        }
    }

    private void CheckStage2Completion()
    {
        if (Stage2KeyZoneSolved && Stage2KeypadSolved)
        {
            Debug.Log("[Host] Stage 2 Requirements Met! Opening Sector 2 Exit Gates.");
            if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = true;
            if (stage2RightDoor != null) stage2RightDoor.IsOpen = true;
        }
    }
}