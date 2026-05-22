using Fusion;
using UnityEngine;
using TMPro;

public class NetworkedTimer : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    public TextMeshProUGUI timerText;

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

        // Freeze condition: Stop adding time when the escape rooms are hidden (game beat)
        if (progressManager != null && progressManager.FinalRoomsHidden)
        {
            IsRunning = false; 
            Debug.Log($"[Timer] Game Complete! Final Time Frozen at: {FormatTime(ElapsedTime)}");
            return;
        }

        ElapsedTime += Runner.DeltaTime;
    }

    public override void Render()
    {
        if (timerText != null)
        {
            timerText.text = FormatTime(ElapsedTime);
        }
    }

    // --- 🚨 THE MATCHING RPC FOR YOUR START BUTTON 🚨 ---
    // This allows any player (All) to tell the Host (StateAuthority) to start the clock
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StartTimer()
    {
        // Don't restart the clock if the game is already won!
        if (progressManager != null && progressManager.FinalRoomsHidden) return;

        IsRunning = true;
        Debug.Log("[Timer Host Sync] Speedrun timer started via Player Button Click.");
    }

    // Called automatically by RoomProgressManager when pressing the master 'B' button
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