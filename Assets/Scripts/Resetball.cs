using UnityEngine;
using Fusion;

public class ResetBall : NetworkBehaviour
{
    private Vector3 startPosition;
    private Rigidbody rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    public void ResetPosition()
    {
        // Stop movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Move back
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }
}