using Fusion;
using UnityEngine;
using TMPro;

public class NetworkedTimer : NetworkBehaviour
{
    public RoomProgressManager progressManager;

    [Header("Timer Displays (Add Start and End Texts Here)")]
    public TextMeshProUGUI[] timerTexts;

    [Networked] private float ElapsedTime { get; set; }
    [Networked] private bool IsRunning { get; set; }

    public override void Spawned()
    {
        if (progressManager == null)
            progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!IsRunning) return;

        // Freeze condition: Stop updating time when the game is won
        if (progressManager != null && progressManager.FinalRoomsHidden)
        {
            IsRunning = false; 
            Debug.Log($"[Timer] Game Complete! Final Time Frozen at: {FormatTime(ElapsedTime)}");
            return;
        }

        ElapsedTime += Runner.DeltaTime;
    }

    // Render updates ALL linked screens simultaneously every frame
    public override void Render()
    {
        string currentFormattedTime = FormatTime(ElapsedTime);

        // Loop through every text component plugged into the Inspector array
        foreach (TextMeshProUGUI textDisplay in timerTexts)
        {
            if (textDisplay != null)
            {
                textDisplay.text = currentFormattedTime;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StartTimer()
    {
        if (progressManager != null && progressManager.FinalRoomsHidden) return;

        IsRunning = true;
        Debug.Log("[Timer Host Sync] Speedrun timer started via Player Button Click.");
    }

    public void ResetTimerState()
    {
        if (!Object.HasStateAuthority) return;
        
        IsRunning = false;
        ElapsedTime = 0f;
        Debug.Log("[Timer] Speedrun clock wiped back to 00:00.00 via Master Reset.");
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int fraction = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }
}