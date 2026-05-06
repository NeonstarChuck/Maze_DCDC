using UnityEngine;

public class ControllerBall : MonoBehaviour
{
    public Transform xrController;

    void Update()
    {
        transform.position = xrController.position;
        transform.rotation = xrController.rotation;
    }
}