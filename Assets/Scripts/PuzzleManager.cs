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
        // Any player can call the RPC
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
                progressManager.RPC_ColorPuzzleSolved();
            }
        }
        else
        {
            Index = 0; // Reset for everyone if someone messes up
        }
    }
}