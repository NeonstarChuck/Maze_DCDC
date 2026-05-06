using UnityEngine;

public class TableZoneDistance : MonoBehaviour
{
    public Transform ball;
    public GameObject door;
    public float triggerDistance = 0.2f;

    void Update()
    {
        if (ball == null) return;

        float dist = Vector3.Distance(ball.position, transform.position);

        if (dist < triggerDistance)
        {
            Debug.Log("Ball close enough → hiding door");
            door.SetActive(false);
        }
    }
}