using Fusion;
using UnityEngine;

public class PuzzleManager : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    private readonly string[] sequence = { "Red", "Yellow", "Green", "Blue" };

    [Networked] private int Index { get; set; }
    [Networked] private bool Solved { get; set; }

    public void PressButton(string color)
    {
        RPC_PressButton(color);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_PressButton(string color)
    {
        if (Solved) return;

        if (color == sequence[Index])
        {
            Index++;
            if (Index >= sequence.Length)
            {
                Solved = true;
                if (progressManager != null)
                {
                    progressManager.RPC_ColorPuzzleSolved();
                }
            }
        }
        else
        {
            Index = 0; 
        }
    }

    // Call this from the progress manager on the host to clear data
    public void ResetPuzzleState()
    {
        if (!Object.HasStateAuthority) return;
        Solved = false;
        Index = 0;
        Debug.Log("[PuzzleManager] Combo tracker reset.");
    }
}