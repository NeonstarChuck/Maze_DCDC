using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System on Mac/Editor

public class RoomProgressManager : NetworkBehaviour
{
    [Header("Stage 1 Doors")]
    public Door1 stage1LeftDoor;
    public Door1 stage1RightDoor;

    [Header("Stage 2 Doors")]
    public Door1 stage2LeftDoor;  
    public Door1 stage2RightDoor; 

    [Header("Stage 3 Final Sequence (Rooms to hide)")]
    public GameObject[] lastRoomsToHide;

    [Header("Central Audio Source (SFX Only)")]
    public AudioSource puzzleAudioSource;

    [Header("Background Music System")]
    public AudioSource bgmAudioSource; 

    [Header("Individual Puzzle Chimes")]
    public AudioClip stage1ColorSolvedClip;
    public AudioClip stage1KeySolvedClip;
    public AudioClip stage2KeyZoneClip;
    public AudioClip stage2KeypadClip;

    [Header("Gate & Victory Sounds")]
    public AudioClip stage1DoorOpenClip;
    public AudioClip stage2DoorOpenClip;
    public AudioClip gameCompleteClip;

    [Header("Door Close Sounds")]
    public AudioClip stage1DoorCloseClip;
    public AudioClip stage2DoorCloseClip;

    [Header("Timing Settings")]
    [Tooltip("Total time in seconds before doors automatically close after being solved.")]
    public float doorCloseDelay = 4.0f; 
    
    [Tooltip("Triggers the sound this many seconds BEFORE the door closes. Use this to eliminate network lag or match door animations!")]
    public float doorCloseSoundOffset = 1.0f; 

    // Inspector slider to choose exactly how many seconds the emergency release lasts
    [Range(1f, 20f)]
    [Tooltip("How many seconds the doors stay open when the Big Red Emergency Button is pushed.")]
    public float emergencyOpenDuration = 5.0f;

    [Header("Volume Control Sliders")]
    [Range(0f, 1f)] public float puzzleChimeVolume = 0.5f;   
    [Range(0f, 1f)] public float doorOpenVolume = 0.8f;     
    [Range(0f, 1f)] public float doorCloseVolume = 0.7f;    
    [Range(0f, 1f)] public float gameCompleteVolume = 1.0f; 

    // --- NETWORKED STATE VARIABLES ---
    [Networked] public bool ColorSolved { get; set; }
    [Networked] public bool KeySolved { get; set; }
    [Networked] public bool Stage2KeyZoneSolved { get; set; }
    [Networked] public bool Stage2KeypadSolved { get; set; }

    [Networked] private bool Stage1Complete { get; set; }
    [Networked] private bool Stage2Complete { get; set; }

    [Networked] public bool Scanner1Done { get; set; }
    [Networked] public bool Scanner2Done { get; set; }
    [Networked] public bool Scanner3Done { get; set; }
    [Networked] public bool Scanner4Done { get; set; }
    [Networked] public bool FinalRoomsHidden { get; set; }

    private ChangeDetector _changes;

