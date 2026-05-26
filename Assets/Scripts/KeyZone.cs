using Fusion;
using System.Collections;
using TMPro;
using UnityEngine;

public class KeyZone : NetworkBehaviour
{
    [Header("KEY ZONE CONFIGURATION")]
    public RoomProgressManager progressManager;
    [Tooltip("Set this to 1 for Stage 1 (Unhides object), or 2 for Stage 2 (Hides object)")]
    public int stageNumber = 1;
    public float triggerDistance = 0.04f; // Clean 4 centimeters for physical alignment

    [Header("Target Tracking Transform (Small Card Child)")]
    public Transform targetControllerTransform;

    [Header("Puzzle Object (Dynamic Visibility)")]
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
        // === MILESTONE 1: 0.0 Seconds (Processing Begins) ===
        if (statusText != null)
        {
            statusText.text = "Väntar...";
            statusText.color = idleColor;
        }

        // 🔥 FIXED: Sound now triggers immediately alongside the "Processing..." text switch!
        if (audioSource != null && keycardSequenceClip != null)
        {
            audioSource.PlayOneShot(keycardSequenceClip, soundVolume);
        }
        
        // SUSPENSE DELAY: Wait while audio plays and text reads "Processing..."
        yield return new WaitForSeconds(5.5f);

        // === MILESTONE 2: Processing Complete ===
        if (statusText != null) statusText.text = "Snart där...";

        yield return new WaitForSeconds(0.1f);

        // === MILESTONE 3: Access Granted! ===
        if (statusText != null)
        {
            statusText.text = "Klart!";
            statusText.color = successColor;
        }

        // DYNAMIC VISIBILITY CHECK 
        if (hiddenObject != null) 
        {
            if (stageNumber == 1)
                hiddenObject.SetActive(true);  // Stage 1: Reveal/Unhide when solved!
            else
                hiddenObject.SetActive(false); // Stage 2: Hide when solved!
        }

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
        // DYNAMIC NETWORK REFRESH CHECK
        if (hiddenObject != null)
        {
            if (stageNumber == 1)
                hiddenObject.SetActive(IsSolved);  // Stage 1: Active if solved, Inactive if not
            else
                hiddenObject.SetActive(!IsSolved); // Stage 2: Inactive if solved, Active if not
        }

        if (statusText != null)
        {
            if (IsSolved)
            {
                statusText.text = "Klart!";
                statusText.color = successColor;
            }
            else
            {
                statusText.text = "Placera Nyckeln";
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