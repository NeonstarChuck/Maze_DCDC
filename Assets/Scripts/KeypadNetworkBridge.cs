using Fusion;
using UnityEngine;

public class KeypadNetworkBridge : NetworkBehaviour
{
    public RoomProgressManager progressManager;

    [Header("Local Keypad Integration")]
    // Drag your 3D Keypad object (containing the NavKeypad.Keypad script) here!
    public NavKeypad.Keypad localKeypad; 

    // Executed by the local Keypad's UnityEvent (OnAccessGranted)
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

    public void ResetLocalKeypadUI()
    {
        if (localKeypad != null)
        {
            localKeypad.ResetKeypad();
        }
        else
        {
            Debug.LogWarning("[KeypadBridge] Cannot reset! Local Keypad reference is missing in the Inspector.");
        }
    }
}