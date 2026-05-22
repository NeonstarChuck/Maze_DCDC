using Fusion;
using UnityEngine;
using TMPro;

public class NetworkedTimer : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    
    [Header("UI Display Target")]
    public TextMeshProUGUI timerText;

    [Networked] private bool IsRunning { get; set; }
    [Networked] private float ElapsedTime { get; set; }

    public override void Spawned()
    {
        if (progressManager == null)
            progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();
            
        UpdateTextDisplay();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_StartTimer()
    {
        // Block execution if already tracking, or if the game map is already cleared
        if (IsRunning || (progressManager != null && progressManager.FinalRoomsHidden)) return;
        
        IsRunning = true;
        Debug.Log("[Timer Engine] Speedrun clock initiated.");
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        // Auto-Stop Check: Freeze immediately if the final trackers finish
        if (progressManager != null && progressManager.FinalRoomsHidden && IsRunning)
        {
            IsRunning = false;
            Debug.Log($"[Timer Engine] Complete! Locked Time: {timerText.text}");
        }

        if (IsRunning)
        {
            ElapsedTime += Runner.DeltaTime;
        }
    }

    public override void Render()
    {
        UpdateTextDisplay();
    }

    private void UpdateTextDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(ElapsedTime / 60F);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((ElapsedTime * 100F) % 100F);

        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }

    public void ResetTimerState()
    {
        if (!Object.HasStateAuthority) return;
        
        IsRunning = false;
        ElapsedTime = 0f;
        Debug.Log("[Timer Engine] Clock safely set back to zero.");
    }
}