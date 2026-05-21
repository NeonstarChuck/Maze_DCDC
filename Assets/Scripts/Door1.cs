using Fusion;
using UnityEngine;

public class Door1 : NetworkBehaviour
{
    [Networked] public bool IsOpen { get; set; }

    private Collider col;
    private Renderer[] rends;

    void Awake()
    {
        col = GetComponent<Collider>();
        rends = GetComponentsInChildren<Renderer>();
    }

    public override void Render()
    {
        if (IsOpen)
        {
            // Turn off physics and visuals when open
            if (col != null && col.enabled) col.enabled = false;
            foreach (var r in rends)
            {
                if (r != null && r.enabled) r.enabled = false;
            }
        }
        else
        {
            // --- THE FIX: Restore physics and visuals when closed ---
            if (col != null && !col.enabled) col.enabled = true;
            foreach (var r in rends)
            {
                if (r != null && !r.enabled) r.enabled = true;
            }
        }
    }
}