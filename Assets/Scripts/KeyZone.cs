using Fusion;
using UnityEngine;

public class KeyZone : NetworkBehaviour
{
    public RoomProgressManager progressManager;
    public float triggerDistance = 0.25f;

    [Networked] private bool Solved { get; set; }

    void Update()
    {
        // Every player checks their own controller locally
        if (Solved) return;

        // Note: You must assign the local headset's controller at runtime 
        // or use OVRInput.GetLocalControllerPosition
        Vector3 handPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        float dist = Vector3.Distance(handPos, transform.position);

        if (dist < triggerDistance)
        {
            Debug.Log("Key puzzle triggered locally");
            // Tell the Host we solved it
            RPC_SetSolved();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_SetSolved()
    {
        Solved = true;
        if (Object.HasStateAuthority)
        {
            progressManager.RPC_KeyPuzzleSolved();
        }
    }
}