    private Coroutine stage1CloseCoroutine;
    private Coroutine stage2CloseCoroutine;
    private Coroutine emergencyOverrideCoroutine; 

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Play();
        }
    }

    void Update()
    {
        // Button B on Quest Controller OR R Key on Mac -> Master System Reset
        if (OVRInput.GetDown(OVRInput.Button.Two) || (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame))
        {
            Debug.Log("[ProgressManager] Reset Input Detected! Running Master Reset Sequence...");
            RPC_RequestReset();
        }

        // E Key on Mac/Editor ONLY to manually test the emergency override sequence locally
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[ProgressManager] Emergency Override Triggered via Keyboard! Opening all doors temporarily...");
            RPC_RequestEmergencyOverride();
        }
    }

    public override void Render()
    {
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
            if (change == nameof(ColorSolved) && ColorSolved)
                PlayLocalSound(stage1ColorSolvedClip, puzzleChimeVolume);

            if (change == nameof(KeySolved) && KeySolved)
                PlayLocalSound(stage1KeySolvedClip, puzzleChimeVolume);

            if (change == nameof(Stage2KeyZoneSolved) && Stage2KeyZoneSolved)
                PlayLocalSound(stage2KeyZoneClip, puzzleChimeVolume);

            if (change == nameof(Stage2KeypadSolved) && Stage2KeypadSolved)
                PlayLocalSound(stage2KeypadClip, puzzleChimeVolume);

            if (change == nameof(Stage1Complete) && Stage1Complete)
                PlayLocalSound(stage1DoorOpenClip, doorOpenVolume);

            if (change == nameof(Stage2Complete) && Stage2Complete)
                PlayLocalSound(stage2DoorOpenClip, doorOpenVolume);

            if (change == nameof(FinalRoomsHidden) && FinalRoomsHidden)
                PlayLocalSound(gameCompleteClip, gameCompleteVolume);

            if (change == nameof(Stage2KeypadSolved))
            {
                if (!Stage2KeypadSolved)
                {
                    KeypadNetworkBridge[] keypadBridges = UnityEngine.Object.FindObjectsByType<KeypadNetworkBridge>(FindObjectsSortMode.None);
                    foreach (KeypadNetworkBridge bridge in keypadBridges)
                    {
                        if (bridge != null) bridge.ResetLocalKeypadUI();
                    }
                }
            }
        }
    }

    private void PlayLocalSound(AudioClip clip, float volumeMultiplier)
    {
        if (puzzleAudioSource != null && clip != null)
        {
            puzzleAudioSource.PlayOneShot(clip, volumeMultiplier);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayCloseSound(int stageID)
    {
        if (stageID == 1) PlayLocalSound(stage1DoorCloseClip, doorCloseVolume);
        if (stageID == 2) PlayLocalSound(stage2DoorCloseClip, doorCloseVolume);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ColorPuzzleSolved() { ColorSolved = true; CheckStage1Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_KeyPuzzleSolved() { KeySolved = true; CheckStage1Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeyZoneSolved() { Stage2KeyZoneSolved = true; CheckStage2Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Stage2KeypadSolved() { Stage2KeypadSolved = true; CheckStage2Completion(); }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RegisterHandScanner(int id)
    {
        if (id == 1) Scanner1Done = true;
        if (id == 2) Scanner2Done = true;
        if (id == 3) Scanner3Done = true; 
        if (id == 4) Scanner4Done = true;
        CheckStage3Completion();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestEmergencyOverride()
    {
        if (emergencyOverrideCoroutine != null) StopCoroutine(emergencyOverrideCoroutine);
        emergencyOverrideCoroutine = StartCoroutine(EmergencyOverrideRoutine());
    }

    private IEnumerator EmergencyOverrideRoutine()
    {
        if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = true;
        if (stage1RightDoor != null) stage1RightDoor.IsOpen = true;
        if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = true;
        if (stage2RightDoor != null) stage2RightDoor.IsOpen = true;

        // Uses the inspector range slider value to stay open
        yield return new WaitForSeconds(emergencyOpenDuration);

        if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = false;
        if (stage1RightDoor != null) stage1RightDoor.IsOpen = false;
        if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = false;
        if (stage2RightDoor != null) stage2RightDoor.IsOpen = false;

        RPC_PlayCloseSound(1);
        RPC_PlayCloseSound(2);
    }

    private void CheckStage1Completion()
    {
        if (ColorSolved && KeySolved && !Stage1Complete)
        {
            Stage1Complete = true; 
            if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = true;
            if (stage1RightDoor != null) stage1RightDoor.IsOpen = true;

            if (stage1CloseCoroutine != null) StopCoroutine(stage1CloseCoroutine);
            stage1CloseCoroutine = StartCoroutine(CloseStage1DoorsRoutine());
        }
    }

    private IEnumerator CloseStage1DoorsRoutine()
    {
        float timeBeforeSound = Mathf.Max(0f, doorCloseDelay - doorCloseSoundOffset);
        yield return new WaitForSeconds(timeBeforeSound);
        
        RPC_PlayCloseSound(1); 
        
        if (doorCloseSoundOffset > 0f)
        {
            yield return new WaitForSeconds(doorCloseSoundOffset);
        }

        if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = false;
        if (stage1RightDoor != null) stage1RightDoor.IsOpen = false;
    }

    private void CheckStage2Completion()
    {
        if (Stage2KeyZoneSolved && Stage2KeypadSolved && !Stage2Complete)
        {
            Stage2Complete = true; 
            if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = true;
            if (stage2RightDoor != null) stage2RightDoor.IsOpen = true;

            if (stage2CloseCoroutine != null) StopCoroutine(stage2CloseCoroutine);
            stage2CloseCoroutine = StartCoroutine(CloseStage2DoorsRoutine());
        }
    }

    private IEnumerator CloseStage2DoorsRoutine()
    {
        float timeBeforeSound = Mathf.Max(0f, doorCloseDelay - doorCloseSoundOffset);
        yield return new WaitForSeconds(timeBeforeSound);
        
        RPC_PlayCloseSound(2); 
        
        if (doorCloseSoundOffset > 0f)
        {
            yield return new WaitForSeconds(doorCloseSoundOffset);
        }

        if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = false;
        if (stage2RightDoor != null) stage2RightDoor.IsOpen = false;
    }

    private void CheckStage3Completion()
    {
        if (Scanner1Done && Scanner2Done && Scanner3Done && Scanner4Done)
        {
            FinalRoomsHidden = true; 
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestReset()
    {
        Debug.Log("[Master System Wipe] Initiating protected system reset chain...");

        if (stage1CloseCoroutine != null) StopCoroutine(stage1CloseCoroutine);
        if (stage2CloseCoroutine != null) StopCoroutine(stage2CloseCoroutine);
        if (emergencyOverrideCoroutine != null) StopCoroutine(emergencyOverrideCoroutine);

        ColorSolved = false;
        KeySolved = false;
        Stage1Complete = false; 
        Stage2KeyZoneSolved = false;
        Stage2KeypadSolved = false;
        Stage2Complete = false; 
        Scanner1Done = false;
        Scanner2Done = false;
        Scanner3Done = false;
        Scanner4Done = false;
        FinalRoomsHidden = false;

        try {
            if (stage1LeftDoor != null) stage1LeftDoor.IsOpen = false;
            if (stage1RightDoor != null) stage1RightDoor.IsOpen = false;
            if (stage2LeftDoor != null) stage2LeftDoor.IsOpen = false;
            if (stage2RightDoor != null) stage2RightDoor.IsOpen = false;
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] Doors failed: {e.Message}"); }

        try {
            PuzzleManager[] colorPuzzles = UnityEngine.Object.FindObjectsByType<PuzzleManager>(FindObjectsSortMode.None);
            foreach (PuzzleManager p in colorPuzzles) if (p != null) p.ResetPuzzleState();
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] Color Puzzles failed: {e.Message}"); }

        try {
            KeyZone[] keyZones = UnityEngine.Object.FindObjectsByType<KeyZone>(FindObjectsSortMode.None);
            foreach (KeyZone kz in keyZones) if (kz != null) kz.ResetKeyZoneState();

            KeyZoneStage2[] stage2KeyZones = UnityEngine.Object.FindObjectsByType<KeyZoneStage2>(FindObjectsSortMode.None);
            foreach (KeyZoneStage2 kz2 in stage2KeyZones) if (kz2 != null) kz2.ResetKeyZoneState();
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] Key Zones failed: {e.Message}"); }

        try {
            HandScanner[] scanners = UnityEngine.Object.FindObjectsByType<HandScanner>(FindObjectsSortMode.None);
            foreach (HandScanner hs in scanners) if (hs != null) hs.ResetScanner();
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] Hand Scanners failed: {e.Message}"); }

        try {
            NetworkedTimer[] timers = UnityEngine.Object.FindObjectsByType<NetworkedTimer>(FindObjectsSortMode.None);
            foreach (NetworkedTimer t in timers) if (t != null) t.ResetTimerState();
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] Timers failed: {e.Message}"); }

        try {
            RPC_BroadcastClientReset();
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] Client Broadcast failed: {e.Message}"); }
        
        try {
            QRSpawner qrSpawner = UnityEngine.Object.FindFirstObjectByType<QRSpawner>();
            if (qrSpawner != null) qrSpawner.ResetQRSpawnerState();
        } catch (System.Exception e) { Debug.LogError($"[Reset Leak] QR Spawner failed: {e.Message}"); }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_BroadcastClientReset()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.Play();
        }

        KeypadNetworkBridge[] keypadBridges = UnityEngine.Object.FindObjectsByType<KeypadNetworkBridge>(FindObjectsSortMode.None);
        foreach (KeypadNetworkBridge bridge in keypadBridges)
        {
            if (bridge != null) bridge.ResetLocalKeypadUI();
        }
    }
}