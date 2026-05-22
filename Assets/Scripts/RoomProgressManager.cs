using Fusion;
using UnityEngine;

public class RoomProgressManager : NetworkBehaviour
{
    [Header("Stage 1 Doors")]
    public Door1 stage1LeftDoor;
    public Door1 stage1RightDoor;

    [Header("Stage 2 Doors")]
    public Door1 stage2LeftDoor;  
    public Door1 stage2RightDoor; 

    [Header("Stage 3 Final Sequence (Drag rooms or roots to hide here)")]
    public GameObject[] lastRoomsToHide;

    // --- NETWORKED STATE VARIABLES ---
    [Networked] public bool ColorSolved { get; set; }
    [Networked] public bool KeySolved { get; set; }
    [Networked] public bool Stage2KeyZoneSolved { get; set; }
    [Networked] public bool Stage2KeypadSolved { get; set; }

    // Hand scanner networked registers
    [Networked] public bool Scanner1Done { get; set; }
    [Networked] public bool Scanner2Done { get; set; }
    [Networked] public bool Scanner3Done { get; set; }
    [Networked] public bool Scanner4Done { get; set; }
    [Networked] public bool FinalRoomsHidden { get; set; }

    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[ProgressManager] Reset Input Detected! Running Master Reset Sequence...");
            RPC_RequestReset();
        }
    }

    public override void Render()
    {
        // Smoothly and reactively manages final room execution across ALL client headsets simultaneously
        if (FinalRoomsHidden)
        {
            foreach (var room in lastRoomsToHide) 
                if (room != null && room.activeSelf) room.SetActive(false);
        }
        else
        {
            foreach (var room in lastRoomsToHide) 
                if (room != null && !room.activeSelf) room.SetActive(true);
        }

        foreach (var change in _changes.DetectChanges(this))
        {
            if (change == nameof(Stage2KeypadSolved))
            {
                if (!Stage2KeypadSolved)
                {
                    KeypadNetworkBridge keypadBridge = UnityEngine.Object.FindFirstObjectByType<KeypadNetworkBridge>();
                    if (keypadBridge != null) keypadBridge.ResetLocalKeypadUI();
                }
            }
        }
    }

    // --- PUZZLE SOLVED ROUTERS ---
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ColorPuzzleSolved() { ColorSolved = true; CheckStage1Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_KeyPuzzleSolved() { KeySolved = true; CheckStage1Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeyZoneSolved() { Stage2KeyZoneSolved = true; CheckStage2Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeypadSolved() { Stage2KeypadSolved = true; CheckStage2Completion(); }

    // --- HAND SCANNER INTAKE PACKET ---
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RegisterHandScanner(int id)
    {
        if (id == 1) Scanner1Done = true;
        if (id == 2) Scanner2Done = true;
        if (id == 3) Scanner3Done = true;
        if (id == 4) Scanner4Done = true;

        Debug.Log($"[Host Tracker Verification] Status -> S1:{Scanner1Done} | S2:{Scanner2Done} | S3:{Scanner3Done} | S4:{Scanner4Done}");
        CheckStage3Completion();
    }


    // --- CENTRAL REVALUATION LOOPS ---
    private void CheckStage1Completion()
    {
        if (ColorSolved && KeySolved)
        {
            if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = true;
            if (stage1RightDoor != null) stage1RightDoor.IsOpen = true;
        }
    }

    private void CheckStage2Completion()
    {
        if (Stage2KeyZoneSolved && Stage2KeypadSolved)
        {
            if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = true;
            if (stage2RightDoor != null) stage2RightDoor.IsOpen = true;
        }
    }

    private void CheckStage3Completion()
    {
        if (Scanner1Done && Scanner2Done && Scanner3Done && Scanner4Done)
        {
            Debug.Log("[HOST SUCCESS] All 4 Hand Trackers Finished! Vanishing terminal rooms.");
            FinalRoomsHidden = true; // Flips network bit; Render updates everyone instantly!
        }
    }


    // --- THE ABSOLUTE MASTER RESET ---
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestReset()
    {
        Debug.Log("[Master System Wipe] Re-locking environments and defaulting puzzles.");

        // 1. Clear Master State Values
        ColorSolved = false;
        KeySolved = false;
        Stage2KeyZoneSolved = false;
        Stage2KeypadSolved = false;
        
        // Hand scanner state clears
        Scanner1Done = false;
        Scanner2Done = false;
        Scanner3Done = false;
        Scanner4Done = false;
        FinalRoomsHidden = false;

        // 2. Clear Doors
        if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = false;
        if (stage1RightDoor != null) stage1RightDoor.IsOpen = false;
        if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = false;
        if (stage2RightDoor != null) stage2RightDoor.IsOpen = false;

        // 3. Clear Color Combination tracking scripts
        PuzzleManager[] colorPuzzles = UnityEngine.Object.FindObjectsByType<PuzzleManager>(FindObjectsSortMode.None);
        foreach (PuzzleManager p in colorPuzzles) if (p != null) p.ResetPuzzleState();

        // 4. Clear Proximity Key Zones (Both types)
        KeyZone[] keyZones = UnityEngine.Object.FindObjectsByType<KeyZone>(FindObjectsSortMode.None);
        foreach (KeyZone kz in keyZones) if (kz != null) kz.ResetKeyZoneState();

        KeyZoneStage2[] stage2KeyZones = UnityEngine.Object.FindObjectsByType<KeyZoneStage2>(FindObjectsSortMode.None);
        foreach (KeyZoneStage2 kz2 in stage2KeyZones) if (kz2 != null) kz2.ResetKeyZoneState();

        // 5. AUTOMATION: Find and reset all HandScanner components on the map
        HandScanner[] scanners = UnityEngine.Object.FindObjectsByType<HandScanner>(FindObjectsSortMode.None);
        foreach (HandScanner hs in scanners) if (hs != null) hs.ResetScanner();

        // 6. Clean UI Networks
        RPC_BroadcastVisualReset();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_BroadcastVisualReset()
    {
        KeypadNetworkBridge keypadBridge = UnityEngine.Object.FindFirstObjectByType<KeypadNetworkBridge>();
        if (keypadBridge != null) keypadBridge.ResetLocalKeypadUI();
    }
}