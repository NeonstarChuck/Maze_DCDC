using Fusion;
using UnityEngine;

public class KeypadNetworkBridge : NetworkBehaviour
{
    public RoomProgressManager progressManager;

    // This public method will be executed by your local Keypad script
    public void OnKeypadAccessGranted()
    {
        Debug.Log("Keypad code sequence matched locally! Routing via RPC to Host.");
        RPC_RequestKeypadSolve();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestKeypadSolve()
    {
        if (progressManager != null)
        {
            progressManager.RPC_Stage2KeypadSolved();
        }
    }
}