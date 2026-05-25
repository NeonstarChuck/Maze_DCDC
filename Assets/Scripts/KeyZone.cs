using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;

public class KeyZone : NetworkBehaviour
{
    [Header("KEY ZONE CONFIGURATION")]
    public RoomProgressManager progressManager;
    [Tooltip("Set this to 1 for Stage 1, or 2 for Stage 2")]
    public int stageNumber = 1;
    public float triggerDistance = 0.04f; // Clean 4 centimeters for physical alignment

    [Header("Target Tracking Transform (Small Card Child)")]
    public Transform targetControllerTransform;

    [Header("Puzzle Object (Will HIDE when solved)")]
    public GameObject hiddenObject;

    [Header("Co-op Status UI")]
    public TMP_Text statusText;
    public Color idleColor = Color.white;
    public Color successColor = Color.green;

    [Header("Audio Custom Timeline Setup")]
    public AudioSource audioSource;
    public AudioClip keycardSequenceClip;
    [Range(0f, 1f)] public float soundVolume = 0.8f;

    // Networked synchronization states
    [Networked] public bool IsSolved { get; set; }
    [Networked] private bool IsProcessing { get; set; }

    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        UpdateVisualsLocal();
    }

    void Update()
    {
        if (IsSolved || IsProcessing || targetControllerTransform == null) return;

        float currentDistance = Vector3.Distance(transform.position, targetControllerTransform.position);

        if (currentDistance <= triggerDistance)
        {
            Debug.Log($"[KeyZone] Card detected within distance ({currentDistance}m). Booting timeline sequence...");
            RPC_StartSwipeSequence();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_StartSwipeSequence()
    {
        if (IsSolved || IsProcessing) return;
        IsProcessing = true;
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this))
        {
            if (change == nameof(IsProcessing) && IsProcessing)
            {
                StartCoroutine(SwipeTimingSequenceRoutine());
            }
            if (change == nameof(IsSolved))
            {
                UpdateVisualsLocal();
            }
        }
    }

    private IEnumerator SwipeTimingSequenceRoutine()
    {
        // === MILESTONE 1: 0.0 Seconds ===
        if (statusText != null)
        {
            statusText.text = "Processing...";
            statusText.color = idleColor;
        }
        
        // SUSPENSE DELAY: Wait 2.5 seconds while display says "Processing..."
        yield return new WaitForSeconds(2.5f);

        // === MILESTONE 2: 2.5 Seconds ===
        if (statusText != null) statusText.text = "Granting Access...";

        // Play the 4-second track locally on the reader box object (Bling sound starts)
        if (audioSource != null && keycardSequenceClip != null)
        {
            audioSource.PlayOneShot(keycardSequenceClip, soundVolume);
        }

        // Wait 2.0 seconds for the internal blings/silence portion of your clip to pass
        yield return new WaitForSeconds(2.0f);

        // === MILESTONE 3: 4.5 Seconds (The Audio Track hits the "Granted Sound") ===
        if (statusText != null)
        {
            statusText.text = "Granted!";
            statusText.color = successColor;
        }

        if (hiddenObject != null) hiddenObject.SetActive(false);

        // Commit state to host authority at the VERY END of the timeline
        if (HasStateAuthority)
        {
            IsSolved = true;
            IsProcessing = false;
            
            // Fire RPC to open doors only AFTER the visual sequence finishes completely
            if (progressManager != null)
            {
                if (stageNumber == 1) progressManager.RPC_KeyPuzzleSolved();
                if (stageNumber == 2) progressManager.RPC_Stage2KeyZoneSolved();
            }
        }
    }

    private void UpdateVisualsLocal()
    {
        if (hiddenObject != null) hiddenObject.SetActive(!IsSolved);

        if (statusText != null)
        {
            if (IsSolved)
            {
                statusText.text = "Granted!";
                statusText.color = successColor;
            }
            else
            {
                statusText.text = "Insert Card";
                statusText.color = idleColor;
            }
        }
    }

    public void ResetKeyZoneState()
    {
        StopAllCoroutines();
        
        if (HasStateAuthority)
        {
            IsSolved = false;
            IsProcessing = false;
        }

        if (audioSource != null) audioSource.Stop();
        UpdateVisualsLocal();
    }
}