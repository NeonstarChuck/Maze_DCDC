using Fusion;
using UnityEngine;

public class RoomProgressManager : NetworkBehaviour
{
    public Door1 leftDoor;
    public Door1 rightDoor;

    [Networked] public bool ColorSolved { get; set; }
    [Networked] public bool KeySolved { get; set; }

    // This detects changes across the network
    private ChangeDetector _changes;

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(ColorSolved):
                case nameof(KeySolved):
                    CheckCompletion();
                    break;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ColorPuzzleSolved()
    {
        ColorSolved = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_KeyPuzzleSolved()
    {
        KeySolved = true;
    }

    private void CheckCompletion()
    {
        if (ColorSolved && KeySolved)
        {
            Debug.Log("Puzzles Done - Opening Doors");
            if (leftDoor != null) leftDoor.IsOpen = true;
            if (rightDoor != null) rightDoor.IsOpen = true;
        }
    }
}