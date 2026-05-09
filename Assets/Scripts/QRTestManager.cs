using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

public class QRTestManager : MonoBehaviour
{
    public GameObject cubePrefab;

    private bool spawned = false;

    void Start()
    {
        Debug.Log("QR Test Running");
    }

    void Update()
    {
        if (spawned) return;

        // TEMP TEST (replace later with real anchor callback)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 2f;

            Instantiate(cubePrefab, pos, Quaternion.identity);

            Debug.Log("TEST SPAWN");
            spawned = true;
        }
    }
}