using Fusion;
using UnityEngine;
using TMPro; // --- REQUIRED FOR THE FEEDBACK SCREEN ---

public class PuzzleManager : NetworkBehaviour
{
    public RoomProgressManager progressManager;

    [Header("Numpad UI Display")]
    public TextMeshProUGUI statusText; 
    public Color normalColor = Color.white;
    public Color successColor = Color.green;
    public Color errorColor = Color.red;

    private readonly string[] sequence = { "Red", "Yellow", "Green", "Blue" };

    [Networked] private int Index { get; set; }
    [Networked] private bool Solved { get; set; }
    
    // Tracks error windows across the network to flash the screen red
    [Networked] private TickTimer errorFlashTimer { get; set; } 

    public override void Spawned()
    {
        UpdateDisplay();
    }

    public void PressButton(string color)
    {
        RPC_PressButton(color);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_PressButton(string color)
    {
        // Ignore inputs if solved or currently flashing an error lockout
        if (Solved || !errorFlashTimer.ExpiredOrNotRunning(Runner)) return;

        if (color == sequence[Index])
        {
            Index++;
            if (Index >= sequence.Length)
            {
                Solved = true;
                
                if (progressManager == null) 
                    progressManager = UnityEngine.Object.FindAnyObjectByType<RoomProgressManager>();
                
                if (progressManager != null)
                {
                    progressManager.RPC_ColorPuzzleSolved();
                }
            }
        }
        else
        {
            // Wrong button penalty: wipe index and trigger 1.2-second lock out
            Index = 0; 
            errorFlashTimer = TickTimer.CreateFromSeconds(Runner, 1.2f);
            Debug.Log("[PuzzleManager] Wrong button! Combo wiped.");
        }
    }

    // Render handles updating the screen smoothly on every single player's headset
    public override void Render()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (statusText == null) return;

        // State 1: Puzzle is fully completed (Matches KeyZone Co-op style!)
        if (Solved)
        {
            statusText.text = "Korrekt!\nFortsätt till nästa rum";
            statusText.color = successColor;
            return;
        }

        // State 2: Player recently guessed wrong (Flash Error Lockout)
        if (!errorFlashTimer.ExpiredOrNotRunning(Runner))
        {
            statusText.text = "ERROR: Fel Kombination";
            statusText.color = errorColor;
            return;
        }

        // State 3: Normal entry tracking (Draws progress like: ★ ★ _ _ )
        statusText.color = normalColor;
        string progressString = "";

        for (int i = 0; i < sequence.Length; i++)
        {
            if (i < Index)
            {
                progressString += "★ "; 
            }
            else
            {
                progressString += "_ "; 
            }
        }

        statusText.text = progressString.TrimEnd();
    }

    public void ResetPuzzleState()
    {
        if (!Object.HasStateAuthority) return;
        Solved = false;
        Index = 0;
        errorFlashTimer = TickTimer.None; // Instantly clear error screens on map reset
        Debug.Log("[PuzzleManager] Combo tracker reset.");
    }
}